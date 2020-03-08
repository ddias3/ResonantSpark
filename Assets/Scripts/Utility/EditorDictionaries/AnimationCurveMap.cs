using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Utility {
        public class AnimationCurveMap : MonoBehaviour {
            public List<string> keys;
            public List<AnimationCurve> values;

            public Dictionary<string, AnimationCurve> BuildDictionary() {
                Dictionary<string, AnimationCurve> dict = new Dictionary<string, AnimationCurve>();
                for (int n = 0; n < keys.Count; ++n) {
                    dict.Add(keys[n], values.Count > n ? values[n] : null);
                }
                return dict;
            }
        }
    }
}

