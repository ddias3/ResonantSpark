using System;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Gameplay {
        public class InGameEntityStateDict : MonoBehaviour {

            private Dictionary<string, InGameEntityBaseState> dict = new Dictionary<string, InGameEntityBaseState>();

            public void Register(InGameEntityBaseState state, string id) {
                dict.Add(id, state);
            }

            public InGameEntityBaseState Get(string id) {
                return dict[id];
            }

            public InGameEntityStateDict Each(Action<InGameEntityBaseState> action) {
                foreach (KeyValuePair<string, InGameEntityBaseState> entry in dict) {
                    action(entry.Value);
                }
                return this;
            }
        }
    }
}
