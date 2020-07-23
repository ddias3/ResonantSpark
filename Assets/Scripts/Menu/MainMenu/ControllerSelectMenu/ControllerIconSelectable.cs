using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using ResonantSpark.DeviceManagement;

namespace ResonantSpark {
    namespace Menu {
        public class ControllerIconSelectable : Selectable {
            public Transform root;
            public Vector3 rightDistance;
            public TMP_Text text;
            public MeshRenderer checkMark;

            private int position;

            private PlayerLabel label = PlayerLabel.None;
            private Vector3 center;

            private bool readied = false;

            private List<int> invalidPositions;

            public new void Awake() {
                base.Awake();

                invalidPositions = new List<int> { -2, 2 };

                center = root.transform.position;
                position = 0;
                readied = false;
                checkMark.enabled = false;

                eventHandler.On("submit", () => {
                    if (position == -1) {
                        label = PlayerLabel.Player1;
                        readied = true;
                        checkMark.enabled = true;
                    }
                    else if (position == 0) {
                        label = PlayerLabel.None;
                        readied = false;
                        checkMark.enabled = false;
                    }
                    else if (position == 1) {
                        label = PlayerLabel.Player2;
                        readied = true;
                        checkMark.enabled = true;
                    }
                });

                eventHandler.On("cancel", () => {
                    if (readied) {
                        readied = false;
                        checkMark.enabled = false;
                    }
                });

                eventHandler.On("deactivate", () => {
                    gameObject.SetActive(false);
                });
                eventHandler.On("activate", () => {
                    readied = false;
                    position = 0;
                    checkMark.enabled = false;
                    invalidPositions = new List<int> { -2, 2 };
                    UpdateDisplay();

                    gameObject.SetActive(true);
                });
            }

            public PlayerLabel GetPlayerLabel() {
                return label;
            }

            public void MovePosition(int moveAmount) {
                this.position += moveAmount;

                if (invalidPositions.Contains(this.position)) {
                    this.position -= moveAmount;
                }

                UpdateDisplay();
            }

            public int GetPosition() {
                return position;
            }

            public void DisablePosition(int invalidPosition) {
                invalidPositions.Add(invalidPosition);
            }

            public void EnablePosition(int validPosition) {
                invalidPositions.Remove(validPosition);
            }

            public bool Readied() {
                return readied;
            }

            public void UpdateDisplay() {
                root.transform.position = center + rightDistance * position;
            }

            public void SetText(string text) {
                this.text.text = text;
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