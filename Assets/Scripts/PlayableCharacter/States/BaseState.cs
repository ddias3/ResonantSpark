using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Input.Combinations;
using ResonantSpark.Gameplay;

namespace ResonantSpark {
    namespace CharacterStates {
        public abstract class BaseState : MonoBehaviour, IState {

            protected StateDict states;

            protected FightingGameCharacter fgChar;
            protected FrameEnforcer frame;

            protected Action<IState> changeState;

            private bool continueInputSearch = true;
            private Action stopInputSearch;

            private CallbackRegistry inputRegistry;
            private CallbackRegistry enterRegistry;
            private Dictionary<Type, Action<Action, Combination>> inputCallbacks;
            private Dictionary<Type, Action<Action, Combination>> enterCallbacks;

            public void Awake() {
                inputCallbacks = new Dictionary<Type, Action<Action, Combination>>();
                enterCallbacks = new Dictionary<Type, Action<Action, Combination>>();
                inputRegistry = new CallbackRegistry(inputCallbacks);
                enterRegistry = new CallbackRegistry(enterCallbacks);
                stopInputSearch = new Action(StopInputSearch);

                states = gameObject.GetComponentInParent<StateDict>();
                fgChar = gameObject.GetComponentInParent<FightingGameCharacter>();
                frame = GameObject.FindGameObjectWithTag("rspTime").GetComponent<FrameEnforcer>();
            }

            public CallbackRegistry RegisterInputCallbacks() {
                return inputRegistry;
            }

            public CallbackRegistry RegisterEnterCallbacks() {
                return enterRegistry;
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

            protected void GivenInput(List<Combination> inputCombos) {
                continueInputSearch = true;
                for (int n = 0; n < inputCombos.Count && continueInputSearch; ++n) {
                    Combination combo = inputCombos[n];
                    Action<Action, Combination> callback;

                    if (enterCallbacks.TryGetValue(combo.GetType(), out callback)) {
                        callback(stopInputSearch, combo);
                    }
                }
                inputCombos.Clear();
            }

            public void OnStateMachineEnable(Action<IState> changeState) {
                this.changeState = changeState;
            }

            public abstract void Enter(int frameIndex, IState previousState);
            public abstract void Execute(int frameIndex);
            public abstract void Exit(int frameIndex);

            public struct CallbackRegistry {
                private Dictionary<Type, Action<Action, Combination>> callbackMap;

                public CallbackRegistry(Dictionary<Type, Action<Action, Combination>> callbackMap) {
                    this.callbackMap = callbackMap;
                }

                public CallbackRegistry On<Tbase>(Action<Action, Combination> callback) where Tbase : Combination {
                    callbackMap.Add(typeof(Tbase), callback);
                    return this;
                }
            }
        }
    }
}
