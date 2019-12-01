using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Builder;
using ResonantSpark.Character;

namespace ResonantSpark {
    namespace CharacterProperties {
        public class HitBoxBuilder : IHitBoxBuilder {
            private List<HitBox> hitBoxes;
            private Dictionary<string, Action> callbacks;

            public HitBoxBuilder() {
                hitBoxes = new List<HitBox>();
                callbacks = new Dictionary<string, Action>();
            }

            public IHitBoxBuilder Add(Vector3 v0) {
                return this;
            }

            public IHitBoxBuilder Event(string eventName, Action callback) {
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