using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Character {
        public class HitBox : ScriptableObject {

            public Vector3 relativeLocation { get; private set; }
            public Collider collider { get; private set; } // maybe make all of them capsule colliders

            public HitBox Init(Vector3 relLoc, Collider col) {
                this.relativeLocation = relLoc;
                this.collider = col;
                return this;
            }
        }
    }
}