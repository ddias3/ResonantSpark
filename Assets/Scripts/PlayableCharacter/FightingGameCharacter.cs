using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    public class FightingGameCharacter : MonoBehaviour {

        public Animator animator;

        private InputBuffer inputBuffer;

        private GameObject target;
        private GameObject opponentChar;

        public GameObject opponentCharacter {
            set { opponentChar = value; }
        }

        public InputBuffer input {
            set { inputBuffer = value; }
        }

        public void Start() {
            // so far do nothing
        }

        public void Play(string animationState) {
            animator.Play(animationState);
        }

        public void Play(string animationState, int layer, float normalizedTime) {
            animator.Play(animationState, layer, normalizedTime);
        }

        public List<Input.Combinations.Combination> GetFoundCombinations() {
            // TODO: look through them if necessary.
            return inputBuffer.GetFoundCombinations();
        }
    }
}