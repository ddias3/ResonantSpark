using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ResonantSpark {
    namespace UI {
        public class HealthBar : GameUiElement {
            public Image fillHealth;
            public Image fillRed;

            public float delayTime = 1.5f;
            public float flushSpeed = 1.0f;
            public float flushEndingLinearSpeed = 16f;

            private int maxHealth = 1;
            private int prevHealth = 1;
            private int currHealth = 1;

            private bool freezeSync = false;
            private bool flushHealthBar = false;

            private float timer;

            private float fullHealthWidth;
            private float anchorOffset;

            private UnityAction startGameCallback;

            public new void Start() {
                base.Start();

                this.enabled = false;
                anchorOffset = fillHealth.rectTransform.anchoredPosition.x;
                fullHealthWidth = fillHealth.rectTransform.sizeDelta.x;

                EventManager.StartListening<Events.StartGame>(startGameCallback = new UnityAction(StartGame));
            }

            public void OnDestroy() {
                EventManager.StopListening<Events.StartGame>(startGameCallback);
            }

            private void StartGame() {
                this.enabled = true;
            }

            public void SetMaxHealth(int maxHealth) {
                this.maxHealth = maxHealth;
                currHealth = maxHealth;
                SyncHealthValues();
            }

            public void SetHealthValue(int healthValue) {
                if (healthValue > 0) {
                    currHealth = healthValue;
                }
                else {
                    currHealth = 0;
                }
                timer = 0.0f;
            }

            public void SyncHealthValues() {
                prevHealth = currHealth;
                flushHealthBar = false;
                timer = 0.0f;
            }

            public void FreezeSync(bool value) {
                if (value) {
                    freezeSync = true;
                }
                else {
                    freezeSync = false;
                    timer = delayTime + 0.0001f;
                }
            }

            public void FlushHealthBar() {
                flushHealthBar = true;
            }

            public void SetFillDimensions() {
                fillHealth.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, fullHealthWidth * currHealth / maxHealth);
                fillRed.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, fullHealthWidth * prevHealth / maxHealth);
            }

            public void Update() {
                if (timer > delayTime || flushHealthBar) {
                    if (prevHealth - currHealth < 400) {
                        prevHealth -= Mathf.RoundToInt(flushEndingLinearSpeed * 1 / FrameEnforcer.FRAME_TIME * gameTime.DeltaTime("frameDelta", "game"));
                    }
                    else {
                        prevHealth = Mathf.RoundToInt(Mathf.Lerp(prevHealth, currHealth, gameTime.DeltaTime("frameDelta", "game") * flushSpeed));
                    }

                    if (prevHealth <= currHealth + 20) {
                        SyncHealthValues();
                    }
                }

                SetFillDimensions();

                if (!freezeSync) {
                    timer += gameTime.DeltaTime("frameDelta", "game");
                }
            }
        }
    }
}