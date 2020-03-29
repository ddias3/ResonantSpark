using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Input.Combinations;
using ResonantSpark.Gameplay;
using ResonantSpark.Character;
using ResonantSpark.Utility;

namespace ResonantSpark {
    namespace CharacterStates {
        public abstract class CharacterBaseState : BaseState {

            protected FightingGameCharacter fgChar;

            private bool continueInputSearch = true;
            private Action stopInputSearch;

            private CallbackRegistry inputRegistry;
            private CallbackRegistry enterRegistry;
            private Dictionary<Type, Action<Action, Combination>> inputCallbacks;
            private Dictionary<Type, Action<Action, Combination>> enterCallbacks;

            public new void Awake() {
                base.Awake();
                inputCallbacks = new Dictionary<Type, Action<Action, Combination>>();
                enterCallbacks = new Dictionary<Type, Action<Action, Combination>>();
                inputRegistry = new CallbackRegistry(inputCallbacks);
                enterRegistry = new CallbackRegistry(enterCallbacks);
                stopInputSearch = new Action(StopInputSearch);

                fgChar = gameObject.GetComponentInParent<FightingGameCharacter>();
            }

            public override void OnStateMachineEnable(Action<IState> changeState) {
                this.changeState = (newState) => {
                    fgChar.SetState((CharacterBaseState) newState);
                };
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

            public abstract GroundRelation GetGroundRelation();
            public abstract void GetHitBy(HitBox hitBox);

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
