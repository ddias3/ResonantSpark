using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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

            public AnimatorStateInfo GetCurrentAnimatorStateInfo(int index, int layerIndex) {
                return animators[index].GetCurrentAnimatorStateInfo(layerIndex);
            }
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(AnimatorAdapter))]
        public class AnimatorAdapterInspector : Editor {
            public override void OnInspectorGUI() {
                AnimatorAdapter adapter = (AnimatorAdapter)target;

                GUILayout.Label("Test Label");
                GUILayout.Label("When you get around, make the 1-to-many relations of this adapter");

                EditorGUILayout.LabelField("Some help", "Some other text");

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.TextField("ab");
                EditorGUILayout.TextField("cd");
                EditorGUILayout.EndHorizontal();

                DrawDefaultInspector();
            }
        }
#endif

    }
}