using System;
using UnityEngine;

using ResonantSpark.Input.Combinations;
using ResonantSpark.Input;
using ResonantSpark.Character;
using ResonantSpark.Utility;

namespace ResonantSpark {
    namespace CharacterStates {
        public class Crouch : CharacterBaseState {

            private bool crouchDodge = false;
            private Input.FightingGameInputCodeDir dirPress = Input.FightingGameInputCodeDir.None;

            public new void Awake() {
                base.Awake();
                states.Register(this, "crouch");

                RegisterInputCallbacks()
                    .On<DoubleTap>(OnDoubleTap)
                    .On<ButtonsCurrent>(OnButtonsCurrent)
                    .On<ButtonPress>(OnButtonPress)
                    .On<Button2Press>(OnButton2Press)
                    .On<DirectionCurrent>(OnDirectionCurrent);

                RegisterEnterCallbacks()
                    .On<DirectionPress>(GivenDirectionPress)
                    .On<DirectionCurrent>(GivenDirectionCurrent)
                    .On<NeutralReturn>(GivenNeutralReturn)
                    .On<Empty>(GivenNothing);
            }

            public override void Enter(int frameIndex, IState previousState) {
                dirPress = FightingGameInputCodeDir.Neutral;

                GivenInput(fgChar.GivenCombinations());

                //fgChar.SetLocalMoveDirection(0.0f, 0.0f);
                fgChar.Play("idle_crouch");
            }

            public override void Execute(int frameIndex) {
                FindInput(fgChar.GetFoundCombinations());
            }

            public override void Exit(int frameIndex) {
                // do nothing
            }

            public override GroundRelation GetGroundRelation() {
                return GroundRelation.GROUNDED;
            }

            private void OnDirectionPress(Action stop, Combination combo) {
                var dirPress = combo as DirectionPress;

                if (GameInputUtil.Up(fgChar.MapAbsoluteToRelative(dirPress.direction))) {
                    fgChar.UseCombination(dirPress);
                    stop();
                    changeState(states.Get("jump"));
                }
            }

            private void OnDirectionCurrent(Action stop, Combination combo) {
                var curr = (DirectionCurrent) combo;
                dirPress = fgChar.MapAbsoluteToRelative(curr.direction);

                if (!GameInputUtil.Down(dirPress)) {
                    changeState(states.Get("stand"));
                }
            }

            private void OnButtonPress(Action stop, Combination combo) {
                var buttonPress = (ButtonPress)combo;

                if (buttonPress.button0 != FightingGameInputCodeBut.D) {
                    fgChar.ChooseAttack(this, null, buttonPress.button0, this.dirPress);
                    stop();
                }
            }

            private void OnButton2Press(Action stop, Combination combo) {
                var but2Press = (Button2Press)combo;
                Debug.Log("Crouch received 2 button press");
            }

            private void OnDoubleTap(Action stop, Combination combo) {
                var doubleTap = (DoubleTap) combo;
                FightingGameInputCodeDir relDir = fgChar.MapAbsoluteToRelative(doubleTap.direction);

                if (relDir == FightingGameInputCodeDir.Forward ||
                    relDir == FightingGameInputCodeDir.Back) {
                    fgChar.UseCombination(doubleTap);
                    stop();
                    changeState(states.Get("backDash"));
                }
            }

            private void OnButtonsCurrent(Action stop, Combination combo) {
                ButtonsCurrent curr = (ButtonsCurrent)combo;

                this.crouchDodge = !curr.butD;
            }

            private void GivenDirectionPress(Action stop, Combination combo) {
                dirPress = fgChar.MapAbsoluteToRelative(((DirectionPress)combo).direction);
            }

            private void GivenDirectionCurrent(Action stop, Combination combo) {
                dirPress = fgChar.MapAbsoluteToRelative(((DirectionCurrent)combo).direction);
            }

            private void GivenNeutralReturn(Action stop, Combination combo) {
                dirPress = fgChar.MapAbsoluteToRelative(Input.FightingGameAbsInputCodeDir.Neutral);
            }

            private void GivenNothing(Action stop, Combination combo) {
                dirPress = fgChar.MapAbsoluteToRelative(Input.FightingGameAbsInputCodeDir.Neutral);
            }
        }
    }
}