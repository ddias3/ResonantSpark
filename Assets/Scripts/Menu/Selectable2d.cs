using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Menu {
        public class Selectable2d : Selectable {
            public Vector3 offset;
            public float width;

            private RectTransform rectTransform;

            public new void Awake() {
                base.Awake();
                rectTransform = GetComponent<RectTransform>();
            }

            public override Transform GetTransform() {
                return rectTransform;
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
