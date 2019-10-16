using System;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace CharacterStates {
        public class StateDict : MonoBehaviour {

            private Dictionary<string, BaseState> dict = new Dictionary<string, BaseState>();

            public void Register(BaseState state, string id) {
                dict.Add(id, state);
            }

            public BaseState Get(string id) {
                return dict[id];
            }

            public StateDict Each(Action<BaseState> action) {
                foreach (KeyValuePair<string, BaseState> entry in dict) {
                    action(entry.Value);
                }
                return this;
            }
        }
    }
}
