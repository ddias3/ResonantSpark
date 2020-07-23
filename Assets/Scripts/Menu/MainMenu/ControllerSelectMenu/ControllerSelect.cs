using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.DeviceManagement;

namespace ResonantSpark {
    namespace Menu {
        public class ControllerSelect : Selectable {
            public ControllerIconSelectable controllerIconPrefab;
            public ControllerIconSelectable keyboardIconPrefab;

            public Transform controllerPointsStart;
            public Transform controllerPointsOffset;

            private DeviceManager deviceManager;
            private Vector3 offset;

            private Dictionary<GameDeviceMapping, ControllerIconSelectable> devToIconMap;
            private List<ControllerIconSelectable> controllers;

            private GameDeviceMapping player1 = null;
            private GameDeviceMapping player2 = null;

            private bool active = false;

            private Action<GameDeviceMapping, GameDeviceMapping> onMenuCompleteCallback;

            public void Start() {
                active = false;

                devToIconMap = new Dictionary<GameDeviceMapping, ControllerIconSelectable>();
                controllers = new List<ControllerIconSelectable>();

                deviceManager = DeviceManager.Get();

                deviceManager.AddOnDeviceAdded(OnDeviceAdded);
                deviceManager.AddOnDeviceRemoved(OnDeviceRemoved);

                offset = controllerPointsOffset.position - controllerPointsStart.position;

                deviceManager.ForEach((devMap, index) => {
                    CreateControllerIcon(devMap);
                });

                On("activate", () => {
                    active = true;
                    player1 = null;
                    player2 = null;
                    for (int n = 0; n < controllers.Count; ++n) {
                        controllers[n].TriggerEvent("activate");
                    }
                });
                On("deactivate", () => {
                    active = false;
                    for (int n = 0; n < controllers.Count; ++n) {
                        controllers[n].TriggerEvent("deactivate");
                    }
                });
                On("left", (GameDeviceMapping devMap) => {
                    ControllerIconSelectable currIcon = devToIconMap[devMap];
                    if (!currIcon.Readied()) {
                        currIcon.MovePosition(-1);
                    }
                });
                On("right", (GameDeviceMapping devMap) => {
                    ControllerIconSelectable currIcon = devToIconMap[devMap];
                    if (!currIcon.Readied()) {
                        currIcon.MovePosition(1);
                    }
                });
                On("submit", (GameDeviceMapping devMap) => {
                    ControllerIconSelectable currIcon = devToIconMap[devMap];
                    if (!currIcon.Readied()) {
                        if (currIcon.GetPosition() == -1) {
                            player1 = devMap;
                            MoveOtherIcons(currIcon, invalidPosition: -1, moveAmount: 1);
                        }
                        else if (currIcon.GetPosition() == 1) {
                            player2 = devMap;
                            MoveOtherIcons(currIcon, invalidPosition: 1, moveAmount: -1);
                        }
                    }
                    else {
                        onMenuCompleteCallback(player1, player2);
                    }
                    currIcon.TriggerEvent("submit");
                });
                On("cancel", (GameDeviceMapping devMap) => {
                    ControllerIconSelectable currIcon = devToIconMap[devMap];
                    if (currIcon.GetPosition() == -1) {
                        player1 = null;
                        EnablePosition(-1);
                    }
                    else if (currIcon.GetPosition() == 1) {
                        player2 = null;
                        EnablePosition(1);
                    }
                    currIcon.TriggerEvent("cancel");
                });
            }

            public void OnDestroy() {
                deviceManager.RemoveOnDeviceAdded(OnDeviceAdded);
                deviceManager.RemoveOnDeviceRemoved(OnDeviceRemoved);
            }

            public void OnDeviceAdded(GameDeviceMapping devMap) {
                CreateControllerIcon(devMap);
            }

            public void OnDeviceRemoved(GameDeviceMapping devMap) {
                ControllerIconSelectable currIcon = devToIconMap[devMap];
                controllers.Remove(currIcon);
                devToIconMap.Remove(devMap);

                Destroy(currIcon.gameObject);
            }

            public void OnMenuComplete(Action<GameDeviceMapping, GameDeviceMapping> callback) {
                onMenuCompleteCallback = callback;
            }

            private void MoveOtherIcons(ControllerIconSelectable currIcon, int invalidPosition, int moveAmount) {
                for (int n = 0; n < controllers.Count; ++n) {
                    if (controllers[n] != currIcon) {
                        var otherIcon = controllers[n];

                        otherIcon.DisablePosition(invalidPosition);
                        if (currIcon.GetPosition() == otherIcon.GetPosition()) {
                            otherIcon.MovePosition(moveAmount);
                        }
                    }
                }
            }

            private void EnablePosition(int validPosition) {
                for (int n = 0; n < controllers.Count; ++n) {
                    controllers[n].EnablePosition(validPosition);
                }
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

            private void CreateControllerIcon(GameDeviceMapping devMap) {
                ControllerIconSelectable currIcon;
                if (deviceManager.IsKeyboard(devMap)) {
                    currIcon = Instantiate(keyboardIconPrefab,
                        controllerPointsStart.position + 0 * offset,
                        Quaternion.identity,
                        transform)
                    .GetComponent<ControllerIconSelectable>();
                }
                else {
                    currIcon = Instantiate(controllerIconPrefab,
                        controllerPointsStart.position + devMap.displayNumber * offset,
                        Quaternion.identity,
                        transform)
                    .GetComponent<ControllerIconSelectable>();
                    currIcon.SetText("Controller " + devMap.displayNumber);
                }

                if (active) {
                    currIcon.TriggerEvent("activate");
                }
                else {
                    currIcon.TriggerEvent("deactivate");
                }
                devToIconMap.Add(devMap, currIcon);
                controllers.Add(currIcon);
            }
        }
    }
}