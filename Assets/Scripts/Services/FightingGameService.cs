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

            private PlayerService playerService;
            private PersistenceService persistenceService;

            private FrameEnforcer frame;

            private IGamemode gamemode;

            public void Start() {
                frame = GameObject.FindGameObjectWithTag("rspTime").GetComponent<FrameEnforcer>();
                playerService = GetComponent<PlayerService>();
                persistenceService = GetComponent<PersistenceService>();

                EventManager.TriggerEvent<Events.ServiceReady, Type>(typeof(FightingGameService));
            }

            public void CreateGamemode() {
                gamemode = persistenceService.GetGamemode();
                playerService.SetMaxPlayers(gamemode.GetMaxPlayers());
            }

            public Transform GetCharacterRoot(FightingGameCharacter fgChar) {
                throw new System.NotImplementedException();
            }

            public void RunAnimationState(FightingGameCharacter fgChar, string animationStateName) {
                throw new System.NotImplementedException();
            }
        }
    }
}