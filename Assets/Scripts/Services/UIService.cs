using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.UI;

namespace ResonantSpark {
    namespace Service {
        public class UIService : MonoBehaviour, IUIService {

            public HealthBar healthBarP0;
            public HealthBar healthBarP1;
            public RoundTimer roundTimer;

            public void Start() {
                EventManager.TriggerEvent<Events.ServiceReady, Type>(typeof(UIService));
            }

            public void SetTime(float time) {
                roundTimer.SetTime(time);
            }

            public void SetHealth(int playerId, int health) {
                if (playerId == 0) {
                    healthBarP0.SetHealthValue(health);
                }
                else if (playerId == 1) {
                    healthBarP1.SetHealthValue(health);
                }
            }

            public void SetMaxHealth(int playerId, int health) {
                if (playerId == 0) {
                    healthBarP0.SetMaxHealth(health);
                }
                else if (playerId == 1) {
                    healthBarP1.SetMaxHealth(health);
                }
            }
        }
    }
}