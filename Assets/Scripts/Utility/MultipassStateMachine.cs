using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Utility;

namespace ResonantSpark {
    public class MultipassStateMachine : MonoBehaviour {

        public MultipassStateDict stateDict;

#if UNITY_EDITOR
        public string currStateName;
#endif

        private MultipassBaseState initState;

        private MultipassBaseState curr;
        private List<MultipassBaseState> nextStates;
        private Action<MultipassBaseState> changeStateCallback;
        private bool changeState = false;

        private List<Action<MultipassBaseState>> onChangeState;

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

        public MultipassBaseState GetCurrentState() {
            return curr;
        }

        public (Action<int>, Action<int>, Action<int>) Enable(MultipassBaseState startState) {
            nextStates = new List<MultipassBaseState>();
            if (onChangeState == null) {
                onChangeState = new List<Action<MultipassBaseState>>();
            }

            //this.enabled = true;
            curr = startState;
            initState = startState;

            stateDict.Each(state => {
                state.OnStateMachineEnable(new Action<MultipassBaseState>(QueueStateChange));
            });

            startState.Enter(-1, null);

            return (ExecutePass0, ExecutePass1, LateExecute);
        }

        public void RegisterOnChangeStateCallback(Action<MultipassBaseState> callback) {
            if (onChangeState == null) {
                onChangeState = new List<Action<MultipassBaseState>>();
            }

            onChangeState.Add(callback);
        }

        public void Reset() {
            // TODO: Reset StateMachine
            //   I'm not sure I like this design, so far
        }

        public void QueueStateChange(MultipassBaseState nextState) {
            if (nextStates.Count != 0) Debug.LogWarning("Next State isn't empty, multiple ChangeState calls on the same frame");

            changeState = true;
            nextStates.Add(nextState);

            foreach (Action<MultipassBaseState> callback in onChangeState) {
                callback(nextState);
            }
        }

        private void ChangeState(int frameIndex) {
            MultipassBaseState nextState = nextStates[0];
            nextStates.Clear(); //nextStates.RemoveAt(0);

            curr.Exit(frameIndex);
            nextState.Enter(frameIndex, curr);

            curr = nextState;

            if (nextStates.Count == 0) {
                changeState = false;
            }
        }
    }
}