using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Input.Combinations;

namespace ResonantSpark {
    namespace CharacterStates {
        public abstract class BaseState : MonoBehaviour, State {

            protected StateDict states;
            protected GameTimeManager gameTime;

            protected FightingGameCharacter fgChar;
            protected FrameEnforcer frame;

            protected Action<State> changeState;

            protected Queue<Combination> messages;

            private bool continueInputSearch = true;
            private Action stopInputSearch;

            private CallbackRegistry callbackRegistry;
            private Dictionary<Type, Action<Action, Combination>> inputCallbacks;

            public void Start() {
                callbackRegistry = new CallbackRegistry(this);
                inputCallbacks = new Dictionary<Type, Action<Action, Combination>>();
                stopInputSearch = new Action(StopInputSearch);

                messages = new Queue<Combination>();

                states = gameObject.GetComponentInParent<StateDict>();
                fgChar = gameObject.GetComponentInParent<FightingGameCharacter>();
                frame = GameObject.FindGameObjectWithTag("rspGamemode").GetComponent<FrameEnforcer>();
            }

            public CallbackRegistry RegisterInputCallbacks() {
                return callbackRegistry;
            }

            private void StopInputSearch() {
                continueInputSearch = false;
            }

            protected void FindInput(List<Combination> inputCombos) {
                continueInputSearch = true;
                for (int n = 0; n < inputCombos.Count && continueInputSearch; ++n) {
                    Combination combo = inputCombos[n];
                    Action<Action, Combination> callback;

                    if (inputCallbacks.TryGetValue(combo.GetType(), out callback)) {
                        callback(stopInputSearch, combo);
                    }
                }
            }

            public void OnStateMachineEnable(Action<State> changeState) {
                this.changeState = changeState;
            }

            public BaseState Message(Combination combo) {
                messages.Enqueue(combo);
                return this;
            }

            public abstract void Enter(int frameIndex, State previousState);
            public abstract void Execute(int frameIndex);
            public abstract void Exit(int frameIndex);

            public struct CallbackRegistry {
                private BaseState stateRef;

                public CallbackRegistry(BaseState stateRef) {
                    this.stateRef = stateRef;
                }

                public CallbackRegistry On<Tbase>(Action<Action, Combination> callback) where Tbase : Combination {
                    stateRef.inputCallbacks.Add(typeof(Tbase), callback);
                    return this;
                }
            }
        }
    }
}
