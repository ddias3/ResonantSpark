using System;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace CharacterStates {
        public class StateDict : MonoBehaviour {

            private Dictionary<string, CharacterBaseState> dict = new Dictionary<string, CharacterBaseState>();

            public void Register(CharacterBaseState state, string id) {
                dict.Add(id, state);
            }

            public CharacterBaseState Get(string id) {
                return dict[id];
            }

            public StateDict Each(Action<CharacterBaseState> action) {
                foreach (KeyValuePair<string, CharacterBaseState> entry in dict) {
                    action(entry.Value);
                }
                return this;
            }
        }
    }
}
