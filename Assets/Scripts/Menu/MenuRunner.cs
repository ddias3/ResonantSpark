using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Utility;

namespace ResonantSpark {
    namespace Menu {
        public class MenuRunner : MonoBehaviour {
            protected StateMachine stateMachine;
            protected StateDict states;

            public void Start() {
                stateMachine = GetComponentInChildren<StateMachine>();
                states = GetComponentInChildren<StateDict>();
            }
        }
    }
}
