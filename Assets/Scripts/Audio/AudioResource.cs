using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Utility;

namespace ResonantSpark {
    public class AudioResource : MonoBehaviour, IResource {

        private bool inUse;
        private AudioSource source;

        public void Awake() {
            inUse = false;
            source = GetComponent<AudioSource>();
        }

        public bool Active() {
            return inUse;
        }

        public void Activate() {
            throw new System.NotImplementedException();
        }

        public void Deactivate() {
            source.Stop();
        }

        // Update is called once per frame
        void Update() {

        }
    }
}