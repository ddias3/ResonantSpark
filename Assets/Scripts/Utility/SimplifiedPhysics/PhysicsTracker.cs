using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Gameplay;

namespace ResonantSpark {
    namespace SimplifiedPhysics {
        public class PhysicsTracker {
            private static PhysicsTracker physics = null;
            public static PhysicsTracker Get() {
                if (physics == null) {
                    physics = new PhysicsTracker();
                }
                return physics;
            }

            public List<RigidbodyFG> rigidFGs { get; private set; }
            public List<StationaryCollider> stationaryColliders { get; private set; }

            private PhysicsTracker() {
                rigidFGs = new List<RigidbodyFG>();
                stationaryColliders = new List<StationaryCollider>();
            }

            public void Add(RigidbodyFG rigidFG) {
                rigidFGs.Add(rigidFG);
            }

            public void Add(StationaryCollider collider) {
                stationaryColliders.Add(collider);
            }

            public void Clear() {
                rigidFGs.Clear();
                stationaryColliders.Clear();
            }
        }
    }
}