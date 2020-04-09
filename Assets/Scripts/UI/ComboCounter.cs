using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using ResonantSpark.Utility;

namespace ResonantSpark {
    namespace UI {
        public class ComboCounter : GameUiElement {

            public enum ComboCounterState : int {
                Hidden = 0,
                Appear,
                Display,
                Hide,
            }

            public bool exitScreenLeft;
            public float appearTime = 0.1f;
            public float displayHidingTime = 0.5f;
            public float hideTime = 0.1f;

            public TextMeshProUGUI numberGui;

            public ComboCounterState state;

            private float timer = 0.0f;

            private int numHits = 0;

            private RectTransform rectTransform;
            private Vector2 displayPos = Vector2.zero;
            private Vector2 hiddenPos;

            public new void Start() {
                base.Start();

                rectTransform = GetComponent<RectTransform>();
                numberGui.text = this.numHits.ToString();

                if (exitScreenLeft) {
                    hiddenPos = new Vector2(-rectTransform.rect.width, 0);
                }
                else {
                    hiddenPos = new Vector2(rectTransform.rect.width, 0);
                }

                Debug.LogFormat("HiddenPos = {0}, DisplayPos = {1}, startPos = {2}", hiddenPos.ToString("F3"), displayPos.ToString("F3"), rectTransform.anchoredPosition.ToString("F3"));
                Debug.LogFormat("   sizeDelta = {0}", rectTransform.sizeDelta.ToString("F3"));
                rectTransform.anchoredPosition = hiddenPos;

                EventManager.StartListening<Events.StartGame>(new UnityEngine.Events.UnityAction(StartGame));
            }

            private void StartGame() {
                this.enabled = true;
                this.state = ComboCounterState.Hidden;
            }

            public void SetNumHits(int numHits) {
                if (state == ComboCounterState.Hidden || state == ComboCounterState.Hide) {
                    if (numHits >= 2) {
                        numberGui.text = numHits.ToString();

                        state = ComboCounterState.Appear;
                        timer = 0.0f;
                    }
                }
                else if (state == ComboCounterState.Display) {
                    if (numHits < this.numHits) {
                        if (numHits >= 2) {
                            numberGui.text = numHits.ToString();

                            state = ComboCounterState.Appear;
                            timer = 0.0f;
                        }
                    }
                    else {
                        numberGui.text = numHits.ToString();
                    }
                }
                else {
                    numberGui.text = numHits.ToString();
                }

                this.numHits = numHits;
            }

            public void Hide() {
                state = ComboCounterState.Hide;
                timer = 0.0f;
            }

            public void Update() {
                switch (state) {
                    case ComboCounterState.Hidden:
                        rectTransform.anchoredPosition = hiddenPos;
                        break;
                    case ComboCounterState.Appear:
                        rectTransform.anchoredPosition = Vector2.Lerp(hiddenPos, displayPos, timer / appearTime);
                        if (timer >= appearTime) {
                            state = ComboCounterState.Display;
                        }
                        break;
                    case ComboCounterState.Display:
                        rectTransform.anchoredPosition = displayPos;
                        break;
                    case ComboCounterState.Hide:
                        if (timer < displayHidingTime) {
                            rectTransform.anchoredPosition = displayPos;
                        }
                        else {
                            rectTransform.anchoredPosition = Vector2.Lerp(displayPos, hiddenPos, (timer - displayHidingTime) / hideTime);
                            if (timer >= displayHidingTime + hideTime) {
                                state = ComboCounterState.Hidden;
                            }
                        }
                        break;
                }

                timer += Time.deltaTime;
            }
        }
    }
}