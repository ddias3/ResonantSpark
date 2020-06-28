using System;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Utility {
        public class MultipassStateDict : MonoBehaviour {

            private Dictionary<string, MultipassBaseState> dict = new Dictionary<string, MultipassBaseState>();

            public void Register(MultipassBaseState state, string id) {
                dict.Add(id, state);
            }

            public MultipassBaseState Get(string id) {
                return dict[id];
            }

            public MultipassStateDict Each(Action<MultipassBaseState> action) {
                foreach (KeyValuePair<string, MultipassBaseState> entry in dict) {
                    action(entry.Value);
                }
                return this;
            }
        }
    }
}
