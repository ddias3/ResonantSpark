using System;
using UnityEngine;

using ResonantSpark.Input.Combinations;
using ResonantSpark.Input;

namespace ResonantSpark {
    namespace CharacterStates {
        public class Crouch : BaseState {

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

                fgChar.Play("idle_crouch", 0, 0.0f);
            }

            public override void Execute(int frameIndex) {
                FindInput(fgChar.GetFoundCombinations());
            }

            public override void Exit(int frameIndex) {
                // do nothing
            }

            private void OnDirectionPress(Action stop, Combination combo) {
                var dirPress = combo as DirectionPress;
                switch (dirPress.direction) {
                    //case FightingGameInputCodeDir.UpLeft:
                    case FightingGameInputCodeDir.Up:
                    //case FightingGameInputCodeDir.UpRight:
                        //fgChar.Use(dirPress);
                        stop();
                        changeState(states.Get("jump"));
                        break;
                    //case FightingGameInputCodeDir.DownLeft:
                    case FightingGameInputCodeDir.Down:
                    //case FightingGameInputCodeDir.DownRight:
                        //fgChar.Use(dirPress);
                        stop();
                        changeState(states.Get("crouch"));
                        break;
                }
            }

            private void OnButtonPress(Action stop, Combination combo) {
                var butPress = (ButtonPress) combo;
                switch (butPress.button0) {
                    case FightingGameInputCodeBut.A:
                        Debug.Log("Crouch pressed A");
                        break;
                    case FightingGameInputCodeBut.B:
                        Debug.Log("Crouch pressed B");
                        break;
                    case FightingGameInputCodeBut.C:
                        Debug.Log("Crouch pressed C");
                        break;
                    case FightingGameInputCodeBut.D:
                        Debug.Log("Crouch pressed D");
                        break;
                    case FightingGameInputCodeBut.S:
                        Debug.Log("Crouch pressed S");
                        break;
                }
            }

            private void OnButton2Press(Action stop, Combination combo) {
                var but2Press = (Button2Press) combo;
                Debug.Log("Crouch received 2 button press");
            }

            private void OnDirectionCurrent(Action stop, Combination combo) {
                var curr = (DirectionCurrent) combo;
                if (curr.direction != FightingGameInputCodeDir.DownLeft &&
                    curr.direction != FightingGameInputCodeDir.Down &&
                    curr.direction != FightingGameInputCodeDir.DownRight) {
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