using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Utility;

namespace ResonantSpark {
    namespace MenuStates {
        public class Intro : MonoBehaviour, IState {
            public Animator camera;

            private StateDict states;
            private Action<IState> changeState;

            public void Start() {
                states = gameObject.GetComponentInParent<StateDict>();

                states.Register(this, "intro");

                // TODO: camera.onEvent("end intro"); or something like that
            }

            public virtual void OnStateMachineEnable(Action<IState> changeState) {
                this.changeState = changeState;
            }

            public void Enter(int frameIndex, IState previousState) {
                camera.Play("intro");
            }

            public void Execute(int frameIndex) {
                // TODO: figure this one out.
            }

            public void Exit(int frameIndex) {
                // do nothing
            }
        }
    }
}