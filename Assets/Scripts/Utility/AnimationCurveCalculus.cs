using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Utility {
        public class AnimationCurveCalculus {
            private static float delta = 1e-6f;
            public static float Differentiate(AnimationCurve curve, float x) {
                float x0 = x - delta;
                float x1 = x + delta;
                float y0 = curve.Evaluate(x0);
                float y1 = curve.Evaluate(x1);

                return (y1 - y0) / (x1 - x0);
            }
        }
    }
}