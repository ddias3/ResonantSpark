﻿using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

using ResonantSpark.Gameplay;
using ResonantSpark.Gamemode;
using ResonantSpark.Camera;
using ResonantSpark.Character;
using ResonantSpark.Utility;

namespace ResonantSpark {
    namespace Service {
        public enum ComboState : int {
            None,
            InCombo,
        }

        public struct CharacterVulnerability {
            public bool throwable;
            public bool strikable;
            public bool armored;
        }

        public class FightingGameService : MonoBehaviour, IFightingGameService {

            public GameObject mapCamera;
            public Vector3 underLevel = new Vector3(0, -100, 0);
            public Transform spawnPoint;
            public float offset = 3.0f;
            public Transform outOfBounds;
            public Transform cameraStart;

            public List<Transform> levelBoundaries;

            private PlayerService playerService;
            private PersistenceService persistenceService;

            private List<InGameEntity> entities;
            private Dictionary<InGameEntity, int> numComboHits;
            private Dictionary<InGameEntity, int> comboScalingIndex;

            private HashSet<BlockType> requiredBlocks;

            private Dictionary<InGameEntity, ComboState> prevComboState;

            private List<(InGameEntity, InGameEntity, Hit, Action<AttackPriority, int>, Action<AttackPriority, int>)> hitQueue;

            private FrameEnforcer frame;

            private IGamemode gamemode;
            private IHookExpose gamemodeHooks;

            public void Start() {
                frame = GameObject.FindGameObjectWithTag("rspTime").GetComponent<FrameEnforcer>();
                frame.AddUpdate((int)FramePriority.ServiceFG, new System.Action<int>(FrameUpdate));
                frame.AddUpdate((int)FramePriority.LateService, new System.Action<int>(LateFrameUpdate));

                playerService = GetComponent<PlayerService>();
                persistenceService = GetComponent<PersistenceService>();

                entities = new List<InGameEntity>();
                numComboHits = new Dictionary<InGameEntity, int>();
                comboScalingIndex = new Dictionary<InGameEntity, int>();

                requiredBlocks = new HashSet<BlockType>();

                prevComboState = new Dictionary<InGameEntity, ComboState>();

                hitQueue = new List<(InGameEntity, InGameEntity, Hit, Action<AttackPriority, int>, Action<AttackPriority, int>)>();
                mapCamera.SetActive(false);

                EventManager.TriggerEvent<Events.ServiceReady, Type>(typeof(FightingGameService));
            }

            public void RegisterInGameEntity(InGameEntity entity) {
                entities.Add(entity);
                numComboHits[entity] = 0;
                comboScalingIndex[entity] = 0;
                prevComboState.Add(entity, ComboState.None);
            }

            public void RemoveInGameEntity(InGameEntity entity) {
                entities.Remove(entity);
                numComboHits[entity] = 0;
                comboScalingIndex[entity] = 0;
                prevComboState.Remove(entity);
            }

            public void CreateGamemode() {
                GameObject newGameMode = GameObject.Instantiate(persistenceService.GetGamemode());
                newGameMode.name = "Gamemode";
                this.gamemode = newGameMode.GetComponent<IGamemode>();
                this.gamemodeHooks = newGameMode.GetComponent<IHookExpose>();

                this.gamemode.CreateDependencies(persistenceService, playerService, this, GetComponent<UiService>(), GetComponent<InputService>());
            }

            public void SetUpGamemode() {
                gamemode.SetUp();

                HookUpGamemode(gamemodeHooks.GetHooks());
            }

            private void HookUpGamemode(Dictionary<string, UnityEventBase> hooks) {
                HookReceive hookReceive = new HookReceive(hooks);
                hookReceive.HookIn("loadMainMenu", new UnityAction(LoadMainMenu));
            }

            public bool IsCurrentFGChar(InGameEntity entity) {
                return gamemode.IsCurrentFGChar(entity);
            }

            public void Strike(InGameEntity hitEntity, InGameEntity byEntity, Hit hit, Action<AttackPriority, int> onHit, Action<AttackPriority, int> onBlock) {
                Debug.Log(hitEntity);
                hitQueue.Add((hitEntity, byEntity, hit, onHit, onBlock));
            }

            public void Throw(InGameEntity hitEntity, InGameEntity byEntity, Action<AttackPriority, int> onSuccess, Action<AttackPriority, int> onBreak) {

            }

            public void HitStunStart(FightingGameCharacter fgChar) {
                gamemode.OnHitStunStart(fgChar);
            }

            public void HitStunEnd(FightingGameCharacter fgChar) {
                gamemode.OnHitStunEnd(fgChar);
            }

            private void ResolveHits() {
                if (hitQueue.Count == 0) {
                    return;
                }

                Dictionary<InGameEntity, InGameEntity> hitBy = new Dictionary<InGameEntity, InGameEntity>();

                for (int n = 0; n < hitQueue.Count; ++n) {
                    InGameEntity hitEntity = hitQueue[n].Item1;
                    InGameEntity byEntity = hitQueue[n].Item2;

                    hitBy[hitEntity] = byEntity;
                }

                foreach (KeyValuePair<InGameEntity, InGameEntity> kvp in hitBy) {
                    var hitEntity = kvp.Key;

                    if (HitEachOther(hitEntity, hitBy)) {
                        // TODO: Fix this case just like the other one.
                        InGameEntity ent0 = hitEntity;
                        InGameEntity ent1 = hitBy[hitEntity];

                        (InGameEntity, InGameEntity, Hit, Action<AttackPriority, int>, Action<AttackPriority, int>) info0 = hitQueue.Single(tuple => tuple.Item1 == ent0);
                        (InGameEntity, InGameEntity, Hit, Action<AttackPriority, int>, Action<AttackPriority, int>) info1 = hitQueue.Single(tuple => tuple.Item1 == ent1);

                        if (IsCurrentFGChar(info0.Item1)) {
                            InGameEntity fgChar = info0.Item1;
                            int prevNumHits = numComboHits[fgChar];
                            numComboHits[fgChar]++;
                            gamemode.OnGameEntityNumHitsChange(fgChar, numComboHits[fgChar], prevNumHits);

                            comboScalingIndex[fgChar]++;
                        }
                        if (IsCurrentFGChar(info1.Item1)) {
                            InGameEntity fgChar = info1.Item1;
                            int prevNumHits = numComboHits[fgChar];
                            numComboHits[fgChar]++;
                            gamemode.OnGameEntityNumHitsChange(fgChar, numComboHits[fgChar], prevNumHits);

                            comboScalingIndex[fgChar]++;
                        }

                        info0.Item4?.Invoke(info1.Item3.priority, GetComboScale(info0.Item1, info0.Item3));
                        info1.Item4?.Invoke(info0.Item3.priority, GetComboScale(info1.Item1, info1.Item3));
                    }
                    else {
                        InGameEntity ent0 = hitEntity;

                        if (IsCurrentFGChar(ent0)) {
                            FightingGameCharacter fgChar = (FightingGameCharacter)ent0;
                            if (fgChar.GetCharacterVulnerability().strikable) {
                                IEnumerable<(InGameEntity, InGameEntity, Hit, Action<AttackPriority, int>, Action<AttackPriority, int>)> relevantTuples = hitQueue.Where(tuple => tuple.Item1 == ent0);
                                bool hitSuccessful = relevantTuples.Any(tuple => !fgChar.CheckBlockSuccess(tuple.Item3));

                                if (hitSuccessful) {
                                    foreach ((InGameEntity, InGameEntity, Hit, Action<AttackPriority, int>, Action<AttackPriority, int>) tuple in relevantTuples) {
                                        numComboHits[fgChar]++;
                                        int prevNumHits = numComboHits[fgChar];
                                        comboScalingIndex[fgChar]++;

                                        gamemode.OnGameEntityNumHitsChange(fgChar, numComboHits[fgChar], prevNumHits);

                                        tuple.Item4?.Invoke(AttackPriority.None, GetComboScale(tuple.Item1, tuple.Item3));
                                    }
                                }
                                else {
                                    foreach ((InGameEntity, InGameEntity, Hit, Action<AttackPriority, int>, Action<AttackPriority, int>) tuple in relevantTuples) {
                                        tuple.Item5?.Invoke(AttackPriority.None, tuple.Item3.blockDamage);
                                    }
                                }
                            }
                        }
                        else {
                            IEnumerable<(InGameEntity, InGameEntity, Hit, Action<AttackPriority, int>, Action<AttackPriority, int>)> relevantTuples = hitQueue.Where(tuple => tuple.Item1 == ent0);
                            foreach ((InGameEntity, InGameEntity, Hit, Action<AttackPriority, int>, Action<AttackPriority, int>) tuple in relevantTuples) {
                                tuple.Item4?.Invoke(AttackPriority.None, GetComboScale(tuple.Item1, tuple.Item3));
                            }
                        }
                    }
                }

                hitQueue.Clear();
            }

            private bool HitEachOther(InGameEntity hitEntity, Dictionary<InGameEntity, InGameEntity> hitBy) {
                InGameEntity byEntity = hitBy[hitEntity];

                if (hitBy.TryGetValue(byEntity, out InGameEntity byEntityResponse)) {
                    if (hitEntity == byEntityResponse) {
                        return true;
                    }
                }

                return false;
            }

            private void FrameUpdate(int frameIndex) {
                ResetComboCounter();
                ResolveHits();
            }

            private void LateFrameUpdate(int frameIndex) {
                foreach (InGameEntity entity in entities) {
                    prevComboState[entity] = entity.GetComboState();
                }
            }

            private List<float> comboScaling = new List<float> { 1.0f, 0.8f, 0.65f, 0.5f, 0.45f, 0.4f, 0.35f, 0.3f, 0.25f };

            private int GetComboScale(InGameEntity fgChar, Hit hit) {
                if (numComboHits[fgChar] == 0) {
                    for (int n = 0; n < comboScaling.Count; ++n) {
                        if (comboScaling[n] <= hit.comboScaling) {
                            comboScalingIndex[fgChar] = n;
                        }
                    }
                }
                if (comboScalingIndex[fgChar] >= comboScaling.Count) {
                    return (int)(hit.hitDamage * comboScaling[comboScaling.Count - 1]);
                }
                else {
                    return (int)(hit.hitDamage * comboScaling[comboScalingIndex[fgChar]]);
                }
            }

            private void ResetComboCounter() {
                playerService.ForEach((fgChar, id) => {
                    if (fgChar.GetComboState() == ComboState.None && prevComboState[fgChar] == ComboState.InCombo) {
                        int prevNumHits = numComboHits[fgChar];
                        numComboHits[fgChar] = 0;
                        comboScalingIndex[fgChar] = 0;

                        gamemode.OnGameEntityNumHitsChange(fgChar, 0, prevNumHits);
                    }
                });
            }

            public void LoadMainMenu() {
                Debug.Log("LoadMainMenu called");
                SceneManager.LoadScene("Scenes/Menu/MainMenu2");
            }

            public int GetMaxPlayers() {
                return gamemode.GetMaxPlayers();
            }

            public int GetNumHits(InGameEntity fgChar) {
                return numComboHits[fgChar];
            }

            public void DisableControl() {
                playerService.ForEach((fgChar, id) => {
                    fgChar.SetControlEnable(false);
                });
            }

            public void EnableControl() {
                playerService.ForEach((fgChar, id) => {
                    fgChar.SetControlEnable(true);
                });
            }

            public void OpeningCamera() {
                gamemode.CameraEnable(false);
                mapCamera.SetActive(true);
            }

            public void FightingGameCamera() {
                mapCamera.SetActive(false);
                gamemode.CameraEnable(true);
            }

            public Transform GetSpawnPoint() {
                return spawnPoint;
            }

            public float GetSpawnPointOffset() {
                return offset;
            }

            public Vector2 ScreenOrientation(FightingGameCharacter fgChar) {
                return gamemode.ScreenOrientation(fgChar);
            }

            public Transform GetCameraStart() {
                return cameraStart;
            }

            public List<Transform> GetLevelBoundaries() {
                return levelBoundaries;
            }

            public void SetGameTimeScaling(float scaling) {
                gamemode.SetGameTimeScaling(scaling);
            }

            public float GetGameTimeScaling() {
                return gamemode.GetGameTimeScaling();
            }
        }
    }
}