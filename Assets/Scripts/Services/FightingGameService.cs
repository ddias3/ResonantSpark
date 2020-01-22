using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Gameplay;
using ResonantSpark.Utility;
using ResonantSpark.Gamemode;

namespace ResonantSpark {
    namespace Service {
        public class FightingGameService : MonoBehaviour, IFightingGameService {

            public Vector3 underLevel = new Vector3(0, -100, 0);
            public Transform spawnPoint;
            public float offset = 3.0f;
            public Transform outOfBounds;

            private PlayerService playerService;
            private FightingGameService fgService;
            private PersistenceService persistenceService;

            private FrameEnforcer frame;

            private IGamemode gamemode;

            public void Start() {
                frame = GameObject.FindGameObjectWithTag("rspTime").GetComponent<FrameEnforcer>();
                playerService = GetComponent<PlayerService>();
                fgService = GetComponent<FightingGameService>();
                persistenceService = GetComponent<PersistenceService>();

                EventManager.TriggerEvent<Events.ServiceReady, Type>(typeof(FightingGameService));
            }

            public void CreateGamemode() {
                GameObject newGameMode = GameObject.Instantiate(persistenceService.GetGamemode());
                newGameMode.name = "Gamemode";
                this.gamemode = newGameMode.GetComponent<OneOnOneRoundBased>();

                playerService.SetMaxPlayers(gamemode.GetMaxPlayers());
            }

            public void SetUpGamemode() {
                gamemode.SetUp(playerService, fgService);
            }

            public Transform GetSpawnPoint() {
                return spawnPoint;
            }

            public float GetSpawnPointOffset() {
                return offset;
            }

            public Transform GetCharacterRoot(FightingGameCharacter fgChar) {
                throw new System.NotImplementedException();
            }
        }
    }
}