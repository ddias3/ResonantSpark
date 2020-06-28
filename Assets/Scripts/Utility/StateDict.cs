using System;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Utility {
        public class StateDict: MonoBehaviour {

            private Dictionary<string, IState> dict = new Dictionary<string, IState>();

            public void Register(IState state, string id) {
                dict.Add(id, state);
            }

            public IState Get(string id) {
                return dict[id];
            }

            public StateDict Each(Action<IState> action) {
                foreach (KeyValuePair<string, IState> entry in dict) {
                    action(entry.Value);
                }
                return this;
            }
        }
    }
}
