using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Gameplay;
using ResonantSpark.Gamemode;
using ResonantSpark.Camera;
using ResonantSpark.Character;

namespace ResonantSpark {
    namespace Service {
        public enum ComboState : int {
            None,
            InCombo,
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

            private new StageLeakCamera camera;

            private List<InGameEntity> entities;
            private Dictionary<InGameEntity, int> numComboHits;
            private Dictionary<InGameEntity, int> comboScalingIndex;

            private Dictionary<InGameEntity, ComboState> prevComboState;

            private List<(InGameEntity, InGameEntity, Hit, Action<AttackPriority, int>)> hitQueue;

            private FrameEnforcer frame;

            private IGamemode gamemode;

            public void Start() {
                frame = GameObject.FindGameObjectWithTag("rspTime").GetComponent<FrameEnforcer>();
                frame.AddUpdate((int)FramePriority.ServiceFG, new System.Action<int>(FrameUpdate));
                frame.AddUpdate((int)FramePriority.LateService, new System.Action<int>(LateFrameUpdate));

                playerService = GetComponent<PlayerService>();
                persistenceService = GetComponent<PersistenceService>();

                entities = new List<InGameEntity>();
                numComboHits = new Dictionary<InGameEntity, int>();
                comboScalingIndex = new Dictionary<InGameEntity, int>();

                prevComboState = new Dictionary<InGameEntity, ComboState>();

                hitQueue = new List<(InGameEntity, InGameEntity, Hit, Action<AttackPriority, int>)>();

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
                this.gamemode = newGameMode.GetComponent<OneOnOneRoundBased>();

                GameObject newCamera = GameObject.Instantiate(persistenceService.GetCamera());
                newCamera.name = "FightingGameCamera";
                this.camera = newCamera.GetComponent<StageLeakCamera>();

                mapCamera.SetActive(false);

                playerService.SetMaxPlayers(gamemode.GetMaxPlayers());
            }

            public void SetUpGamemode() {
                gamemode.SetUp(playerService, this, GetComponent<UiService>());
                camera.SetUpCamera(this);
            }

            public bool IsCurrentFGChar(InGameEntity entity) {
                return gamemode.IsCurrentFGChar(entity);
            }

            public void Hit(InGameEntity hitEntity, InGameEntity byEntity, Hit hit, Action<AttackPriority, int> callback) {
                Debug.Log(hitEntity);
                hitQueue.Add((hitEntity, byEntity, hit, callback));
            }

            private void ResolveHits() {
                if (hitQueue.Count == 0) {
                    return;
                }

                List<InGameEntity> entities = new List<InGameEntity>();
                Dictionary<InGameEntity, InGameEntity> hitBy = new Dictionary<InGameEntity, InGameEntity>();

                for (int n = 0; n < hitQueue.Count; ++n) {
                    InGameEntity hitEntity = hitQueue[n].Item1;
                    InGameEntity byEntity = hitQueue[n].Item2;

                    hitBy[hitEntity] = byEntity;
                }

                foreach (KeyValuePair<InGameEntity, InGameEntity> kvp in hitBy) {
                    var hitEntity = kvp.Key;
                    if (entities.Contains(hitEntity)) continue;

                    if (HitEachOther(hitEntity, hitBy)) {
                        InGameEntity ent0 = hitEntity;
                        InGameEntity ent1 = hitBy[hitEntity];

                        entities.Add(hitBy[hitEntity]); //hitBy.Remove(hitBy[hitEntity]);
                        entities.Add(hitEntity);        //hitBy.Remove(hitEntity);

                        (InGameEntity, InGameEntity, Hit, Action<AttackPriority, int>) info0 = hitQueue.Single(tuple => tuple.Item1 == ent0);
                        (InGameEntity, InGameEntity, Hit, Action<AttackPriority, int>) info1 = hitQueue.Single(tuple => tuple.Item1 == ent1);

                        if (IsCurrentFGChar(info0.Item1)) {
                            InGameEntity fgChar = info0.Item1;
                            numComboHits[fgChar]++;
                            gamemode.OnGameEntityNumHitsChange(fgChar, numComboHits[fgChar]);

                            comboScalingIndex[fgChar]++;
                        }
                        if (IsCurrentFGChar(info1.Item1)) {
                            InGameEntity fgChar = info1.Item1;
                            numComboHits[fgChar]++;
                            gamemode.OnGameEntityNumHitsChange(fgChar, numComboHits[fgChar]);

                            comboScalingIndex[fgChar]++;
                        }

                        info0.Item4?.Invoke(info1.Item3.priority, GetComboScale(info0.Item1, info0.Item3));
                        info1.Item4?.Invoke(info0.Item3.priority, GetComboScale(info1.Item1, info1.Item3));
                    }
                    else {
                        InGameEntity ent0 = hitEntity;

                        entities.Add(hitEntity); //hitBy.Remove(hitEntity);

                        (InGameEntity, InGameEntity, Hit, Action<AttackPriority, int>) info0 = hitQueue.Single(tuple => tuple.Item1 == ent0);

                        if (IsCurrentFGChar(info0.Item1)) {
                            InGameEntity fgChar = info0.Item1;
                            numComboHits[fgChar]++;
                            gamemode.OnGameEntityNumHitsChange(fgChar, numComboHits[fgChar]);

                            comboScalingIndex[fgChar]++;
                        }

                        info0.Item4?.Invoke(AttackPriority.None, GetComboScale(info0.Item1, info0.Item3));
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
                playerService.EachFGChar((id, fgChar) => {
                    if (fgChar.GetComboState() == ComboState.None && prevComboState[fgChar] == ComboState.InCombo) {
                        numComboHits[fgChar] = 0;
                        comboScalingIndex[fgChar] = 0;

                        gamemode.OnGameEntityNumHitsChange(fgChar, 0);
                    }
                });
            }

            public int GetNumHits(InGameEntity fgChar) {
                return numComboHits[fgChar];
            }

            public void DisableControl() {
                playerService.EachFGChar((id, fgChar) => {
                    fgChar.SetControlEnable(false);
                });
            }

            public void EnableControl() {
                playerService.EachFGChar((id, fgChar) => {
                    fgChar.SetControlEnable(true);
                });
            }

            public void OpeningCamera() {
                camera.gameObject.SetActive(false);
                mapCamera.SetActive(true);
            }

            public void FightingGameCamera() {
                mapCamera.SetActive(false);
                camera.gameObject.SetActive(true);
            }

            public Transform GetSpawnPoint() {
                return spawnPoint;
            }

            public float GetSpawnPointOffset() {
                return offset;
            }

            public void ResetCamera() {
                camera.ResetCameraPosition();
            }

            public Vector2 ScreenOrientation(FightingGameCharacter fgChar) {
                return camera.ScreenOrientation(fgChar.transform);
            }

            public Transform GetCameraStart() {
                return cameraStart;
            }

            public List<Transform> GetLevelBoundaries() {
                return levelBoundaries;
            }
        }
    }
}