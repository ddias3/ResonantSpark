using System;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    public class StateMachine : MonoBehaviour {

        public CharacterStates.StateDict stateDict;

#if UNITY_EDITOR
        public string currStateName;
#endif

        private IState initState;

        private IState curr;
        private List<IState> nextStates;
        private Action<IState> changeStateCallback;
        private bool changeState = false;

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
            catch {
                Debug.LogError("State Machine threw Exception");
                Debug.LogError("  error in state: " + curr.ToString());
                Debug.LogError("Missing Exception");
                //gameObject.SetActive(false);
                this.enabled = false;
            }
        }

        public IState GetCurrentState() {
            return curr;
        }

        public void Enable(IState startState, FrameEnforcer frame) {
            nextStates = new List<IState>();
            changeStateCallback = (IState nextState) => {
                changeState = true;
                nextStates.Add(nextState);
            };
            //this.enabled = true;

            frame.AddUpdate((int) FramePriority.StateMachine, new Action<int>(Execute));
            curr = startState;
            initState = startState;

            stateDict.Each(state => {
                state.OnStateMachineEnable(changeStateCallback);
            });

            startState.Enter(-1, null);
        }

        public void Reset() {
            //TODO: Reset StateMachine
            //   I'm not sure I like this design, so far
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