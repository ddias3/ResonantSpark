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

            public Hit(Dictionary<string, Action<HitInfo>> eventCallbacks) {
                this.id = Hit.hitCounter++;

                hitBoxes = new List<HitBox>();
            }

            public void AddHitBox(HitBox hitBox) {
                hitBoxes.Add(hitBox);
            }

            public void Active() {
                // TODO: Create Active call in Hit
                //hitBoxService.Active(this);
            }

            public bool Equals(Hit other) {
                return id == other.id;
            }

            public override int GetHashCode() {
                return id;
            }
        }
    }
}