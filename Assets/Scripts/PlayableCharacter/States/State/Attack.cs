using System;
using UnityEngine;

using ResonantSpark.Input.Combinations;
using ResonantSpark.Input;
using ResonantSpark.Character;

namespace ResonantSpark {
    namespace CharacterStates {
        public class Attack : BaseState {

            public AttackServer server;
            public AttackTracker tracker;

            private CharacterData character;

            private InputNotation notation;

            public new void Start() {
                base.Start();
                states.Register(this, "attack");

                RegisterInputCallbacks()
                    .On<ButtonPress>(OnButtonPress)
                    .On<DirectionPress>(OnDirectionPress);

                RegisterEnterCallbacks()
                    .On<ButtonPress>(GivenButtonPress)
                    .On<DirectionPress>(GivenDirectionPress)
                    .On<DirectionCurrent>(GivenDirectionCurrent);

                tracker.onCompleteAttackCallback = new Action(OnCompleteAttack);
            }

            public override void Enter(int frameIndex, IState previousState) {
                    // Start OnEnter with this
                GivenInput(fgChar.GivenCombinations());

                CharacterProperties.Attack attack = character.SelectAttack(fgChar.GetOrientation(), fgChar.GetGroundRelation(), notation);

                if (attack != null) {
                    tracker.TrackAttack(frameIndex, attack);
                }
                else {
                    Debug.LogFormat(LogType.Exception, LogOption.None, this, "Missing attack given conditions: {0}, {1}, {2}", fgChar.GetOrientation(), fgChar.GetGroundRelation(), notation);
                    throw new InvalidOperationException("Missing attack given conditions.");
                }

                //fgChar.SetLocalMoveDirection(0.0f, 0.0f);
                //fgChar.Play("5AA", 0, 0.0f);
            }

            public override void Execute(int frameIndex) {
                FindInput(fgChar.GetFoundCombinations());

                tracker.IncrementTracker();
            }

            public override void Exit(int frameIndex) {
                // do nothing
            }

            public void OnCompleteAttack() {
                tracker.ClearAttack();
                // TODO: Figure out rest of this code
            }

            private void OnDirectionPress(Action stop, Combination combo) {
                // TODO: figure out the stuff on attack
            }

            private void OnButtonPress(Action stop, Combination combo) {
                // TODO: figure out the stuff on button press
            }

            private void GivenButtonPress(Action stop, Combination combo) {
                //TODO: Select the proper input notation only after the correct direction for the character has been established.
                InputNotation notation = InputNotation.None;
                switch (((ButtonPress) combo).button0) {
                    case FightingGameInputCodeBut.A:
                        notation = InputNotation._5A;
                        break;
                    case FightingGameInputCodeBut.B:
                        notation = InputNotation._5B;
                        break;
                }
                this.notation = notation;
            }

            private void GivenDirectionPress(Action stop, Combination combo) {

            }

            private void GivenDirectionCurrent(Action stop, Combination combo) {

            }
        }
    }
}