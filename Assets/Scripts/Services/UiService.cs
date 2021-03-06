﻿using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.UI;

namespace ResonantSpark {
    namespace Service {
        public class UiService : MonoBehaviour, IUiService {

            public HealthBar healthBarP0;
            public HealthBar healthBarP1;

            public RoundTimer roundTimer;
            public OpeningText openingText;

            public RoundCounter roundCounterP0;
            public RoundCounter roundCounterP1;

            public ComboCounter comboCounterP0;
            public ComboCounter comboCounterP1;

            public void Start() {
                EventManager.TriggerEvent<Events.ServiceReady, Type>(typeof(UiService));
            }

            public void SetTime(float time) {
                roundTimer.SetTime(time);
            }

            public void SetMainScreenText(string text) {
                openingText.SetOpeningText(text);
            }

            public void HideMainScreenText() {
                openingText.Hide();
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

            public void SetComboCounter(int playerId, int comboCounter) {
                if (playerId == 0) {
                    comboCounterP0.SetNumHits(comboCounter);
                }
                else if (playerId == 1) {
                    comboCounterP1.SetNumHits(comboCounter);
                }
            }

            public void HideComboCounter(int playerId) {
                if (playerId == 0) {
                    comboCounterP0.Hide();
                }
                else if (playerId == 1) {
                    comboCounterP1.Hide();
                }
            }

            public void SetRoundWins(int playerId, int numWins) {
                if (playerId == 0) {
                    roundCounterP0.SetRoundCount(numWins);
                }
                else if (playerId == 1) {
                    roundCounterP1.SetRoundCount(numWins);
                }
            }
        }
    }
}