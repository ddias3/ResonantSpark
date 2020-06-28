using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Input.Combinations;
using ResonantSpark.Gameplay;
using ResonantSpark.Character;
using ResonantSpark.Utility;

namespace ResonantSpark {
    namespace CharacterStates {
        public abstract class CharacterBaseState : MultipassBaseState {

            protected FightingGameCharacter fgChar;

            private bool continueInputSearch = true;
            private Action stopInputSearch;
            private Action<Combination> removeComboFromInUse;
            private List<Combination> toBeRemovedCombosFromInUse;

            private CallbackRegistry<Action<Action, Combination>> inputRegistry;
            private CallbackRegistry<Action<Action, Combination>> enterRegistry;
            private Dictionary<Type, Action<Action, Combination>> inputCallbacks;
            private Dictionary<Type, Action<Action, Combination>> enterCallbacks;

            public new void Awake() {
                base.Awake();
                inputCallbacks = new Dictionary<Type, Action<Action, Combination>>();
                enterCallbacks = new Dictionary<Type, Action<Action, Combination>>();
                inputRegistry = new CallbackRegistry<Action<Action, Combination>>(inputCallbacks);
                enterRegistry = new CallbackRegistry<Action<Action, Combination>>(enterCallbacks);
                stopInputSearch = new Action(StopInputSearch);

                removeComboFromInUse = new Action<Combination>(RemoveComboFromInUse);
                toBeRemovedCombosFromInUse = new List<Combination>();

                fgChar = gameObject.GetComponentInParent<FightingGameCharacter>();
            }

            public override void OnStateMachineEnable(Action<MultipassBaseState> changeState) {
                this.changeState = (newState) => {
                    fgChar.SetState((CharacterBaseState) newState);
                };
            }

            public CallbackRegistry<Action<Action, Combination>> RegisterInputCallbacks() {
                return inputRegistry;
            }

            public CallbackRegistry<Action<Action, Combination>> RegisterEnterCallbacks() {
                return enterRegistry;
            }

            private void StopInputSearch() {
                continueInputSearch = false;
            }

            private void RemoveComboFromInUse(Combination combo) {
                toBeRemovedCombosFromInUse.Add(combo);
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
                        toBeRemovedCombosFromInUse.Add(combo);
                    }
                }

                RemoveCombosFromInUse();
            }

            private void RemoveCombosFromInUse() {
                fgChar.RemoveInUseCombinations(toBeRemovedCombosFromInUse);
                toBeRemovedCombosFromInUse.Clear();
            }

            public abstract GroundRelation GetGroundRelation();
            public abstract void GetHit(bool launch);

            public struct CallbackRegistry<Tcallback> {
                private Dictionary<Type, Tcallback> callbackMap;

                public CallbackRegistry(Dictionary<Type, Tcallback> callbackMap) {
                    this.callbackMap = callbackMap;
                }

                public CallbackRegistry<Tcallback> On<Tbase>(Tcallback callback) where Tbase : Combination {
                    callbackMap.Add(typeof(Tbase), callback);
                    return this;
                }
            }
        }
    }
}
