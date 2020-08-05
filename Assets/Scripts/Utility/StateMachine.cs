using System;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    public class StateMachine : MonoBehaviour {

        public Utility.StateDict stateDict;

#if UNITY_EDITOR
        public string currStateName;
#endif

        private IState initState;

        private IState curr;
        private List<IState> nextStates;
        private Action<IState> changeStateCallback;
        private bool changeState = false;

        private List<Action<IState>> onChangeState;

        public void Execute(int frameIndex) {
            try {
#if UNITY_EDITOR
                currStateName = curr.GetType().Name;
#endif
                curr.Execute(frameIndex);
                if (changeState) {
                    ChangeState(frameIndex);
                }
            }
            catch (Exception ex) {
                Debug.LogError("State Machine threw Exception");
                Debug.LogError("  error in state: " + curr.ToString());
                Debug.LogError(ex);
                //gameObject.SetActive(false);
                this.enabled = false;
            }
        }

        public IState GetCurrentState() {
            return curr;
        }

        public Action<int> Enable(IState startState) {
            nextStates = new List<IState>();
            if (onChangeState == null) {
                onChangeState = new List<Action<IState>>();
            }

            //this.enabled = true;
            curr = startState;
            initState = startState;

            stateDict.Each(state => {
                state.OnStateMachineEnable(new Action<IState>(QueueStateChange));
            });

            startState.Enter(-1, null);

            return Execute;
        }

        public void RegisterOnChangeStateCallback(Action<IState> callback) {
            if (onChangeState == null) {
                onChangeState = new List<Action<IState>>();
            }

            onChangeState.Add(callback);
        }

        public void Reset() {
            // TODO: Reset StateMachine
            //   I'm not sure I like this design, so far
        }

        public void QueueStateChange(IState nextState) {
            if (nextStates.Count != 0) Debug.LogWarning("Next State isn't empty, multiple ChangeState calls on the same frame");

            changeState = true;
            nextStates.Add(nextState);

            foreach (Action<IState> callback in onChangeState) {
                callback(nextState);
            }
        }

        private void ChangeState(int frameIndex) {
            IState nextState = nextStates[0];
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