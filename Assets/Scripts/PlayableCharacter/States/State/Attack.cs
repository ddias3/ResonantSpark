using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Input.Combinations;
using ResonantSpark.Input;
using ResonantSpark.Character;

namespace ResonantSpark {
    namespace CharacterStates {
        public class Attack : BaseState {

            public CharacterData charData { get; set; }

            private Action onCompleteAttack;

            private InputNotation notation;

            public new void Awake() {
                base.Awake();
                states.Register(this, "attack");

                RegisterInputCallbacks()
                    .On<ButtonPress>(OnButtonPress)
                    .On<DirectionPress>(OnDirectionPress);

                RegisterEnterCallbacks()
                    .On<ButtonPress>(GivenButtonPress)
                    .On<DirectionPress>(GivenDirectionPress)
                    .On<DirectionCurrent>(GivenDirectionCurrent);

                this.onCompleteAttack = new Action(OnCompleteAttack);
            }

            public override void Enter(int frameIndex, IState previousState) {
                    // Start OnEnter with this
                GivenInput(fgChar.GivenCombinations());

                List<CharacterProperties.Attack> attack = charData.SelectAttacks(fgChar.GetOrientation(), fgChar.GetGroundRelation(), notation);

                if (attack.Count == 0) {
                    Debug.LogFormat(LogType.Exception, LogOption.None, this, "Missing attack given conditions: {0}, {1}, {2}", fgChar.GetOrientation(), fgChar.GetGroundRelation(), notation);
                    throw new InvalidOperationException("Missing attack given conditions.");
                }


                // FOR NOW ONLY, just do this
                attack[0].StartPerformable(frameIndex);
                attack[0].SetOnCompleteCallback(onCompleteAttack);
            }

            public override void Execute(int frameIndex) {
                FindInput(fgChar.GetFoundCombinations());
            }

            public override void Exit(int frameIndex) {
                // do nothing
            }

            public void OnCompleteAttack() {
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