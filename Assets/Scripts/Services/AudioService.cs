using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Utility;

namespace ResonantSpark {
    namespace Service {
        public class AudioService : MonoBehaviour, IAudioService {

            public AudioResource prefab;
            public Transform audioEmpty;

            private ResourceRecycler<AudioResource> audioSources;

            private List<AudioResource> previousActiveSounds;
            private List<AudioResource> activeSounds;

            private List<AudioResource> activeOneShots;

            private FrameEnforcer frame;

            public void Start() {
                frame = GameObject.FindGameObjectWithTag("rspTime").GetComponent<FrameEnforcer>();
                frame.AddUpdate((int)FramePriority.Service, new System.Action<int>(FrameUpdate));
                frame.AddUpdate((int)FramePriority.ActivePollingReset, new System.Action<int>(ResetActivePolling));

                audioSources = new ResourceRecycler<AudioResource>(prefab, Vector3.zero, 4, audioEmpty, resource => {
                    Debug.Log("callback called");
                    resource.SetService(this);
                    resource.Deactivate();
                });

                // This works exactly as expected.
                //AudioSource test = GameObject.Instantiate<AudioSource>(prefab, new Vector3(-33, 0, 47), Quaternion.identity);
                //test.Play();

                activeSounds = new List<AudioResource>();
                previousActiveSounds = new List<AudioResource>();

                activeOneShots = new List<AudioResource>();

                EventManager.TriggerEvent<Events.ServiceReady, Type>(typeof(AudioService));
            }

            public AudioResource Resource(Vector3 position, AudioClip audioClip, float playbackPosition = 0.0f) {
                AudioResource resource = audioSources.UseResource();
                resource.SetUp(position, audioClip, playbackPosition);

                return resource;
            }

            public void PlayOneShot(Vector3 position, AudioClip audioClip, float playbackPosition = 0.0f) {
                AudioResource resource = audioSources.UseResource();
                resource.SetUp(position, audioClip, playbackPosition);

                activeOneShots.Add(resource);
            }

            private void FrameUpdate(int frameIndex) {
                foreach (AudioResource audio in activeSounds) {
                    if (!previousActiveSounds.Contains(audio)) {
                        if (!audio.IsActive()) {
                            audio.Activate();
                        }
                    }
                }

                foreach (AudioResource audio in previousActiveSounds) {
                    if (!activeSounds.Contains(audio)) {
                        audio.Deactivate();
                    }
                    else {
                        if (!audio.IsActive()) {
                            audio.Deactivate();
                        }
                    }
                }

                for (int n = 0; n < activeOneShots.Count; ++n) {
                    if (!activeOneShots[n].IsActive()) {
                        activeOneShots[n].Deactivate();
                        activeOneShots.RemoveAt(n);
                        --n;
                    }
                }
            }

            private void ResetActivePolling(int frameIndex) {
                previousActiveSounds.Clear();
                foreach (AudioResource audio in activeSounds) {
                    previousActiveSounds.Add(audio);
                }
                activeSounds.Clear();
            }

            public void Play(AudioResource audioResource) {
                activeSounds.Add(audioResource);
            }

            public Transform GetEmptyHoldTransform() {
                return audioEmpty;
            }
        }
    }
}