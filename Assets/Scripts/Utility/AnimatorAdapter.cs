using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Utility {
        public class AnimatorAdapter : MonoBehaviour {
            public List<Animator> animators;

            public void SetFloat(string varName, float value) {
                for (int n = 0; n < animators.Count; ++n) {
                    animators[n].SetFloat(varName, value);
                }
            }

            public void Play(string stateName, int layer, float normalizedTime) {
                for (int n = 0; n < animators.Count; ++n) {
                    animators[n].Play(stateName, layer, normalizedTime);
                }
            }
        }
    }
}