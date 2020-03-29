using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace CharacterStates {
        public class StandAnimation : StateHelper {

            private Utility.Tracker currTracker;
            private Utility.Tracker standTracker;

            public new void Awake() {
                base.Awake();

                standTracker = new Utility.Tracker(10, FromCrouchComplete);
            }

            public void SetInfo(Vector3 currVel, Vector3 currInput) {

            }

            public void IncrementTracker() {
                currTracker?.Increment();
            }

            public void FromCrouch() {
                fgChar.Play("crouch_to_idle");
                standTracker.Track();
                currTracker = standTracker;
            }

            private void FromCrouchComplete() {
                currTracker = null;
                fgChar.Play("idle");
            }
        }
    }
}
