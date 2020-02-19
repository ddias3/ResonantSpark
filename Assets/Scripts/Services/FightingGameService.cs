using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Gameplay;
using ResonantSpark.Utility;
using ResonantSpark.Gamemode;
using ResonantSpark.Camera;

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

            private FrameEnforcer frame;

            private IGamemode gamemode;

            public void Start() {
                frame = GameObject.FindGameObjectWithTag("rspTime").GetComponent<FrameEnforcer>();
                frame.AddUpdate((int)FramePriority.Service, new System.Action<int>(FrameUpdate));
                playerService = GetComponent<PlayerService>();
                persistenceService = GetComponent<PersistenceService>();
                uiService = GetComponent<UiService>();

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