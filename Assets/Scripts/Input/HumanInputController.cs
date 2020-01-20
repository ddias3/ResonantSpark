using System;
using System.Collections;
using System.Collections.Generic;
using ResonantSpark.Gameplay;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ResonantSpark {
    namespace Input {
        [RequireComponent(typeof(InputBuffer))]
        public class HumanInputController : MonoBehaviour, ICharacterControlSystem {
                // These are empirically good numbers
            public Vector2 deadZone = new Vector2(0.25f, 0.25f);
            public float cardinalOverlap = 0.4f;

            private InputBuffer inputBuffer;

            [SerializeField]
            private BasicActions inputActions;

            private int controllerId = -1;

            private float horizontalInput = 0;
            private float verticalInput = 0;

            private int buttonInputCode = 0;

            public void Awake() {
                inputBuffer = GetComponent<InputBuffer>();

                inputActions = new BasicActions();

                inputActions.GamePlay.Move.performed += OnMove;
                inputActions.GamePlay.ButtonA.performed += OnButtonA;
                inputActions.GamePlay.ButtonB.performed += OnButtonB;
                inputActions.GamePlay.ButtonC.performed += OnButtonC;
                inputActions.GamePlay.ButtonD.performed += OnButtonD;
                inputActions.GamePlay.ButtonS.performed += OnButtonS;

                FrameEnforcer frame = GameObject.FindGameObjectWithTag("rspTime").GetComponent<FrameEnforcer>();
                frame.AddUpdate((int) FramePriority.InputBuffer, new Action<int>(FrameUpdate));
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
                }
                else {
                    horizontalInput = 0f;
                    verticalInput = 0f;
                }
            }

            public void OnButtonA(InputAction.CallbackContext context) {
                Debug.Log("ButtonA performed: " + context.performed + ")");
                SetButtonBit(context.performed, FightingGameInputCodeBut.A);
            }

            public void OnButtonB(InputAction.CallbackContext context) {
                Debug.Log("ButtonB performed: " + context.performed + ")");
                SetButtonBit(context.performed, FightingGameInputCodeBut.B);
            }

            public void OnButtonC(InputAction.CallbackContext context) {
                Debug.Log("ButtonC performed: " + context.performed + ")");
                SetButtonBit(context.performed, FightingGameInputCodeBut.C);
            }

            public void OnButtonD(InputAction.CallbackContext context) {
                Debug.Log("ButtonD performed: " + context.performed + ")");
                SetButtonBit(context.performed, FightingGameInputCodeBut.D);
            }

            public void OnButtonS(InputAction.CallbackContext context) {
                Debug.Log("ButtonS performed: " + context.performed + ")");
                SetButtonBit(context.performed, FightingGameInputCodeBut.S);
            }

            private void SetButtonBit(bool setTrue, FightingGameInputCodeBut bitMask) {
                buttonInputCode = buttonInputCode & ~((int) bitMask) | (setTrue ? (int) bitMask : 0);
            }

            public void SetControllerId(int controllerId) {
                this.controllerId = controllerId;
            }

            public void ConnectToCharacter(FightingGameCharacter fgChar) {
                fgChar.SetInputBuffer(inputBuffer);
            }

            public void FrameUpdate(int frameIndex) {
                FightingGameInputCodeDir fgInputCodeDir = (FightingGameInputCodeDir) ((verticalInput + 1) * 3 + (horizontalInput + 1) + 1);
                inputBuffer.SetCurrentInputState(fgInputCodeDir, buttonInputCode);
            }
        }
    }
}