using System;
using System.Collections;
using System.Collections.Generic;
using ResonantSpark.Gameplay;
using UnityEngine;

using Rewired;

namespace ResonantSpark {
    namespace Input {
        [RequireComponent(typeof(InputBuffer))]
        public class HumanInputController : MonoBehaviour, ICharacterControlSystem {
                // These are empirically good numbers
            public Vector2 deadZone = new Vector2(0.25f, 0.25f);
            public float cardinalOverlap = 0.4f;
            [Tooltip("Buttons will only ever return 0 or 1, but some continuous inputs can be used as buttons, like triggers")]
            public float buttonDeadZone = 0.5f;

                // The Rewired player id of this controller
            public int playerId = 0;

            private Player player;

            private int controllerId = -1;

            private float horizontalInput = 0;
            private float verticalInput = 0;

            private int buttonInputCode = 0;

            private Vector2 moveVec2 = Vector2.zero;

            private InputBuffer inputBuffer;

            public void Awake() {
                inputBuffer = GetComponent<InputBuffer>();

                FrameEnforcer frame = GameObject.FindGameObjectWithTag("rspTime").GetComponent<FrameEnforcer>();
                frame.AddUpdate((int) FramePriority.InputBuffer, new Action<int>(FrameUpdate));

                player = ReInput.players.GetPlayer(playerId);

                player.AddInputEventDelegate(OnMoveHorizontal, UpdateLoopType.Update, "Move Horizontal");
                player.AddInputEventDelegate(OnMoveVertical,   UpdateLoopType.Update, "Move Vertical");

                player.AddInputEventDelegate(OnButtonA, UpdateLoopType.Update, "Button A");
                player.AddInputEventDelegate(OnButtonB, UpdateLoopType.Update, "Button B");
                player.AddInputEventDelegate(OnButtonC, UpdateLoopType.Update, "Button C");
                player.AddInputEventDelegate(OnButtonD, UpdateLoopType.Update, "Button D");
            }

            public void OnMoveHorizontal(InputActionEventData data) {
                moveVec2.x = data.GetAxis();
                FinalMoveValues();
            }

            public void OnMoveVertical(InputActionEventData data) {
                moveVec2.y = data.GetAxis();
                FinalMoveValues();
            }

            private void FinalMoveValues() {
                if (moveVec2.sqrMagnitude > deadZone.sqrMagnitude) {
                    if (Mathf.Abs(moveVec2.x) > cardinalOverlap) horizontalInput = Mathf.Sign(moveVec2.x);
                    else horizontalInput = 0f;
                    if (Mathf.Abs(moveVec2.y) > cardinalOverlap) verticalInput = Mathf.Sign(moveVec2.y);
                    else verticalInput = 0f;

                    // TODO: Also create version with angles for determining input.
                    //   But, a cardinalOverlap of 0.4f essentially equals an equal 45d for all 8 direction inputs.
                }
                else {
                    horizontalInput = 0f;
                    verticalInput = 0f;
                }
            }

            public void OnButtonA(InputActionEventData data) {
                if (data.GetButtonDown()) Debug.Log("ButtonA performed: Pressed Down");
                SetButtonBit(data.GetButton(), FightingGameInputCodeBut.A);
            }

            public void OnButtonB(InputActionEventData data) {
                if (data.GetButtonDown()) Debug.Log("ButtonB performed: Pressed Down");
                SetButtonBit(data.GetButton(), FightingGameInputCodeBut.B);
            }

            public void OnButtonC(InputActionEventData data) {
                if (data.GetButtonDown()) Debug.Log("ButtonC performed: Pressed Down");
                SetButtonBit(data.GetButton(), FightingGameInputCodeBut.C);
            }

            public void OnButtonD(InputActionEventData data) {
                if (data.GetButtonDown()) Debug.Log("ButtonD performed: Pressed Down");
                SetButtonBit(data.GetButton(), FightingGameInputCodeBut.D);
            }

            private void SetButtonBit(bool buttonValue, FightingGameInputCodeBut bitMask) {
                buttonInputCode = buttonInputCode & ~((int) bitMask) | (buttonValue ? (int) bitMask : 0);
            }

            public void SetControllerId(int controllerId) {
                this.controllerId = controllerId;
            }

            public void ConnectToCharacter(FightingGameCharacter fgChar) {
                fgChar.SetInputBuffer(inputBuffer);
            }

            public void FrameUpdate(int frameIndex) {
                FightingGameAbsInputCodeDir fgInputCodeDir = (FightingGameAbsInputCodeDir) ((verticalInput + 1) * 3 + (horizontalInput + 1) + 1);
                inputBuffer.SetCurrentInputState(fgInputCodeDir, buttonInputCode);
            }

            public void OnDestroy() {
                player.RemoveInputEventDelegate(OnMoveHorizontal);
                player.RemoveInputEventDelegate(OnMoveVertical);

                player.RemoveInputEventDelegate(OnButtonA);
                player.RemoveInputEventDelegate(OnButtonB);
                player.RemoveInputEventDelegate(OnButtonC);
                player.RemoveInputEventDelegate(OnButtonD);
            }
        }
    }
}