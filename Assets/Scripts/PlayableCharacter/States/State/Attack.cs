using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Input.Combinations;
using ResonantSpark.Input;
using ResonantSpark.Character;
using ResonantSpark.Utility;

namespace ResonantSpark {
    namespace CharacterStates {
        public class Attack : CharacterBaseState {

            private Action onCompleteAttack;

            private InputNotation notation;

            private FightingGameInputCodeBut button;
            private FightingGameInputCodeDir direction;

            private FightingGameInputCodeDir currDir;

            private CharacterProperties.Attack activeAttack;
            private CharacterProperties.Attack queuedUpAttack;

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
                fgChar.__debugSetStateText("Back Dash", Color.red);
                    // Start OnEnter with this
                GivenInput(fgChar.GivenCombinations());

                activeAttack = queuedUpAttack;
                queuedUpAttack = null;

                activeAttack.StartPerformable(frameIndex);
                activeAttack.SetOnCompleteCallback(onCompleteAttack);
            }

            public override void Execute(int frameIndex) {
                FindInput(fgChar.GetFoundCombinations());

                activeAttack?.RunFrame();
            }

            public override void Exit(int frameIndex) {
                activeAttack = null;
            }

            public void OnCompleteAttack() {
                //TODO: Choose a new attack, which is also possible.
                if (GameInputUtil.Down(currDir)) {
                    changeState(states.Get("crouch"));
                }
                else {
                    changeState(states.Get("stand"));
                }

                activeAttack = null;
            }

            public void SetActiveAttack(CharacterProperties.Attack atk) {
                queuedUpAttack = atk;
                changeState(this);
            }

            public override GroundRelation GetGroundRelation() {
                return GroundRelation.GROUNDED;
            }

            private void OnDirectionPress(Action stop, Combination combo) {
                direction = fgChar.MapAbsoluteToRelative(((DirectionPress) combo).direction);
            }

            private void OnButtonPress(Action stop, Combination combo) {
                button = ((ButtonPress) combo).button0;

                if (activeAttack == null || activeAttack.ChainCancellable()) {
                        // TODO: I need to change the input buffer to look further into the future than the input delay for a direction press.
                    fgChar.ChooseAttack(this, activeAttack, button, direction);
                }
            }

            private void GivenButtonPress(Action stop, Combination combo) {
                button = ((ButtonPress) combo).button0;
            }

            private void GivenDirectionPress(Action stop, Combination combo) {
                direction = fgChar.MapAbsoluteToRelative(((DirectionPress) combo).direction);
            }

            private void OnDirectionCurrent(Action stop, Combination combo) {
                currDir = fgChar.MapAbsoluteToRelative(((DirectionCurrent) combo).direction);
            }
        }
    }
}