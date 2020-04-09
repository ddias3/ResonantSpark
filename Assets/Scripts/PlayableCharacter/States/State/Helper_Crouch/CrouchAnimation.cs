using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace CharacterStates {
        public class CrouchAnimation : StateHelper {

            private Utility.Tracker currTracker;
            private Utility.Tracker crouchTracker;

            public new void Awake() {
                base.Awake();

                crouchTracker = new Utility.Tracker(10, Crouch);
            }

            public void IncrementTracker() {
                currTracker?.Increment();
            }

            public void FromStand() {
                fgChar.Play("idle_to_crouch");
                crouchTracker.Track();
                currTracker = crouchTracker;
            }

            public void Crouch() {
                currTracker = null;
                fgChar.Play("crouch");
            }
        }
    }
}
