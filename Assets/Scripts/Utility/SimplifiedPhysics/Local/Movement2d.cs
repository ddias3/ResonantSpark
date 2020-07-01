using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace SimplifiedPhysics {
        public class Movement2d : Constraint {
            private Vector3 plane;

            public void SetPlane(Vector3 plane) {
                this.plane = plane;
            }

            public override void Restrict(Rigidbody rigidbody) {

            }
        }
    }
}
