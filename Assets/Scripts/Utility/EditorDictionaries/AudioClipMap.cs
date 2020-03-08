using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Utility {
        public class AudioClipMap : MonoBehaviour {
            public List<string> keys;
            public List<AudioClip> values;

            public Dictionary<string, AudioClip> BuildDictionary() {
                Dictionary<string, AudioClip> dict = new Dictionary<string, AudioClip>();
                for (int n = 0; n < keys.Count; ++n) {
                    dict.Add(keys[n], values.Count > n ? values[n] : null);
                }
                return dict;
            }
        }
    }
}