﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Input.Combinations;
using ResonantSpark.Character;
using ResonantSpark.Input;
using ResonantSpark.Utility;
using ResonantSpark.Service;
using ResonantSpark.Gameplay;

namespace ResonantSpark {
    namespace CharacterStates {
        public class Clash : CharacterBaseState {
            public int clashEnd = 20;
            public int clashAnimationLength = 30;

            private FightingGameInputCodeDir currDir;
            private string clashAnimation = "clash_vertical";

            private Utility.AttackTracker tracker;

            public new void Awake() {
                base.Awake();
                states.Register(this, "clash");

                RegisterInputCallbacks()
                    .On<Button2Press>(OnButton2Press)
                    .On<ButtonPress>(OnButtonPress)
                    .On<DirectionCurrent>(OnDirectionCurrent);

                tracker = new Utility.AttackTracker(clashAnimationLength);
            }

            public override void Enter(int frameIndex, MultipassBaseState previousState) {
                fgChar.__debugSetStateText("Clash", Color.black);

                currDir = FightingGameInputCodeDir.Neutral;
                fgChar.Play(clashAnimation);

                tracker.Track(frameIndex);
            }

            public override void ExecutePass0(int frameIndex) {
                FindInput(fgChar.GetFoundCombinations());

                if (tracker.frameCount > clashAnimationLength) {
                    changeState(states.Get("stand"));
                }

                fgChar.CalculateFinalVelocity();
                tracker.Increment();
            }

            public override void ExecutePass1(int frameIndex) {
                //fgChar.UpdateTarget();
                //fgChar.UpdateCharacterMovement();
                //fgChar.CalculateFinalVelocity();
                //fgChar.AnimationWalkVelocity();
            }

            public override void LateExecute(int frameIndex) {
                //fgChar.UpdateCharacterMovement();
                //fgChar.AnimationWalkVelocity();
            }

            public override void Exit(int frameIndex) {
                // do nothing
            }

            public override GroundRelation GetGroundRelation() {
                return GroundRelation.GROUNDED;
            }

            public override ComboState GetComboState() {
                return ComboState.None;
            }

            public override CharacterVulnerability GetCharacterVulnerability() {
                return new CharacterVulnerability {
                    strikable = true,
                    throwable = false,
                };
            }

            public override void BeHit(bool launch) {
                if (launch) {
                    changeState(states.Get("hitStunAirborne"));
                }
                else {
                    changeState(states.Get("hitStunStand"));
                }
            }

            public override void BeBlocked(bool forceCrouch) {
                throw new InvalidOperationException("Clash is somehow trying to block");
            }

            public override void BeGrabbed() {
                changeState(states.Get("grabbed"));
            }

            public override bool CheckBlockSuccess(Hit hit) {
                return false;
            }

            public void SetClashAnimation(string clashAnimation) {
                this.clashAnimation = clashAnimation;
            }

            private void OnDirectionCurrent(Action stop, Combination combo) {
                if (tracker.frameCount >= clashEnd) {
                    this.currDir = fgChar.MapAbsoluteToRelative(((DirectionCurrent) combo).direction);

                    if (this.currDir != FightingGameInputCodeDir.Neutral) {
                        if (Utility.GameInputUtil.Up(this.currDir)) {
                            changeState(states.Get("jump"));
                        }
                        else if (Utility.GameInputUtil.Down(this.currDir)) {
                            changeState(states.Get("crouch"));
                        }
                        else {
                            changeState(states.Get("stand"));
                        }
                    }
                }
            }

            private void OnButtonPress(Action stop, Combination combo) {
                if (tracker.frameCount >= clashEnd) {
                    var buttonPress = (ButtonPress)combo;
                    Debug.Log("Dodge received 1 button press");
                }
            }

            private void OnButton2Press(Action stop, Combination combo) {
                if (tracker.frameCount >= clashEnd) {
                    var but2Press = (Button2Press) combo;
                    Debug.Log("Dodge received 2 button press");
                }
            }
        }
    }
}