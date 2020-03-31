using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Utility;

namespace ResonantSpark {
    namespace Gameplay {
        public class CharacterMovementAnimation {

            private FightingGameCharacter fgChar;
            private readonly int animationLength = 10;
            private readonly Vector3 movementDeadZone = new Vector3(0.1f, 0.0f, 0.1f);

            private Tracker currTracker;

            private Tracker standTracker;
            private Tracker crouchTracker;

            public CharacterMovementAnimation(FightingGameCharacter fgChar) {
                this.fgChar = fgChar;

                standTracker = new Tracker(animationLength, Stand);
                crouchTracker = new Tracker(animationLength, Crouch);
            }

            public void Increment() {
                currTracker?.Increment();
            }

            public void WalkVelocity(Vector3 walkVelocity) {
                if (currTracker == null) {
                    if (walkVelocity.sqrMagnitude < movementDeadZone.sqrMagnitude) {
                        fgChar.SetLocalWalkParameters(0.0f, 0.0f);
                    }
                    else {
                        fgChar.SetLocalWalkParameters(walkVelocity.x, walkVelocity.z);
                    }
                    fgChar.PlayIfOther("stand");
                }
            }

            public void FromCrouch() {
                if (currTracker == null) {
                    standTracker.Track();
                }
                else {
                    standTracker.Track(animationLength - currTracker.frameCount);
                }
                fgChar.Play("crouch_to_idle", ((float) standTracker.frameCount) / animationLength);
                currTracker = standTracker;
            }

            public void Stand() {
                currTracker = null;
                fgChar.Play("idle");
            }

            public void FromStand() {
                if (currTracker == null) {
                    crouchTracker.Track();
                }
                else {
                    crouchTracker.Track(animationLength - currTracker.frameCount);
                }

                fgChar.Play("idle_to_crouch", ((float) crouchTracker.frameCount) / animationLength);
                currTracker = crouchTracker;
            }

            public void Crouch() {
                currTracker = null;
                fgChar.Play("crouch_idle");
            }
        }
    }
}