using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.UI;

namespace ResonantSpark {
    namespace Service {
        public class UiService : MonoBehaviour, IUiService {

            public HealthBar healthBarP1;
            public HealthBar healthBarP2;

            public RoundTimer roundTimer;
            public OpeningText openingText;

            public RoundCounter roundCounterP1;
            public RoundCounter roundCounterP2;

            public ComboCounter comboCounterP1;
            public ComboCounter comboCounterP2;

            public void Start() {
                EventManager.TriggerEvent<Events.ServiceReady, Type>(typeof(UiService));
            }

            public void SetTime(float time) {
                roundTimer.SetTime(time);
            }

            public void SetNoTime() {
                roundTimer.SetNoTime();
            }

            public void SetMainScreenText(string text) {
                openingText.SetOpeningText(text);
            }

            public void HideMainScreenText() {
                openingText.Hide();
            }

            public void HealthBarSyncPause(int playerId, bool pause) {
                if (playerId == 0) {
                    healthBarP1.FreezeSync(pause);
                }
                else if (playerId == 1) {
                    healthBarP2.FreezeSync(pause);
                }
            }

            public void HealthBarSync(int playerId) {
                if (playerId == 0) {
                    healthBarP1.SyncHealthValues();
                }
                else if (playerId == 1) {
                    healthBarP2.SyncHealthValues();
                }
            }

            public void SetHealth(int playerId, int health) {
                if (playerId == 0) {
                    healthBarP1.SetHealthValue(health);
                }
                else if (playerId == 1) {
                    healthBarP2.SetHealthValue(health);
                }
            }

            public void SetMaxHealth(int playerId, int health) {
                if (playerId == 0) {
                    healthBarP1.SetMaxHealth(health);
                }
                else if (playerId == 1) {
                    healthBarP2.SetMaxHealth(health);
                }
            }

            public void SetComboCounter(int playerId, int comboCounter) {
                if (playerId == 0) {
                    comboCounterP1.SetNumHits(comboCounter);
                }
                else if (playerId == 1) {
                    comboCounterP2.SetNumHits(comboCounter);
                }
            }

            public void HideComboCounter(int playerId) {
                if (playerId == 0) {
                    comboCounterP1.Hide();
                }
                else if (playerId == 1) {
                    comboCounterP2.Hide();
                }
            }

            public void SetRoundWins(int playerId, int numWins) {
                if (playerId == 0) {
                    roundCounterP1.SetRoundCount(numWins);
                }
                else if (playerId == 1) {
                    roundCounterP2.SetRoundCount(numWins);
                }
            }
        }
    }
}