using System;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Utility {
        public class FunctionCalculus {
            private static float delta = 1e-4f;
            public static float Differentiate(Func<float, float> function, float x) {
                float x0 = x - delta;
                float x1 = x + delta;
                float y0 = function(x0);
                float y1 = function(x1);

                return (y1 - y0) / (x1 - x0);
            }
        }
    }
}