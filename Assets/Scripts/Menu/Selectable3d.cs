using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Menu {
        public class Selectable3d : Selectable {
            public Vector3 offset;
            public float width;

            public override Transform GetTransform() {
                return transform;
            }

            public override Vector3 Offset() {
                return offset;
            }

            public override float Width() {
                return width;
            }
        }
    }
}
