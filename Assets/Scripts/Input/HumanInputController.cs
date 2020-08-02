using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using ResonantSpark.Gameplay;
using ResonantSpark.DeviceManagement;

namespace ResonantSpark {
    namespace Input {
        [RequireComponent(typeof(InputBuffer))]
        public class HumanInputController : InputController {
            // These are empirically good numbers
            public Vector2 deadZone = new Vector2(0.25f, 0.25f);
            public float cardinalOverlap = 0.4f;
            [Tooltip("Buttons will only ever return 0 or 1, but some continuous inputs can be used as buttons, like triggers")]
            public float buttonDeadZone = 0.5f;

            [SerializeField]
            private BasicActions inputActions;

            private GameDeviceMapping devMapping;

            private float horizontalInput = 0;
            private float verticalInput = 0;

            private int buttonInputCode = 0;

            public new void Awake() {
                base.Awake();

                inputBuffer = GetComponent<InputBuffer>();
                devMapping = null;

                inputActions = new BasicActions();

                inputActions.GamePlay.Move.performed += OnMove;
                inputActions.GamePlay.ButtonA.performed += OnButtonA;
                inputActions.GamePlay.ButtonB.performed += OnButtonB;
                inputActions.GamePlay.ButtonC.performed += OnButtonC;
                inputActions.GamePlay.ButtonD.performed += OnButtonD;
                inputActions.GamePlay.ButtonS.performed += OnButtonS;

                FrameEnforcer frame = GameObject.FindGameObjectWithTag("rspTime").GetComponent<FrameEnforcer>();
                frame.AddUpdate((int)FramePriority.InputBuffer, new Action<int>(FrameUpdate));
            }

            public void OnDestroy() {
                inputActions.GamePlay.Move.performed -= OnMove;
                inputActions.GamePlay.ButtonA.performed -= OnButtonA;
                inputActions.GamePlay.ButtonB.performed -= OnButtonB;
                inputActions.GamePlay.ButtonC.performed -= OnButtonC;
                inputActions.GamePlay.ButtonD.performed -= OnButtonD;
                inputActions.GamePlay.ButtonS.performed -= OnButtonS;
            }

            public void OnEnable() {
                inputActions.Enable();
            }

            public void OnDisable() {
                inputActions.Disable();
            }

            public void OnMove(InputAction.CallbackContext context) {
                Vector2 vec2 = context.ReadValue<Vector2>();
                if (vec2.sqrMagnitude > deadZone.sqrMagnitude) {
                    if (Mathf.Abs(vec2.x) > cardinalOverlap) horizontalInput = Mathf.Sign(vec2.x);
                    else horizontalInput = 0f;
                    if (Mathf.Abs(vec2.y) > cardinalOverlap) verticalInput = Mathf.Sign(vec2.y);
                    else verticalInput = 0f;

                    // TODO: Also create version with angles for determining input.
                    //   But, a cardinalOverlap of 0.4f essentially equals an equal 45d for all 8 direction inputs.
                }
                else {
                    horizontalInput = 0f;
                    verticalInput = 0f;
                }
            }

            public void OnButtonA(InputAction.CallbackContext context) {
                Debug.Log("ButtonA performed: " + context.ReadValue<float>() + ")");
                SetButtonBit(context.ReadValue<float>(), FightingGameInputCodeBut.A);
            }

            public void OnButtonB(InputAction.CallbackContext context) {
                Debug.Log("ButtonB performed: " + context.ReadValue<float>() + ")");
                SetButtonBit(context.ReadValue<float>(), FightingGameInputCodeBut.B);
            }

            public void OnButtonC(InputAction.CallbackContext context) {
                Debug.Log("ButtonC performed: " + context.ReadValue<float>() + ")");
                SetButtonBit(context.ReadValue<float>(), FightingGameInputCodeBut.C);
            }

            public void OnButtonD(InputAction.CallbackContext context) {
                Debug.Log("ButtonD performed: " + context.ReadValue<float>() + ")");
                SetButtonBit(context.ReadValue<float>(), FightingGameInputCodeBut.D);
            }

            public void OnButtonS(InputAction.CallbackContext context) {
                Debug.Log("ButtonS performed: " + context.ReadValue<float>() + ")");
                SetButtonBit(context.ReadValue<float>(), FightingGameInputCodeBut.S);
            }

            private void SetButtonBit(float buttonValue, FightingGameInputCodeBut bitMask) {
                buttonInputCode = buttonInputCode & ~((int)bitMask) | ((buttonValue > buttonDeadZone) ? (int)bitMask : 0);
            }

            public void SetDeviceMap(GameDeviceMapping devMapping) {
                this.devMapping = devMapping;
                if (devMapping.device != null) {
                    inputActions.devices = new[] { devMapping.device };
                }
            }

            public void FrameUpdate(int frameIndex) {
                FightingGameAbsInputCodeDir fgInputCodeDir = (FightingGameAbsInputCodeDir)((verticalInput + 1) * 3 + (horizontalInput + 1) + 1);
                inputBuffer.SetCurrentInputState(fgInputCodeDir, buttonInputCode);
            }
        }
    }
}