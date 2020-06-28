using System;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Utility {
        public abstract class MultipassBaseState : MonoBehaviour {

            protected MultipassStateDict states;
            protected FrameEnforcer frame;
            protected Action<MultipassBaseState> changeState;

            public void Awake() {
                states = gameObject.GetComponentInParent<MultipassStateDict>();
                frame = GameObject.FindGameObjectWithTag("rspTime").GetComponent<FrameEnforcer>();
            }

            public virtual void OnStateMachineEnable(Action<MultipassBaseState> changeState) {
                this.changeState = changeState;
            }

            public abstract void Enter(int frameIndex, MultipassBaseState previousState);
            public abstract void ExecutePass0(int frameIndex);
            public abstract void ExecutePass1(int frameIndex);
            public abstract void LateExecute(int frameIndex);
            public abstract void Exit(int frameIndex);
        }
    }
}
