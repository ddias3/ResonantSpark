using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Builder;
using ResonantSpark.Gameplay;
using ResonantSpark.Input;

namespace ResonantSpark {
    namespace Service {
        public class PersistenceService : MonoBehaviour, IPersistenceService {
            private string gamemode;

            public void Start() {
                    //TODO: Create a singleton to persist data between scenes.
                    //  for now just return one-on-one, round-based
                gamemode = "oneOnOneRoundBased";

                EventManager.TriggerEvent<Events.ServiceReady, Type>(typeof(PersistenceService));
            }

            public string GetGamemode() {
                return gamemode;
            }

            public void SetGamemode(string gamemode) {
                this.gamemode = gamemode;
            }
        }
    }
}