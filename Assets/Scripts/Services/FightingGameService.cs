using System;
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

            private List<(InGameEntity hitEntity, InGameEntity byEntity, Hit hit, Action<AttackPriority, int> onHit, Action<AttackPriority, int> onBlock)> strikeQueue;
            private List<(InGameEntity hitEntity, InGameEntity byEntity, Hit hit, Action<AttackPriority> onGrabbed)> throwQueue;

            private Dictionary<InGameEntity, InGameEntity> entityGrabbingEntity;
            private Dictionary<InGameEntity, InGameEntity> entityGrabbingEntityContinue;
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

                strikeQueue = new List<(InGameEntity hitEntity, InGameEntity byEntity, Hit hit, Action<AttackPriority, int> onHit, Action<AttackPriority, int> onBlock)>();
                throwQueue = new List<(InGameEntity hitEntity, InGameEntity byEntity, Hit hit, Action<AttackPriority> onGrabbed)>();
                entityGrabbingEntity = new Dictionary<InGameEntity, InGameEntity>();
                entityGrabbingEntityContinue = new Dictionary<InGameEntity, InGameEntity>();
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
                strikeQueue.Add((hitEntity, byEntity, hit, onHit, onBlock));
            }

            public void Throw(InGameEntity hitEntity, InGameEntity byEntity, Hit hit, Action<AttackPriority> onGrabbed) {
                throwQueue.Add((hitEntity, byEntity, hit, onGrabbed));
            }

            public void MaintainsGrab(InGameEntity grabbedEntity, InGameEntity byEntity) {
                entityGrabbingEntityContinue[byEntity] = grabbedEntity;
            }

            public void HitStunStart(FightingGameCharacter fgChar) {
                gamemode.OnHitStunStart(fgChar);
            }

            public void HitStunEnd(FightingGameCharacter fgChar) {
                gamemode.OnHitStunEnd(fgChar);
            }

            private void ResolveStrikesAndThrows() {
                if (strikeQueue.Count == 0) {
                    return;
                }

                Dictionary<InGameEntity, InGameEntity> hitBy = new Dictionary<InGameEntity, InGameEntity>();

                for (int n = 0; n < strikeQueue.Count; ++n) {
                    InGameEntity hitEntity = strikeQueue[n].hitEntity;
                    InGameEntity byEntity = strikeQueue[n].byEntity;

                    hitBy[hitEntity] = byEntity;
                }

                foreach (KeyValuePair<InGameEntity, InGameEntity> kvp in hitBy) {
                    var hitEntity = kvp.Key;

                    if (HitEachOther(hitEntity, hitBy)) {
                        // TODO: Fix this case just like the other one.
                        InGameEntity ent0 = hitEntity;
                        InGameEntity ent1 = hitBy[hitEntity];

                        (InGameEntity hitEntity, InGameEntity byEntity, Hit hit, Action<AttackPriority, int> onHit, Action<AttackPriority, int> onBlock) info0 = strikeQueue.Single(tuple => tuple.hitEntity == ent0);
                        (InGameEntity hitEntity, InGameEntity byEntity, Hit hit, Action<AttackPriority, int> onHit, Action<AttackPriority, int> onBlock) info1 = strikeQueue.Single(tuple => tuple.hitEntity == ent1);

                        if (IsCurrentFGChar(info0.hitEntity)) {
                            InGameEntity fgChar = info0.hitEntity;
                            int prevNumHits = numComboHits[fgChar];
                            numComboHits[fgChar]++;
                            gamemode.OnGameEntityNumHitsChange(fgChar, numComboHits[fgChar], prevNumHits);

                            comboScalingIndex[fgChar]++;
                        }
                        if (IsCurrentFGChar(info1.hitEntity)) {
                            InGameEntity fgChar = info1.hitEntity;
                            int prevNumHits = numComboHits[fgChar];
                            numComboHits[fgChar]++;
                            gamemode.OnGameEntityNumHitsChange(fgChar, numComboHits[fgChar], prevNumHits);

                            comboScalingIndex[fgChar]++;
                        }

                        info0.onHit?.Invoke(info1.hit.priority, GetComboScaleDamage(info0.hitEntity, info0.hit.comboScaling, info0.hit.hitDamage));
                        info1.onHit?.Invoke(info0.hit.priority, GetComboScaleDamage(info1.hitEntity, info1.hit.comboScaling, info1.hit.hitDamage));
                    }
                    else {
                        InGameEntity ent0 = hitEntity;

                        if (IsCurrentFGChar(ent0)) {
                            FightingGameCharacter fgChar = (FightingGameCharacter)ent0;
                            if (fgChar.GetCharacterVulnerability().strikable) {
                                IEnumerable<(InGameEntity hitEntity, InGameEntity byEntity, Hit hit, Action<AttackPriority, int> onHit, Action<AttackPriority, int> onBlock)> relevantTuples = strikeQueue.Where(tuple => tuple.hitEntity == ent0);
                                bool hitSuccessful = relevantTuples.Any(tuple => !fgChar.CheckBlockSuccess(tuple.hit));

                                if (hitSuccessful) {
                                    foreach ((InGameEntity hitEntity, InGameEntity byEntity, Hit hit, Action<AttackPriority, int> onHit, Action<AttackPriority, int> onBlock) tuple in relevantTuples) {
                                        numComboHits[fgChar]++;
                                        int prevNumHits = numComboHits[fgChar];
                                        comboScalingIndex[fgChar]++;

                                        gamemode.OnGameEntityNumHitsChange(fgChar, numComboHits[fgChar], prevNumHits);

                                        tuple.onHit?.Invoke(AttackPriority.None, GetComboScaleDamage(tuple.hitEntity, tuple.hit.comboScaling, tuple.hit.hitDamage));
                                    }
                                }
                                else {
                                    foreach ((InGameEntity hitEntity, InGameEntity byEntity, Hit hit, Action<AttackPriority, int> onHit, Action<AttackPriority, int> onBlock) tuple in relevantTuples) {
                                        tuple.onBlock?.Invoke(AttackPriority.None, tuple.hit.blockDamage);
                                    }
                                }
                            }
                        }
                        else {
                            IEnumerable<(InGameEntity hitEntity, InGameEntity byEntity, Hit hit, Action<AttackPriority, int> onHit, Action<AttackPriority, int> onBlock)> relevantTuples = strikeQueue.Where(tuple => tuple.hitEntity == ent0);
                            foreach ((InGameEntity hitEntity, InGameEntity byEntity, Hit hit, Action<AttackPriority, int> onHit, Action<AttackPriority, int> onBlock) tuple in relevantTuples) {
                                //tuple.onHit?.Invoke(AttackPriority.None, GetComboScale(tuple.hitEntity, tuple.hit.comboScaling, tuple.hit.hitDamage));
                                    // I think I'll have strikes on other entities not have combo scaling.
                                tuple.onHit?.Invoke(AttackPriority.None, GetComboScaleDamage(tuple.hitEntity, tuple.hit.comboScaling, tuple.hit.hitDamage));
                            }
                        }
                    }
                }

                strikeQueue.Clear();
                throwQueue.Clear();
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
                ResolveStrikesAndThrows();
            }

            private void LateFrameUpdate(int frameIndex) {
                foreach (InGameEntity entity in entities) {
                    prevComboState[entity] = entity.GetComboState();
                }
            }

            private List<float> comboScaling = new List<float> { 1.0f, 0.8f, 0.65f, 0.5f, 0.45f, 0.4f, 0.35f, 0.3f, 0.25f };

            public int GetComboScaleDamage(InGameEntity hitEntity, float initComboScaling, int hitDamage) {
                if (numComboHits[hitEntity] == 0) {
                    for (int n = 0; n < comboScaling.Count; ++n) {
                        if (comboScaling[n] <= initComboScaling) {
                            comboScalingIndex[hitEntity] = n;
                        }
                    }
                }
                if (comboScalingIndex[hitEntity] >= comboScaling.Count) {
                    return (int)(hitDamage * comboScaling[comboScaling.Count - 1]);
                }
                else {
                    return (int)(hitDamage * comboScaling[comboScalingIndex[hitEntity]]);
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