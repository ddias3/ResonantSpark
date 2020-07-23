using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using ResonantSpark.MenuStates;
using ResonantSpark.DeviceManagement;

namespace ResonantSpark {
    namespace Menu {
        public class MenuInputManager : MonoBehaviour {
            public Vector2 deadZone = new Vector2(0.25f, 0.25f);
            public float cardinalOverlap = 0.4f;
            [Tooltip("Buttons will only ever return 0 or 1, but some continuous inputs can be used as buttons, like triggers")]
            public float buttonDeadZone = 0.5f;

            private MenuBaseState activeState;
            private DeviceManager deviceManager;

            private BasicActions allDevices;
            private Dictionary<GameDeviceMapping, BasicActions> individualDevices;
            private Dictionary<GameDeviceMapping,
                (Action<InputAction.CallbackContext> onNavigate, Action<InputAction.CallbackContext> onSubmit, Action<InputAction.CallbackContext> onCancel, Action<InputAction.CallbackContext> onPause)>
                    individualDevicesCallbacks;

            public void Awake() {
                deviceManager = DeviceManager.Get();

                deviceManager.AddOnDeviceAdded(OnDeviceAdded);
                deviceManager.AddOnDeviceRemoved(OnDeviceRemoved);

                allDevices = new BasicActions();
                individualDevices = new Dictionary<GameDeviceMapping, BasicActions>();
                individualDevicesCallbacks = new Dictionary<GameDeviceMapping, (Action<InputAction.CallbackContext> onNavigate, Action<InputAction.CallbackContext> onSubmit, Action<InputAction.CallbackContext> onCancel, Action<InputAction.CallbackContext> onPause)>();

                allDevices.Ui.Navigate.performed += OnNavigate;
                allDevices.Ui.Submit.performed += OnSubmit;
                allDevices.Ui.Cancel.performed += OnCancel;
                allDevices.Ui.Pause.performed += OnPause;

                deviceManager.ForEach((devMap, index) => {
                    BasicActions deviceActions = new BasicActions();
                    individualDevices.Add(devMap, deviceActions);
                    AddActionCallbacks(devMap, deviceActions);

                    deviceActions.devices = new[] { devMap.device };
                });
            }

            public void OnDestroy() {
                deviceManager.RemoveOnDeviceAdded(OnDeviceAdded);
                deviceManager.RemoveOnDeviceRemoved(OnDeviceRemoved);

                allDevices.Ui.Navigate.performed -= OnNavigate;
                allDevices.Ui.Submit.performed -= OnSubmit;
                allDevices.Ui.Cancel.performed -= OnCancel;
                allDevices.Ui.Pause.performed -= OnPause;

                deviceManager.ForEach((devMap, index) => {
                    BasicActions deviceActions = individualDevices[devMap];
                    RemoveActionCallbacks(devMap, deviceActions);
                    individualDevices.Remove(devMap);
                });
            }

            public void OnEnable() {
                allDevices.Enable();
                foreach (KeyValuePair<GameDeviceMapping, BasicActions> kvp in individualDevices) {
                    var deviceActions = kvp.Value;
                    deviceActions.Enable();
                }
            }

            public void OnDisable() {
                allDevices.Disable();
                foreach (KeyValuePair<GameDeviceMapping, BasicActions> kvp in individualDevices) {
                    var deviceActions = kvp.Value;
                    deviceActions.Disable();
                }
            }

            public void SetActiveState(MenuBaseState state) {
                activeState = state;
            }

            public void OnDeviceAdded(GameDeviceMapping devMapping) {
                BasicActions deviceActions = new BasicActions();
                deviceActions.devices = new[] { devMapping.device };
                if (deviceManager.IsKeyboard(devMapping)) {
                    deviceActions.bindingMask = InputBinding.MaskByGroup("Keyboard");
                }
                else {
                    deviceActions.bindingMask = InputBinding.MaskByGroup("Gamepad");
                }
                deviceActions.Enable();

                individualDevices.Add(devMapping, deviceActions);
                AddActionCallbacks(devMapping, deviceActions);
            }

            public void OnDeviceRemoved(GameDeviceMapping devMapping) {
                BasicActions deviceActions = individualDevices[devMapping];
                deviceActions.Disable();
                RemoveActionCallbacks(devMapping, deviceActions);
                individualDevices.Remove(devMapping);
            }

            private Action<InputAction.CallbackContext> BindCallback(GameDeviceMapping devMap, Action<GameDeviceMapping, InputAction.CallbackContext> callback) {
                return (context) => {
                    callback(devMap, context);
                };
            }

            private void AddActionCallbacks(GameDeviceMapping devMap, BasicActions inputActions) {
                (Action<InputAction.CallbackContext> onNavigate, Action<InputAction.CallbackContext> onSubmit, Action<InputAction.CallbackContext> onCancel, Action<InputAction.CallbackContext> onPause) callbacks =
                    (BindCallback(devMap, OnNavigate), BindCallback(devMap, OnSubmit), BindCallback(devMap, OnCancel), BindCallback(devMap, OnPause));
                individualDevicesCallbacks.Add(devMap, callbacks);
                inputActions.Ui.Navigate.performed += callbacks.onNavigate;
                inputActions.Ui.Submit.performed += callbacks.onSubmit;
                inputActions.Ui.Cancel.performed += callbacks.onCancel;
                inputActions.Ui.Pause.performed += callbacks.onPause;
            }

            private void RemoveActionCallbacks(GameDeviceMapping devMap, BasicActions inputActions) {
                (Action<InputAction.CallbackContext> onNavigate, Action<InputAction.CallbackContext> onSubmit, Action<InputAction.CallbackContext> onCancel, Action<InputAction.CallbackContext> onPause) callbacks =
                    individualDevicesCallbacks[devMap];
                inputActions.Ui.Navigate.performed -= callbacks.onNavigate;
                inputActions.Ui.Submit.performed -= callbacks.onSubmit;
                inputActions.Ui.Cancel.performed -= callbacks.onCancel;
                inputActions.Ui.Pause.performed -= callbacks.onPause;

                individualDevicesCallbacks.Remove(devMap);
            }

            private void OnNavigate(InputAction.CallbackContext context) {
                float horizontalInput = 0;
                float verticalInput = 0;
                Vector2 vec2 = context.ReadValue<Vector2>();
                if (vec2.sqrMagnitude > deadZone.sqrMagnitude) {
                    if (Mathf.Abs(vec2.x) > cardinalOverlap) horizontalInput = Mathf.Sign(vec2.x);
                    else horizontalInput = 0f;
                    if (Mathf.Abs(vec2.y) > cardinalOverlap) verticalInput = Mathf.Sign(vec2.y);
                    else verticalInput = 0f;

                    // TODO: Also create version with angles for determining input.
                    //   But, a cardinalOverlap of 0.4f essentially equals an equal 45d for all 8 direction inputs.

                    if (verticalInput > buttonDeadZone) {
                        activeState.TriggerEvent("up");
                    }
                    else if (verticalInput < -buttonDeadZone) {
                        activeState.TriggerEvent("down");
                    }

                    if (horizontalInput > buttonDeadZone) {
                        activeState.TriggerEvent("right");
                    }
                    else if (horizontalInput < -buttonDeadZone) {
                        activeState.TriggerEvent("left");
                    }
                }
            }

            private void OnNavigate(GameDeviceMapping devMap, InputAction.CallbackContext context) {
                float horizontalInput = 0;
                float verticalInput = 0;
                Vector2 vec2 = context.ReadValue<Vector2>();
                if (vec2.sqrMagnitude > deadZone.sqrMagnitude) {
                    if (Mathf.Abs(vec2.x) > cardinalOverlap) horizontalInput = Mathf.Sign(vec2.x);
                    else horizontalInput = 0f;
                    if (Mathf.Abs(vec2.y) > cardinalOverlap) verticalInput = Mathf.Sign(vec2.y);
                    else verticalInput = 0f;

                    // TODO: Also create version with angles for determining input.
                    //   But, a cardinalOverlap of 0.4f essentially equals an equal 45d for all 8 direction inputs.

                    if (verticalInput > buttonDeadZone) {
                        activeState.TriggerEvent("up", devMap);
                    }
                    else if (verticalInput < -buttonDeadZone) {
                        activeState.TriggerEvent("down", devMap);
                    }

                    if (horizontalInput > buttonDeadZone) {
                        activeState.TriggerEvent("right", devMap);
                    }
                    else if (horizontalInput < -buttonDeadZone) {
                        activeState.TriggerEvent("left", devMap);
                    }
                }
            }

            private void OnSubmit(InputAction.CallbackContext context) {
                activeState.TriggerEvent("submit");
            }

            private void OnSubmit(GameDeviceMapping devMap, InputAction.CallbackContext context) {
                activeState.TriggerEvent("submit", devMap);
            }

            private void OnCancel(InputAction.CallbackContext context) {
                activeState.TriggerEvent("cancel");
            }

            private void OnCancel(GameDeviceMapping devMap, InputAction.CallbackContext context) {
                activeState.TriggerEvent("cancel", devMap);
            }

            private void OnPause(InputAction.CallbackContext context) {
                activeState.TriggerEvent("pause");
            }

            private void OnPause(GameDeviceMapping devMap, InputAction.CallbackContext context) {
                activeState.TriggerEvent("pause", devMap);
            }
        }
    }
}
