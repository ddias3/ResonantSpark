using System;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Gameplay {
        public class InGameEntityStateMachine : MonoBehaviour {

            public InGameEntityStateDict stateDict;

#if UNITY_EDITOR
            public string currStateName;
#endif

            private InGameEntityBaseState initState;

            private InGameEntityBaseState curr;
            private List<InGameEntityBaseState> nextStates;
            private Action<InGameEntityBaseState> changeStateCallback;
            private bool changeState = false;

            private List<Action<InGameEntityBaseState>> onChangeState;

            public void ExecutePass0(int frameIndex) {
                try {
#if UNITY_EDITOR
                    currStateName = curr.GetType().Name;
#endif
                    if (changeState) {
                        ChangeState(frameIndex);
                    }

                    curr.ExecutePass0(frameIndex);
                }
                catch (Exception ex) {
                    Debug.LogError("State Machine threw Exception");
                    Debug.LogError("  error in state: " + curr.ToString());
                    Debug.LogError(ex);
                    //gameObject.SetActive(false);
                    this.enabled = false;
                }
            }

            public void ExecutePass1(int frameIndex) {
                try {
#if UNITY_EDITOR
                    currStateName = curr.GetType().Name;
#endif
                    curr.ExecutePass1(frameIndex);
                }
                catch (Exception ex) {
                    Debug.LogError("State Machine threw Exception");
                    Debug.LogError("  error in state: " + curr.ToString());
                    Debug.LogError(ex);
                    //gameObject.SetActive(false);
                    this.enabled = false;
                }
            }

            public void LateExecute(int frameIndex) {
                try {
#if UNITY_EDITOR
                    currStateName = curr.GetType().Name;
#endif
                    curr.LateExecute(frameIndex);
                }
                catch (Exception ex) {
                    Debug.LogError("State Machine threw Exception");
                    Debug.LogError("  error in state: " + curr.ToString());
                    Debug.LogError(ex);
                    //gameObject.SetActive(false);
                    this.enabled = false;
                }
            }

            public InGameEntityBaseState GetCurrentState() {
                return curr;
            }

            public (Action<int>, Action<int>, Action<int>) Enable(InGameEntityBaseState startState) {
                nextStates = new List<InGameEntityBaseState>();
                if (onChangeState == null) {
                    onChangeState = new List<Action<InGameEntityBaseState>>();
                }

                //this.enabled = true;
                curr = startState;
                initState = startState;

                stateDict.Each(state => {
                    state.OnStateMachineEnable(new Action<InGameEntityBaseState>(QueueStateChange));
                });

                startState.Enter(-1, null);

                return (ExecutePass0, ExecutePass1, LateExecute);
            }

            public void RegisterOnChangeStateCallback(Action<InGameEntityBaseState> callback) {
                if (onChangeState == null) {
                    onChangeState = new List<Action<InGameEntityBaseState>>();
                }

                onChangeState.Add(callback);
            }

            public void Reset() {
                //TODO: Reset StateMachine
                //   I'm not sure I like this design, so far
            }

            public void QueueStateChange(InGameEntityBaseState nextState) {
                if (nextStates.Count != 0) Debug.LogWarning("Next State isn't empty, multiple ChangeState calls on the same frame");

                changeState = true;
                nextStates.Add(nextState);

                foreach (Action<InGameEntityBaseState> callback in onChangeState) {
                    callback(nextState);
                }
            }

            private void ChangeState(int frameIndex) {
                InGameEntityBaseState nextState = nextStates[0];
                nextStates.RemoveAt(0);

                curr.Exit(frameIndex);
                nextState.Enter(frameIndex, curr);

                curr = nextState;

                if (nextStates.Count == 0) {
                    changeState = false;
                }
            }
        }
    }
}