using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Character;

namespace ResonantSpark {
    namespace Menu {
        public class ControllerSelect : Selectable {
            public ControllerIconSelectable controllerIconPrefab;

            public Transform controllerPointsStart;
            public Transform controllerPointsOffset;
            public int numControllers = 1;

            private List<ControllerIconSelectable> controllers;
            private ControllerIconSelectable player1 = null;
            private ControllerIconSelectable player2 = null;

            public void Start() {
                controllers = new List<ControllerIconSelectable>();

                Vector3 offset = controllerPointsOffset.position - controllerPointsStart.position;
                for (int n = 0; n < numControllers; ++n) {
                    ControllerIconSelectable controller = Instantiate(controllerIconPrefab,
                            controllerPointsStart.position + n * offset,
                            Quaternion.identity,
                            transform)
                        .GetComponent<ControllerIconSelectable>();

                    controller.TriggerEvent("deactivate");
                    controllers.Add(controller);
                }

                eventHandler.On("activate", () => {
                    player1 = null;
                    player2 = null;
                    for (int n = 0; n < controllers.Count; ++n) {
                        controllers[n].TriggerEvent("activate");
                    }
                });
                eventHandler.On("deactivate", () => {
                    for (int n = 0; n < controllers.Count; ++n) {
                        controllers[n].TriggerEvent("deactivate");
                    }
                });
                eventHandler.On("left", () => {
                    // NEED to pass in controller information to determine which one moves left.
                    for (int n = 0; n < numControllers; ++n) {
                        controllers[n].MovePosition(-1);
                    }
                });
                eventHandler.On("right", () => {
                    // NEED to pass in controller information to determine which one moves right.
                    for (int n = 0; n < numControllers; ++n) {
                        controllers[n].MovePosition(1);
                    }
                });
            }

            public bool ValidateControllers() {
                for (int n = 0; n < numControllers; ++n) {
                    controllers[n].TriggerEvent("submit");
                }

                for (int n = 0; n < controllers.Count; ++n) {
                    ControllerIconSelectable curr = controllers[n];
                    if (curr.GetPlayerLabel() == PlayerLabel.Player1) {
                        player1 = curr;
                    }
                    else if (curr.GetPlayerLabel() == PlayerLabel.Player2) {
                        player2 = curr;
                    }
                }

                return player1 != null || player2 != null;
            }

            public override float Width() {
                return 0.0f;
            }

            public override Transform GetTransform() {
                return transform;
            }

            public override Vector3 Offset() {
                return Vector3.forward;
            }
        }
    }
}