using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Service;
using ResonantSpark.Input;
using ResonantSpark.Gameplay;

namespace ResonantSpark {
    namespace CharacterStates {
        public abstract class HitStun : CharacterBaseState {

            public new void Awake() {
                base.Awake();
            }

            protected void OnComplete() {
                fgChar.HitStunEnd();
            }

            public override void Exit(int frameIndex) {
                // do nothing
            }

            public override ComboState GetComboState() {
                return ComboState.InCombo;
            }

            public override CharacterVulnerability GetCharacterVulnerability() {
                if (fgChar.GetNumGrabsInCombo() < 1) {
                    return new CharacterVulnerability {
                        strikable = true,
                            // TODO: Make throws in a combo a thing. but for this version, it won't be a thing.
                        throwable = false,
                        //throwable = true,
                    };
                }
                else {
                    return new CharacterVulnerability {
                        strikable = true,
                        throwable = false,
                    };
                }
            }

            public override void ReceiveBlocked(bool forceCrouch) {
                throw new InvalidOperationException("HitStun shouldn't be able to block");
            }

            public override void ReceiveGrabbed() {
                // TODO: Make throws in a combo a thing. but for this version, it won't be a thing.
                //if (GetGroundRelation() == Character.GroundRelation.GROUNDED) {
                //    ...
                //}
                throw new InvalidOperationException("HitStun shouldn't be able to be grabbed");
            }

            public override bool CheckBlockSuccess(Hit hit) {
                return false;
            }
        }
    }
}