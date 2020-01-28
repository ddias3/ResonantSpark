using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Gameplay;
using ResonantSpark.Utility;
using ResonantSpark.Gamemode;

namespace ResonantSpark {
    namespace Service {
        public class FightingGameService : MonoBehaviour, IFightingGameService {

            public GameObject mapCamera;
            public Vector3 underLevel = new Vector3(0, -100, 0);
            public Transform spawnPoint;
            public float offset = 3.0f;
            public Transform outOfBounds;
            public Transform cameraStart;

            private PlayerService playerService;
            private FightingGameService fgService;
            private PersistenceService persistenceService;
            private UIService uiService;

            private new FightingGameCamera camera;

            private FrameEnforcer frame;

            private IGamemode gamemode;

            public void Start() {
                frame = GameObject.FindGameObjectWithTag("rspTime").GetComponent<FrameEnforcer>();
                playerService = GetComponent<PlayerService>();
                fgService = GetComponent<FightingGameService>();
                persistenceService = GetComponent<PersistenceService>();
                uiService = GetComponent<UIService>();

                EventManager.TriggerEvent<Events.ServiceReady, Type>(typeof(FightingGameService));
            }

            public void CreateGamemode() {
                GameObject newGameMode = GameObject.Instantiate(persistenceService.GetGamemode());
                newGameMode.name = "Gamemode";
                this.gamemode = newGameMode.GetComponent<OneOnOneRoundBased>();

                GameObject newCamera = GameObject.Instantiate(persistenceService.GetCamera());
                newCamera.name = "FightingGameCamera";
                this.camera = newCamera.GetComponent<FightingGameCamera>();

                mapCamera.SetActive(false);

                playerService.SetMaxPlayers(gamemode.GetMaxPlayers());
            }

            public void SetUpGamemode() {
                gamemode.SetUp(playerService, fgService, uiService);
                camera.SetUpCamera(cameraStart);
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
        }
    }
}