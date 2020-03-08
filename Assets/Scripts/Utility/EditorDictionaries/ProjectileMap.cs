using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Gameplay;

namespace ResonantSpark {
    namespace Utility {
        public class ProjectileMap : MonoBehaviour {
            public List<string> keys;
            public List<Projectile> values;

            public Dictionary<string, Projectile> BuildDictionary() {
                Dictionary<string, Projectile> dict = new Dictionary<string, Projectile>();
                for (int n = 0; n < keys.Count; ++n) {
                    dict.Add(keys[n], values.Count > n ? values[n] : null);
                }
                return dict;
            }
        }
    }
}