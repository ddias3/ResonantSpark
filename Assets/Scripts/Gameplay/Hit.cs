using System;
using System.Collections;
using System.Collections.Generic;
using ResonantSpark.Character;
using ResonantSpark.CharacterProperties;
using ResonantSpark.Utility;

namespace ResonantSpark {
    namespace Gameplay {
        public class Hit : IEquatable<Hit> {
            private static int hitCounter = 0;
            public int id { get; private set; }

            public AttackPriority priority { get; private set; }

            private List<HitBox> hitBoxes;
            private Dictionary<string, Action<List<HitBox>, HitInfo>> hitEventCallbacks;

            private List<(string, HitBox, HitInfo)> hitBoxQueue;

            public Hit(Dictionary<string, Action<List<HitBox>, HitInfo>> hitEventCallbacks) {
                this.id = Hit.hitCounter++;
                this.hitBoxes = new List<HitBox>();
                this.hitEventCallbacks = hitEventCallbacks;

                hitBoxQueue = new List<(string, HitBox, HitInfo)>();

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

            public void QueueUpEvent(string eventName, HitBox hitBox, HitInfo hitInfo) {
                if (!hitBoxes.Contains(hitBox)) {
                    throw new InvalidOperationException("Attempting to invoke a hitbox event to a hit it doesn't belongs to");
                }

                hitBoxQueue.Add((eventName, hitBox, hitInfo));
            }

            public void InvokeQueuedEvents() {
                if (hitBoxQueue.Count == 0) {
                    return;
                }

                // TODO: Handle the case where not every event is of the same type;

                List<HitBox> hitBoxes = new List<HitBox>();
                string eventName = hitBoxQueue[0].Item1;
                HitInfo hitInfo = hitBoxQueue[0].Item3;

                for (int n = 0; n < hitBoxQueue.Count; ++n) {
                    HitBox hitBox = hitBoxQueue[n].Item2;
                    hitBoxes.Add(hitBox);
                }

                Action<List<HitBox>, HitInfo> callback = this.hitEventCallbacks[eventName];
                callback(hitBoxes, hitInfo);

                hitBoxQueue.Clear();
            }

            public void InvokeEvent(string eventName, HitBox hitBox, HitInfo hitInfo) {
                if (!hitBoxes.Contains(hitBox)) {
                    throw new InvalidOperationException("Attempting to invoke a hitbox event to a hit it doesn't belongs to");
                }

                Action<List<HitBox>, HitInfo> callback = this.hitEventCallbacks[eventName];
                callback(new List<HitBox> { hitBox }, hitInfo);
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

            private Action<List<HitBox>, HitInfo> DefaultEventHandler(string eventName) {
                return (List<HitBox> hitBoxes, HitInfo hitInfo) => {
                    hitBoxes[0].InvokeEvent(eventName, hitInfo);
                };
            } 
        }
    }
}