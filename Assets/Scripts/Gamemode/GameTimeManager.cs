using System;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    public class GameTimeManager : MonoBehaviour {

            // TODO: Remake this to be a tree structure instead of always linear
        private List<Func<float, float>> callbacks;
        private Dictionary<string, int> layerNames;
        private List<float> cachedValues;
        private bool valid = false;

        public void Update() {
            valid = false;
        }

        private void Start() {
            callbacks = new List<Func<float, float>>();
            cachedValues = new List<float>();
            layerNames = new Dictionary<string, int>();

            // This call is skipped in CalculateDeltaTime
            //callbacks.Add(new Func<float, float>(x => x));
            //cachedValues.Add(Time.deltaTime);
            AddLayer(new Func<float, float>(x => x), "realTime");
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
                //CalculateDeltaTime(1.0f / 60.0f);
                CalculateDeltaTime(Time.deltaTime);
            }
            return cachedValues[layerId];
        }

        public float Layer(string layerName) {
            return Layer(layerNames[layerName]);
        }

        public int AddLayer(Func<float, float> callback, string name = null) {
            callbacks.Add(callback);
            cachedValues.Add(0.0f);

            if (name != null) {
                layerNames.Add(name, callbacks.Count - 1);
            }

            return callbacks.Count - 1;
        }
    }
}