﻿using System;
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

            private bool grabBreakable;

            public new void Awake() {
                base.Awake();
                states.Register(this, "grabbed");

                grabBreakable = false;

                RegisterInputCallbacks()
                    .On<Button2Press>(OnButton2Press);
            }

            public override void Enter(int frameIndex, MultipassBaseState previousState) {
                fgChar.__debugSetStateText("Grabbed", new Color(1.0f, 0.0f, 1.0f));

                fgChar.Play("grabbed");
            }

            public override void ExecutePass0(int frameIndex) {
                FindInput(fgChar.GetFoundCombinations());
            }

            public override void ExecutePass1(int frameIndex) {
                //fgChar.UpdateTarget();
                //fgChar.UpdateCharacterMovement();
                fgChar.CalculateFinalVelocity();
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

            public void SetGrabBreakable(bool grabBreakable) {
                this.grabBreakable = grabBreakable;
            }

            private void OnButton2Press(Action stop, Combination combo) {
                var but2Press = (Button2Press)combo;

                if (grabBreakable) {
                    if (but2Press.button0 == Input.FightingGameInputCodeBut.A && but2Press.button1 == Input.FightingGameInputCodeBut.D) {
                        // TODO: Break the throw.
                        Debug.Log("Break the throw");
                    }
                }
                stop();
            }
        }
    }
}