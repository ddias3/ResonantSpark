using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Utility;
using ResonantSpark.Service;

namespace ResonantSpark {
    public class AudioResource : MonoBehaviour, IResource {

        public AudioService service;

        private bool inUse;
        private AudioSource source;

        public void Awake() {
            inUse = false;
            source = GetComponent<AudioSource>();
        }

        public void SetService(IAudioService audioService) {
            this.service = (AudioService) audioService;
        }

        public void SetUp(Vector3 position, AudioClip audioClip, float playbackPosition) {
            transform.position = position;

            source.clip = audioClip;
            source.time = playbackPosition;

            source.Play();
        }

        public bool IsActive() {
            //return inUse;
            return source.isPlaying;
        }

        public void Activate() {
            inUse = true;
            //source.Play();
        }

        public void Deactivate() {
            inUse = false;
            source.Stop();
        }
    }
}