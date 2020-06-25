using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace SimplifiedPhysics {
        public enum StationaryColliderType : int {
            Ground = 0,
            Wall = 1,
            Ceiling = 2,
            Debris = 3,
        }

        public class StationaryCollider : MonoBehaviour {
            public StationaryColliderType colliderType;
            public Collider collider { private set; get; }

            public void Awake() {
                PhysicsTracker.Get().Add(this);
                collider = GetComponent<Collider>();
            }
        }
    }
}