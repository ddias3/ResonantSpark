using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    public class AnimationVariance : MonoBehaviour {

        public float lowerBound;
        public float higherBound;

        private Animator animator;

        private void Start() {
            animator = GetComponent<Animator>();
            StartCoroutine(AdjustAnimatorPlaybackSpeed());
        }

        private IEnumerator AdjustAnimatorPlaybackSpeed() {
            animator.speed = Random.value * (lowerBound - higherBound) + lowerBound;
            yield return new WaitForSeconds(20.0f);
            StartCoroutine(AdjustAnimatorPlaybackSpeed());
        }
    }
}