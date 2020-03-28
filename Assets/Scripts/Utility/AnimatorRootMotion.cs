using System;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Utility {
        [RequireComponent(typeof(Animator))]
        public class AnimatorRootMotion : MonoBehaviour {
            private Animator animator;
            private Action<Quaternion, Vector3> callback;

            public void Awake() {
                animator = GetComponent<Animator>();
            }

            public void SetCallback(Action<Quaternion, Vector3> callback) {
                this.callback = callback;
            }

            public void OnAnimatorMove() {
                callback?.Invoke(animator.rootRotation, animator.velocity);
            }
        }
    }
}