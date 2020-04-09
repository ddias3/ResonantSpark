using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace CharacterStates {
        public class CrouchStandTransition : StateHelper {

            public new void Awake() {
                base.Awake();
            }

            public void FromStand() {
                fgChar.Play("idle_to_crouch");
            }

            public void FromCrouch() {
                fgChar.Play("crouch_to_idle");
            }
        }
    }
}