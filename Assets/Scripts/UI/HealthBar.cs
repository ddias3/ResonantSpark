using System;
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

            public float delayTime = 0.0f; //1.5f;
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

            public new void Awake() {
                base.Awake();

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
                    timer += Time.deltaTime; // this doesn't work here -> gameTime.DeltaTime("frameDelta", "game");
                }
            }

                // TODO: look into this as a possibility.
            //public override void Serialize() {
            //    fieldMap = new Dictionary<string, UnityAction>();
            //    fieldMap.Add("healthSync", new UnityAction(SyncHealthValues));
            //    fieldMap1Value = new Dictionary<string, UnityAction<object>>();
            //    fieldMap1Value.Add("maxHealth", new UnityAction<int>(SetMaxHealth));
            //    fieldMap1Value.Add("health", new UnityAction<int>(SetHealthValue));
            //    fieldMap1Value.Add("healthSyncPause", new UnityAction<bool>(FreezeSync));
            //}

            public override void SetValue(string field) {
                switch (field) {
                    case "healthSync":
                        SyncHealthValues();
                        break;
                    default:
                        throw new InvalidOperationException("Health bar field invalid: " + field);
                }
            }

            public override void SetValue(string field, object value0) {
                switch (field) {
                    case "maxHealth":
                        SetMaxHealth((int) value0);
                        break;
                    case "health":
                        SetHealthValue((int) value0);
                        break;
                    case "healthSyncPause":
                        FreezeSync((bool) value0);
                        break;
                    default:
                        throw new InvalidOperationException("Health bar field with 1 value invalid: " + field);
                }
            }

            public override void SetValue(string field, object value0, object value1) {
                switch (field) {
                    default:
                        throw new InvalidOperationException("Health bar field with 2 values invalid: " + field);
                }
            }
        }
    }
}