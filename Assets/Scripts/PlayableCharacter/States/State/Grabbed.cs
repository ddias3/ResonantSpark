using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Input.Combinations;
using ResonantSpark.Character;
using ResonantSpark.Utility;
using ResonantSpark.Service;
using ResonantSpark.Gameplay;

namespace ResonantSpark {
    namespace CharacterStates {
        public class Grabbed : CharacterBaseState {

            public int grabbedLength;

            private bool moveToPosition;
            private Vector3 finalGrabbedPosition;
            private Utility.AttackTracker tracker;

            public new void Awake() {
                base.Awake();
                states.Register(this, "grabbed");

                moveToPosition = false;
                tracker = new Utility.AttackTracker();

                RegisterInputCallbacks()
                    .On<Button2Press>(OnButton2Press);
            }

            public override void Enter(int frameIndex, MultipassBaseState previousState) {
                fgChar.__debugSetStateText("Grabbed", new Color(1.0f, 0.0f, 1.0f));

                fgChar.Play("grabbed");

                tracker.Track(frameIndex);
            }

            public override void ExecutePass0(int frameIndex) {
                FindInput(fgChar.GetFoundCombinations());

                if (moveToPosition) {
                    Debug.Log("Move grabbed char");
                        // TODO: Move the character to the desired location
                    //WalkCharacter(localVelocity, localInput);
                    //TurnCharacter(localInput);
                }
            }

            public override void ExecutePass1(int frameIndex) {
                fgChar.CalculateFinalVelocity();
            }

            public override void LateExecute(int frameIndex) {
                if (tracker.frameCount > grabbedLength) {
                    if (fgChar.hitStun <= 0.0f) {
                        changeState(states.Get("stand"));
                    }
                    else {
                        changeState(states.Get("hitStunStand"));
                    }
                }

                fgChar.IncrementHitStun();
                tracker.Increment();
            }

            public override void Exit(int frameIndex) {
                // do nothing
            }

            public override GroundRelation GetGroundRelation() {
                return GroundRelation.GROUNDED;
            }

            public override ComboState GetComboState() {
                return ComboState.InCombo;
            }

            public override CharacterVulnerability GetCharacterVulnerability() {
                return new CharacterVulnerability {
                    strikable = false,
                    throwable = false,
                };
            }

            public override void ReceiveHit(bool launch) {
                fgChar.Play("hurt_stand_0");
            }

            public override void ReceiveBlocked(bool forceCrouch) {
                throw new InvalidOperationException("Grabbed is somehow being told to block");
            }

            public override void ReceiveGrabbed() {
                throw new InvalidOperationException("Grabbed is somehow being grabbed again");
            }

            public override bool CheckBlockSuccess(Hit hit) {
                return false;
            }

            public void SetFinalGrabbedPosition(Vector3 position) {
                this.finalGrabbedPosition = position;
            }

            public void SetMoveToPosition(bool moveToPosition) {
                this.moveToPosition = moveToPosition;
            }

            private void OnButton2Press(Action stop, Combination combo) {
                var but2Press = (Button2Press)combo;
                if ((but2Press.button0 == Input.FightingGameInputCodeBut.A && but2Press.button1 == Input.FightingGameInputCodeBut.D) ||
                    (but2Press.button1 == Input.FightingGameInputCodeBut.A && but2Press.button0 == Input.FightingGameInputCodeBut.D)) {
                    fgChar.BreakGrab();
                }
                stop();
            }
        }
    }
}