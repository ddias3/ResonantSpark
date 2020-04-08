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
            private UiService uiService;

            private new StageLeakCamera camera;

            private List<(InGameEntity, InGameEntity, HitBox, Action<AttackPriority>)> hitQueue;

            private FrameEnforcer frame;

            private IGamemode gamemode;

            private Dictionary<FightingGameCharacter, int> numComboAttacks;
            private Dictionary<FightingGameCharacter, int> numComboHits;

            public void Start() {
                frame = GameObject.FindGameObjectWithTag("rspTime").GetComponent<FrameEnforcer>();
                frame.AddUpdate((int)FramePriority.Service, new System.Action<int>(FrameUpdate));
                playerService = GetComponent<PlayerService>();
                persistenceService = GetComponent<PersistenceService>();
                uiService = GetComponent<UiService>();

                hitQueue = new List<(InGameEntity, InGameEntity, HitBox, Action<AttackPriority>)>();

                numComboAttacks = new Dictionary<FightingGameCharacter, int>();
                numComboHits = new Dictionary<FightingGameCharacter, int>();

                EventManager.TriggerEvent<Events.ServiceReady, Type>(typeof(FightingGameService));
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
                gamemode.SetUp(playerService, this, uiService);
                camera.SetUpCamera(this);
            }

            public void Hit(InGameEntity hitEntity, InGameEntity byEntity, HitBox hitBox, Action<AttackPriority> callback) {
                Debug.Log(hitEntity);
                hitQueue.Add((hitEntity, byEntity, hitBox, callback));
            }

            private void ResolveHits() {
                List<InGameEntity> entities = new List<InGameEntity>();
                Dictionary<InGameEntity, InGameEntity> hitBy = new Dictionary<InGameEntity, InGameEntity>();

                for (int n = 0; n < hitQueue.Count; ++n) {
                    InGameEntity hitEntity = hitQueue[n].Item1;
                    InGameEntity byEntity = hitQueue[n].Item2;

                    hitBy.Add(hitEntity, byEntity);
                }

                foreach (KeyValuePair<InGameEntity, InGameEntity> kvp in hitBy) {
                    var hitEntity = kvp.Key;
                    if (entities.Contains(hitEntity)) continue;

                    if (HitEachOther(hitEntity, hitBy)) {
                        InGameEntity ent0 = hitEntity;
                        InGameEntity ent1 = hitBy[hitEntity];

                        entities.Add(hitBy[hitEntity]); //hitBy.Remove(hitBy[hitEntity]);
                        entities.Add(hitEntity);        //hitBy.Remove(hitEntity);

                        (InGameEntity, InGameEntity, HitBox, Action<AttackPriority>) info0 = hitQueue.Single(tuple => tuple.Item1 == ent0);
                        (InGameEntity, InGameEntity, HitBox, Action<AttackPriority>) info1 = hitQueue.Single(tuple => tuple.Item1 == ent1);

                        if (info0.Item1.GetType() == typeof(FightingGameCharacter)) {
                            numComboHits[(FightingGameCharacter)info0.Item1]++;
                        }
                        if (info1.Item1.GetType() == typeof(FightingGameCharacter)) {
                            numComboHits[(FightingGameCharacter)info1.Item1]++;
                        }
                        info0.Item4?.Invoke(info1.Item3.hit.priority);
                        info1.Item4?.Invoke(info0.Item3.hit.priority);
                    }
                    else {
                        InGameEntity ent0 = hitEntity;

                        entities.Add(hitEntity); //hitBy.Remove(hitEntity);

                        (InGameEntity, InGameEntity, HitBox, Action<AttackPriority>) info0 = hitQueue.Single(tuple => tuple.Item1 == ent0);

                        if (info0.Item1.GetType() == typeof(FightingGameCharacter)) {
                            numComboHits[(FightingGameCharacter)info0.Item1]++;
                        }

                        info0.Item4?.Invoke(AttackPriority.None);
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
                playerService.OneToOthers((id, fgChar, others) => {
                    for (int n = 0; n < others.Count; ++n) {
                        //Debug.LogFormat("#{0} : distance from other[{1}] = {2}", id, others[n].id, Vector3.Distance(fgChar.transform.position, others[n].transform.position));

                        if (others[n].GetGroundRelation() == Character.GroundRelation.GROUNDED) {
                            if (Vector3.Distance(fgChar.transform.position, others[n].transform.position) < 0.68f) {
                                fgChar.PushAway(0.68f, others[n]);
                            }
                        }
                    }
                });

                CalculateComboScaling();
                ResolveHits();

                //if (UnityEngine.Input.GetKeyDown(KeyCode.Keypad1)) {
                //    player0ComboHits += 1;
                //    uiService.SetComboCounter(0, player0ComboHits);
                //}
                //if (UnityEngine.Input.GetKeyDown(KeyCode.Keypad4)) {
                //    player0ComboHits = 0;
                //    uiService.HideComboCounter(0);
                //}

                //if (UnityEngine.Input.GetKeyDown(KeyCode.Keypad3)) {
                //    player1ComboHits += 1;
                //    uiService.SetComboCounter(1, player1ComboHits);
                //}
                //if (UnityEngine.Input.GetKeyDown(KeyCode.Keypad6)) {
                //    player1ComboHits = 0;
                //    uiService.HideComboCounter(1);
                //}
            }

            private List<float> comboScaling = new List<float> { 1.0f, 0.8f, 0.6f, 0.5f, 0.45f, 0.4f, 0.35f, 0.3f, 0.25f };

            private void CalculateComboScaling() {
                playerService.EachFGChar((id, fgChar) => {
                    if (!fgChar.InHitStun()) {
                        numComboHits[fgChar] = 0;
                        numComboAttacks[fgChar] = 0;

                        //uiService.HideComboCounter(id);
                    }
                });
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