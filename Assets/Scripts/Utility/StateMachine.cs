using System;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    public class StateMachine : MonoBehaviour {

        private State curr;
        private List<State> nextStates;
        private Action<State> changeStateCallback;
        private bool changeState = false;

        public void Update() {
            try {
                curr.Execute(changeStateCallback);
                if (changeState) {
                    ChangeState();
                }
            }
            catch (Exception ex) {
                Debug.LogError("State Machine threw Exception");
                Debug.LogError("  error in state: " + curr.ToString());
                Debug.LogError(ex);
                gameObject.SetActive(false);
            }
            catch {
                Debug.LogError("State Machine threw Exception");
                Debug.LogError("  error in state: " + curr.ToString());
                Debug.LogError("Missing Exception");
                gameObject.SetActive(false);
            }
        }

        public State GetCurrentState() {
            return curr;
        }

        public void Enable(State startState) {
            nextStates = new List<State>();
            changeStateCallback = (State nextState) => {
                changeState = true;
                nextStates.Add(nextState);
            };
            gameObject.SetActive(true);

            curr = startState;
        }

        private void ChangeState() {
            State nextState = nextStates[0];
            nextStates.RemoveAt(0);
            curr = nextState;

            curr.Exit();
            nextState.Enter(curr);

            if (nextStates.Count == 0) {
                changeState = false;
            }
        }
    }
}