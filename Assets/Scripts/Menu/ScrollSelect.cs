using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using ResonantSpark.DeviceManagement;

namespace ResonantSpark {
    namespace Menu {
        public class ScrollEvent : UnityEvent<string> { }

        public class ScrollSelect : Selectable {

            // TODO: make Unity Editor scripts that let you set this in the editor...
            public List<ScrollSelectOption<string>> options;

            public TMPro.TMP_Text displayText;

            public ScrollEvent scrollEvent = new ScrollEvent();

            private int currSelected = 0;

            private Transform wordLeft;
            private Transform wordRight;

            public void Start() {
                On("left", () => {
                    //if (persistence.player1 == devMap || persistence.player2 == devMap) {
                    currSelected -= 1;
                    if (currSelected < 0) {
                        currSelected = options.Count - 1;
                    }
                    displayText.SetText(options[currSelected].name);
                    scrollEvent.Invoke(options[currSelected].callbackData);
                    //}
                });

                On("right", () => {
                    //if (persistence.player1 == devMap || persistence.player2 == devMap) {
                    currSelected += 1;
                    if (currSelected >= options.Count) {
                        currSelected = 0;
                    }
                    displayText.SetText(options[currSelected].name);
                    scrollEvent.Invoke(options[currSelected].callbackData);
                    //}
                });
            }

            public void SetInitSelected(int index) {
                currSelected = index;
                displayText.SetText(options[currSelected].name);
            }

            public void AddListener(UnityAction<string> callback) {
                this.scrollEvent.AddListener(callback);
            }

            public override Transform GetTransform() {
                return transform;
            }

            public override Vector3 Offset() {
                return Vector3.forward;
            }

            public override float Width() {
                return 0.0f;
            }
        }

        [Serializable]
        public class ScrollSelectOption<T0> {
            public string name;
            public T0 callbackData;
        }
    }
}