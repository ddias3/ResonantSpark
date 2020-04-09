using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Particle;

namespace ResonantSpark {
    namespace Utility {
        public class ParticleEffectMap : MonoBehaviour {
            public List<string> keys;
            public List<ParticleEffect> values;

            public Dictionary<string, ParticleEffect> BuildDictionary() {
                Dictionary<string, ParticleEffect> dict = new Dictionary<string, ParticleEffect>();
                for (int n = 0; n < keys.Count; ++n) {
                    dict.Add(keys[n], values.Count > n ? values[n] : null);
                }
                return dict;
            }
        }
    }
}
