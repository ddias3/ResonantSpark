using System;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    public class GameTimeManager : MonoBehaviour {

            // TODO: Remake this to be a tree structure instead of always linear
        private List<Func<float, float>> callbacks;
        private List<float> cachedValues;
        private bool valid = false;

        public void Update() {
            valid = false;
        }

        private void Start() {
            callbacks = new List<Func<float, float>>();
            cachedValues = new List<float>();

                // This call is skipped
            callbacks.Add(new Func<float, float>(x => x));
            cachedValues.Add(Time.deltaTime);
        }

        private void CalculateDeltaTime(float deltaTime) {
            float finalDeltaTime = deltaTime;
            cachedValues[0] = deltaTime;
            for (int n = 1; n < callbacks.Count; ++n) {
                finalDeltaTime = callbacks[n].Invoke(finalDeltaTime);
                cachedValues[n] = finalDeltaTime;
            }
        }

        public float Layer(int layerId) {
            if (!valid) {
                CalculateDeltaTime(Time.deltaTime);
            }
            return cachedValues[layerId];
        }

        public int AddLayer(Func<float, float> callback) {
            callbacks.Add(callback);
            cachedValues.Add(0.0f);

            return callbacks.Count - 1;
        }
    }
}