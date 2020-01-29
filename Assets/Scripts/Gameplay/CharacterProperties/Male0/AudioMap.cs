using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace CharacterProperties {
        namespace Male0 {
            public class AudioMap : MonoBehaviour {
                public List<string> audioClipNames;
                public List<AudioClip> audioClips;

                public Dictionary<string, AudioClip> BuildDictionary() {
                    Dictionary<string, AudioClip> dict = new Dictionary<string, AudioClip>();
                    for (int n = 0; n < audioClipNames.Count; ++n) {
                        dict.Add(audioClipNames[n], audioClips.Count > n ? audioClips[n] : null);
                    }
                    return dict;
                }
            }
        }
    }
}