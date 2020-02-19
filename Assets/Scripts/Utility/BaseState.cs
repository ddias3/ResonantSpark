using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Input.Combinations;
using ResonantSpark.Gameplay;
using ResonantSpark.Character;

namespace ResonantSpark {
    namespace Utility {
        public abstract class BaseState : MonoBehaviour, IState {

            protected StateDict states;

            protected FrameEnforcer frame;

            protected Action<IState> changeState;

            public void Awake() {

                states = gameObject.GetComponentInParent<StateDict>();
                frame = GameObject.FindGameObjectWithTag("rspTime").GetComponent<FrameEnforcer>();
            }

            public void OnStateMachineEnable(Action<IState> changeState) {
                this.changeState = changeState;
            }

            public abstract void Enter(int frameIndex, IState previousState);
            public abstract void Execute(int frameIndex);
            public abstract void Exit(int frameIndex);
        }
    }
}
