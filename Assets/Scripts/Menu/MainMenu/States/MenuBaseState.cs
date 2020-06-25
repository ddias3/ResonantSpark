using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Utility;
using ResonantSpark.Menu;

namespace ResonantSpark {
    namespace MenuStates {
        public abstract class MenuBaseState : MonoBehaviour, IState {
            protected StateDict states;
            protected Action<IState> changeState;

            protected MenuInputManager inputManager;
            protected MenuStack menuStack;

            public void Start() {
                states = gameObject.GetComponentInParent<StateDict>();
                inputManager = gameObject.GetComponentInParent<MenuInputManager>();
                menuStack = gameObject.GetComponentInParent<MenuStack>();
            }

            public virtual void OnStateMachineEnable(Action<IState> changeState) {
                this.changeState = changeState;
            }

            public abstract void TriggerEvent(string eventName);

            public abstract void Enter(int frameIndex, IState previousState);
            public abstract void ExecutePass0(int frameIndex);
            public abstract void ExecutePass1(int frameIndex);
            public abstract void Exit(int frameIndex);
        }
    }
}
