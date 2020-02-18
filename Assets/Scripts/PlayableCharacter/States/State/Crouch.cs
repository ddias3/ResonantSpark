using System;
using UnityEngine;

using ResonantSpark.Input.Combinations;
using ResonantSpark.Input;
using ResonantSpark.Character;

namespace ResonantSpark {
    namespace CharacterStates {
        public class Crouch : CharacterBaseState {

            public new void Awake() {
                base.Awake();
                states.Register(this, "crouch");

                RegisterInputCallbacks()
                    .On<DirectionCurrent>(OnDirectionCurrent)
                    //.On<NeutralReturn>(OnNeutralReturn)
                    .On<ButtonPress>(OnButtonPress)
                    .On<Button2Press>(OnButton2Press)
                    .On<DirectionPress>(OnDirectionPress);
                    //.On<DirectionPlusButton>(OnDirectionPlusButton)
                    //.On<DoubleTap>(OnDoubleTap)
                    //.On<QuarterCircle>(OnQuarterCircle)
                    //.On<QuarterCircleButtonPress>(OnQuarterCircleButtonPress);
            }

            public override void Enter(int frameIndex, IState previousState) {
                    // Start OnEnter with this
                GivenInput(fgChar.GivenCombinations());

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
                switch (dirPress.direction) {
                    case FightingGameAbsInputCodeDir.UpLeft:
                    case FightingGameAbsInputCodeDir.Up:
                    case FightingGameAbsInputCodeDir.UpRight:
                        fgChar.UseCombination(dirPress);
                        stop();
                        changeState(states.Get("jump"));
                        break;
                }
            }

            private void OnButtonPress(Action stop, Combination combo) {
                var buttonPress = (ButtonPress) combo;

                if (buttonPress.button0 != FightingGameInputCodeBut.S) {
                    fgChar.UseCombination(combo);
                    // TODO: Create a way to also supply the DirectionCurrent.
                    //   something like this:
                    //      stop((dirCurrent) => { fgChar.UseCombination(dirCurrent); });
                    stop();
                    changeState(states.Get("attack"));
                }
            }

            private void OnButton2Press(Action stop, Combination combo) {
                var but2Press = (Button2Press) combo;
                Debug.Log("Crouch received 2 button press");
            }

            private void OnDirectionCurrent(Action stop, Combination combo) {
                var curr = (DirectionCurrent) combo;
                if (curr.direction != FightingGameAbsInputCodeDir.DownLeft &&
                    curr.direction != FightingGameAbsInputCodeDir.Down &&
                    curr.direction != FightingGameAbsInputCodeDir.DownRight) {
                    changeState(states.Get("stand"));
                }
            }

            //private void OnNeutralReturn(Action stop, Combination combo) {
            //    var curr = (NeutralReturn) combo;
            //    changeState(states.Get("stand"));
            //    stop();
            //}
        }
    }
}