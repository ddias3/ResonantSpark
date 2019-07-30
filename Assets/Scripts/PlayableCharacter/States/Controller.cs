using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace CharacterStates {
        public class Controller : MonoBehaviour {

            //public StateMachine charStateMachine;
            //public BaseState startState;

            //public void Start() {
            //    Debug.Log("2");
            //    charStateMachine.Enable(startState);
            //}

            public Animator animator;

            public void Play(string animationState) {
                animator.Play(animationState);
            }

            public void Play(string animationState, int layer, float normalizedTime) {
                animator.Play(animationState, layer, normalizedTime);
            }
        }
    }
}