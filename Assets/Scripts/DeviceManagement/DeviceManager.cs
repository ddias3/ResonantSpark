using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

namespace ResonantSpark {
    namespace DeviceManagement {
        public class DeviceManager {
            private static DeviceManager devMan = null;
            public static bool Exists() { return devMan != null; }
            public static DeviceManager Get() {
                if (devMan == null) {
                    devMan = new DeviceManager();
                }
                return devMan;
            }

            private GameDeviceMapping keyboard;

            private List<GameDeviceMapping> devicesActive;
            private List<GameDeviceMapping> devicesInactive;

            private List<Action<GameDeviceMapping>> onDeviceAdded;
            private List<Action<GameDeviceMapping>> onDeviceRemoved;

            private DeviceManager() {
                onDeviceAdded = new List<Action<GameDeviceMapping>>();
                onDeviceRemoved = new List<Action<GameDeviceMapping>>();

                devicesActive = new List<GameDeviceMapping>();
                devicesInactive = new List<GameDeviceMapping>();

                keyboard = new GameDeviceMapping(Keyboard.current);
                devicesActive.Add(keyboard);
                for (int n = 0; n < Gamepad.all.Count; ++n) {
                    Gamepad gamepad = Gamepad.all[n];
                    GameDeviceMapping mapping = new GameDeviceMapping(gamepad.device);

                    mapping.displayNumber = n + 1;
                    devicesActive.Add(mapping);
                }

                InputSystem.onDeviceChange += OnDeviceChange;
            }

            public bool IsKeyboard(GameDeviceMapping devMap) {
                return devMap == keyboard;
            }

            public void AddOnDeviceAdded(Action<GameDeviceMapping> callback) {
                onDeviceAdded.Add(callback);
            }

            public void AddOnDeviceRemoved(Action<GameDeviceMapping> callback) {
                onDeviceRemoved.Add(callback);
            }

            public void RemoveOnDeviceAdded(Action<GameDeviceMapping> callback) {
                onDeviceAdded.Remove(callback);
            }

            public void RemoveOnDeviceRemoved(Action<GameDeviceMapping> callback) {
                onDeviceRemoved.Remove(callback);
            }

            public GameDeviceMapping GetKeyboard() {
                return keyboard;
            }

            public void ForEach(Action<GameDeviceMapping, int> callback) {
                for (int n = 0; n < devicesActive.Count; ++n) {
                    if (devicesActive[n] != null) {
                        callback(devicesActive[n], n);
                    }
                }
            }

            private void OnDeviceChange(InputDevice device, InputDeviceChange change) {
                switch (change) {
                    case InputDeviceChange.Added:
                    case InputDeviceChange.Reconnected:
                        AddOrReconnectController(device);
                        break;
                    case InputDeviceChange.Disconnected:
                    case InputDeviceChange.Removed:
                        RemoveOrDisconnectController(device);
                        break;
                    default:
                        Debug.LogWarning("InputDeviceChange that was unexpected");
                        break;
                }
            }

            private void AddOrReconnectController(InputDevice device) {
                GameDeviceMapping devMap = null;
                int displayNumber = -1;
                for (int n = 0; n < devicesInactive.Count; ++n) {
                    if (devicesInactive[n] != null && devicesInactive[n].CompareDevice(device)) {
                        devMap = devicesInactive[n];

                        devicesInactive.Remove(devMap);

                        if (devicesActive[devMap.displayNumber] == null) {
                            devicesActive[devMap.displayNumber] = devMap;
                        }
                        else {
                            displayNumber = FindShortestUnusedSlot();
                            if (displayNumber == -1) {
                                devMap.displayNumber = devicesActive.Count;
                                devicesActive.Add(devMap);
                            }
                            else {
                                devMap.displayNumber = displayNumber;
                                devicesActive[displayNumber] = devMap;
                            }
                        }

                        CallOnDeviceAddedCallbacks(devMap);
                    }
                }
                for (int n = 0; n < devicesActive.Count; ++n) {
                    if (devicesActive[n] != null && devicesActive[n].CompareDevice(device)) {
                        devMap = devicesActive[n];
                    }
                }
                if (devMap == null) {
                    CreateDevMap(device);
                }
            }

            private void RemoveOrDisconnectController(InputDevice device) {
                for (int n = 0; n < devicesActive.Count; ++n) {
                    if (devicesActive[n] != null && devicesActive[n].CompareDevice(device)) {
                        CallOnDeviceRemovedCallbacks(devicesActive[n]);

                        devicesInactive.Add(devicesActive[n]);
                        devicesActive[n] = null;
                    }
                }
            }

            private void CreateDevMap(InputDevice device) {
                GameDeviceMapping devMap = new GameDeviceMapping(device);
                int displayNumber = FindShortestUnusedSlot();
                if (displayNumber == -1) {
                    devMap.displayNumber = devicesActive.Count;
                    devicesActive.Add(devMap);
                }
                else {
                    devMap.displayNumber = displayNumber;
                    devicesActive[displayNumber] = devMap;
                }
                CallOnDeviceAddedCallbacks(devMap);
            }

            private void CallOnDeviceAddedCallbacks(GameDeviceMapping devMap) {
                foreach (Action<GameDeviceMapping> callback in onDeviceAdded) {
                    callback(devMap);
                }
            }

            private void CallOnDeviceRemovedCallbacks(GameDeviceMapping devMap) {
                foreach (Action<GameDeviceMapping> callback in onDeviceRemoved) {
                    callback(devMap);
                }
            }

            private int FindShortestUnusedSlot() {
                int number = -1;
                for (int n = 1; n < devicesActive.Count; ++n) {
                    if (devicesActive[n] == null) {
                        number = n;
                        break;
                    }
                }
                return number;
            }
        }
    }
}