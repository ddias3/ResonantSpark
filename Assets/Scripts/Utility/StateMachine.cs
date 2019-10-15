using System;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    public class StateMachine : MonoBehaviour {

        private State curr;
        private List<State> nextStates;
        private Action<State> changeStateCallback;
        private bool changeState = false;

        public void Execute(int frameIndex) {
            try {
                curr.Execute(frameIndex, changeStateCallback);
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
            catch {
                Debug.LogError("State Machine threw Exception");
                Debug.LogError("  error in state: " + curr.ToString());
                Debug.LogError("Missing Exception");
                //gameObject.SetActive(false);
                this.enabled = false;
            }
        }

        public State GetCurrentState() {
            return curr;
        }

        public void Enable(State startState, FrameEnforcer frame) {
            nextStates = new List<State>();
            changeStateCallback = (State nextState) => {
                changeState = true;
                nextStates.Add(nextState);
            };
            //this.enabled = true;

            frame.SetUpdate(new Action<int>(Execute));
            curr = startState;
        }

        private void ChangeState(int frameIndex) {
            State nextState = nextStates[0];
            nextStates.RemoveAt(0);
            curr = nextState;

            curr.Exit(frameIndex);
            nextState.Enter(frameIndex, curr);

            if (nextStates.Count == 0) {
                changeState = false;
            }
        }
    }
}