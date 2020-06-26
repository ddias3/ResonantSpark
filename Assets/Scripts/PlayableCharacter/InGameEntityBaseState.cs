using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Input.Combinations;
using ResonantSpark.Gameplay;
using ResonantSpark.Character;

namespace ResonantSpark {
    namespace Gameplay {
        public abstract class InGameEntityBaseState : MonoBehaviour {

            protected InGameEntityStateDict states;
            protected FrameEnforcer frame;
            protected Action<InGameEntityBaseState> changeState;

            public void Awake() {
                states = gameObject.GetComponentInParent<InGameEntityStateDict>();
                frame = GameObject.FindGameObjectWithTag("rspTime").GetComponent<FrameEnforcer>();
            }

            public virtual void OnStateMachineEnable(Action<InGameEntityBaseState> changeState) {
                this.changeState = changeState;
            }

            public abstract void Enter(int frameIndex, InGameEntityBaseState previousState);
            public abstract void ExecutePass0(int frameIndex);
            public abstract void ExecutePass1(int frameIndex);
            public abstract void LateExecute(int frameIndex);
            public abstract void Exit(int frameIndex);
        }
    }
}
