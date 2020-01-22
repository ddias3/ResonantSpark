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

            private FightingGameInputCodeBut button;
            private FightingGameInputCodeDir direction;

            private CharacterProperties.Attack activeAttack;

            public new void Awake() {
                base.Awake();
                states.Register(this, "attack");

                RegisterInputCallbacks()
                    .On<ButtonPress>(OnButtonPress)
                    .On<DirectionPress>(OnDirectionPress)
                    .On<DirectionCurrent>(OnDirectionCurrent);

                RegisterEnterCallbacks()
                    .On<ButtonPress>(GivenButtonPress)
                    .On<DirectionPress>(GivenDirectionPress)
                    .On<DirectionCurrent>(OnDirectionCurrent);

                this.onCompleteAttack = new Action(OnCompleteAttack);
            }

            public override void Enter(int frameIndex, IState previousState) {
                button = FightingGameInputCodeBut.None;
                direction = FightingGameInputCodeDir.None;

                    // Start OnEnter with this
                GivenInput(fgChar.GivenCombinations());

                InputNotation notation = SelectInputNotation();

                List<CharacterProperties.Attack> attack = charData.SelectAttacks(fgChar.GetOrientation(), fgChar.GetGroundRelation(), notation);

                if (attack.Count == 0) {
                    Debug.LogFormat(LogType.Exception, LogOption.None, this, "Missing attack given conditions: {0}, {1}, {2}", fgChar.GetOrientation(), fgChar.GetGroundRelation(), notation);
                    throw new InvalidOperationException("Missing attack given conditions.");
                }
                

                // FOR NOW ONLY, just do this
                activeAttack = attack[0];

                activeAttack.StartPerformable(frameIndex);
                activeAttack.SetOnCompleteCallback(onCompleteAttack);
            }

            public override void Execute(int frameIndex) {
                FindInput(fgChar.GetFoundCombinations());

                activeAttack?.RunFrame();
            }

            public override void Exit(int frameIndex) {
                // do nothing
            }

            private InputNotation SelectInputNotation() {
                InputNotation notation = InputNotation.None;

                switch (button) {
                    case FightingGameInputCodeBut.A:
                        switch (direction) {
                            case FightingGameInputCodeDir.Neutral:
                            case FightingGameInputCodeDir.None:
                                notation = InputNotation._5A;
                                break;
                            case FightingGameInputCodeDir.DownLeft:
                            case FightingGameInputCodeDir.Down:
                            case FightingGameInputCodeDir.DownRight:
                                notation = InputNotation._2A;
                                break;
                        }
                        break;
                    case FightingGameInputCodeBut.B:
                        switch (direction) {
                            case FightingGameInputCodeDir.Neutral:
                            case FightingGameInputCodeDir.None:
                                notation = InputNotation._5B;
                                break;
                            case FightingGameInputCodeDir.DownLeft:
                            case FightingGameInputCodeDir.Down:
                            case FightingGameInputCodeDir.DownRight:
                                notation = InputNotation._2B;
                                break;
                        }
                        break;
                    case FightingGameInputCodeBut.C:
                        switch (direction) {
                            case FightingGameInputCodeDir.Neutral:
                            case FightingGameInputCodeDir.None:
                                notation = InputNotation._5C;
                                break;
                            case FightingGameInputCodeDir.DownLeft:
                            case FightingGameInputCodeDir.Down:
                            case FightingGameInputCodeDir.DownRight:
                                notation = InputNotation._2C;
                                break;
                        }
                        break;
                    case FightingGameInputCodeBut.D:
                        switch (direction) {
                            case FightingGameInputCodeDir.Neutral:
                            case FightingGameInputCodeDir.None:
                                notation = InputNotation._5D;
                                break;
                            case FightingGameInputCodeDir.DownLeft:
                            case FightingGameInputCodeDir.Down:
                            case FightingGameInputCodeDir.DownRight:
                                notation = InputNotation._2D;
                                break;
                        }
                        break;
                }

                return notation;
            }

            public void OnCompleteAttack() {
                //TODO: Choose a new attack, which is also possible.
                switch (direction) {
                    case FightingGameInputCodeDir.DownLeft:
                    case FightingGameInputCodeDir.Down:
                    case FightingGameInputCodeDir.DownRight:
                        changeState(states.Get("crouch"));
                        break;
                    default:
                        changeState(states.Get("stand"));
                        break;
                }

                activeAttack = null;
            }

            private void OnDirectionPress(Action stop, Combination combo) {
                // TODO: figure out the stuff on attack
            }

            private void OnButtonPress(Action stop, Combination combo) {
                if (activeAttack == null) {
                    fgChar.UseCombination(combo);
                    stop();
                    changeState(states.Get("attack"));
                }

                if (activeAttack.ChainCancellable()) {

                }
            }

            private void GivenButtonPress(Action stop, Combination combo) {
                button = ((ButtonPress) combo).button0;
            }

            private void GivenDirectionPress(Action stop, Combination combo) {
                direction = ((DirectionPress) combo).direction;
            }

            private void OnDirectionCurrent(Action stop, Combination combo) {
                direction = ((DirectionCurrent) combo).direction;
            }
        }
    }
}