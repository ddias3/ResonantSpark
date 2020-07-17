using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Character;

namespace ResonantSpark {
    namespace Menu {
        public class ControllerIconSelectable : Selectable {
            public Transform root;
            public Vector3 rightDistance;

            private int position;

            private PlayerLabel label = PlayerLabel.None;
            private Vector3 center;

            public void Start() {
                center = root.transform.position;
                position = 0;

                eventHandler.On("submit", () => {
                    Debug.Log("Display a controller highlighted");

                    if (position == -1) {
                        label = PlayerLabel.Player1;
                    }
                    else if (position == 0) {
                        label = PlayerLabel.None;
                    }
                    else if (position == 1) {
                        label = PlayerLabel.Player2;
                    }
                });

                eventHandler.On("deactivate", () => {
                    gameObject.SetActive(false);
                });
                eventHandler.On("activate", () => {
                    gameObject.SetActive(true);
                });
            }

            public PlayerLabel GetPlayerLabel() {
                return label;
            }

            public void MovePosition(int moveAmount) {
                this.position += moveAmount;
                if (this.position < -1) {
                    this.position = -1;
                }
                else if (this.position > 1) {
                    this.position = 1;
                }

                UpdateDisplay();
            }

            public void UpdateDisplay() {
                root.transform.position = center + rightDistance * position;
            }

            public override Transform GetTransform() {
                return root.transform;
            }

            public override Vector3 Offset() {
                throw new System.NotImplementedException();
            }

            public override float Width() {
                throw new System.NotImplementedException();
            }
        }
    }
}