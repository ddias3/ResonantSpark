using System;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    public class StateMachine : MonoBehaviour {

        private Stack<State> currStates;
        private List<State> nextStates;
        private Action<State> changeStateCallback;
        private bool changeState = false;

        public void Update() {
            var curr = currStates.Peek();
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
            return currStates.Peek();
        }

        public void Enable(State startState) {
            currStates = new Stack<State>();
            nextStates = new List<State>();
            changeStateCallback = (State nextState) => {
                changeState = true;
                nextStates.Add(nextState);
            };
            gameObject.SetActive(true);

            currStates.Push(startState);
        }

        private void ChangeState() {
            State curr = currStates.Pop();
            State nextState = nextStates[0];
            nextStates.RemoveAt(0);
            currStates.Push(nextState);

            curr.Exit();
            nextState.Enter(curr);

            changeState = false;
        }
    }
}