using System;
using System.Collections;
using System.Collections.Generic;

using ResonantSpark.Utility;

namespace ResonantSpark {
    namespace Gameplay {
        public class Hit : IEquatable<Hit> {
            private static int hitCounter = 0;
            public int id { get; private set; }

            private List<HitBox> hitBoxes;
            private Dictionary<string, Action<HitBox, HitInfo>> hitEventCallbacks;

            public Hit(Dictionary<string, Action<HitBox, HitInfo>> hitEventCallbacks) {
                this.id = Hit.hitCounter++;
                this.hitBoxes = new List<HitBox>();
                this.hitEventCallbacks = hitEventCallbacks;

                PopulateEventCallbacks();
            }

            public void AddHitBox(HitBox hitBox) {
                hitBoxes.Add(hitBox);
            }

            public void Active() {
                foreach (HitBox hitBox in hitBoxes) {
                    hitBox.Active();
                }
            }

            public bool Equals(Hit other) {
                return id == other.id;
            }

            public override int GetHashCode() {
                return id;
            }

            private void PopulateEventCallbacks() {
                List<string> eventNames = new List<string> {
                    "onHitFGChar",
                    "onEqualPriorityHitBox",
                    "onHitProjectile",
                };

                foreach (string eventName in eventNames) {
                    if (!hitEventCallbacks.ContainsKey(eventName)) {
                        hitEventCallbacks.Add(eventName, DefaultEventHandler(eventName));
                    }
                }
            }

            private Action<HitBox, HitInfo> DefaultEventHandler(string eventName) {
                return (HitBox hitBox, HitInfo hitInfo) => {
                    hitBox.InvokeEvent(eventName, hitInfo);
                };
            } 
        }
    }
}