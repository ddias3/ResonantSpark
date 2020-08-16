using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace SimplifiedPhysics {
        public class CollisionOffsetAdjust : Constraint {
            public bool zeroYAxis = true;

            public override Vector3 CollisionOffset(Vector3 offset) {
                offset.y = 0;
                return offset;
            }
        }
    }
}
