using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Builder;
using ResonantSpark.Character;

namespace ResonantSpark {
    namespace CharacterProperties {
        public class HitBoxBuilder : IHitBoxCallbackObject {
            private List<HitBox> hitBoxes;
            private Dictionary<string, Action> callbacks;

            public List<Vector3> vecs {
                private set;
                get;
            }

            public HitBoxBuilder() {
                hitBoxes = new List<HitBox>();
                callbacks = new Dictionary<string, Action>();

                // Temporary
                vecs = new List<Vector3>();
            }

            public IHitBoxCallbackObject Add(Vector3 v0) {
                vecs.Add(v0);
                return this;
            }

            public IHitBoxCallbackObject Event(string eventName, Action callback) {
                callbacks.Add(eventName, callback);
                return this;
            }

            public List<HitBox> CreateHitBoxes() {
                return null;
            }

            public Dictionary<string, Action> GetEvents() {
                return callbacks;
            }
        }
    }
}