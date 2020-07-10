using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Service;

namespace ResonantSpark {
    namespace CharacterStates {
        public abstract class HitStun : CharacterBaseState {

            public new void Awake() {
                base.Awake();
            }

            protected void OnComplete() {
                // TODO: Figure out what to do.
                Debug.Log("Hit Stun ending");
            }

            public override ComboState GetComboState() {
                return ComboState.InCombo;
            }

            public override CharacterVulnerability GetCharacterVulnerability() {
                if (fgChar.GetNumGrabsInCombo() < 1) {
                    return new CharacterVulnerability {
                        strikable = true,
                        throwable = true,
                    };
                }
                else {
                    return new CharacterVulnerability {
                        strikable = true,
                        throwable = false,
                    };
                }
            }
        }
    }
}