using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Utility;

namespace ResonantSpark {
    namespace Service {
        public class AudioService : MonoBehaviour, IAudioService {

            public AudioResource prefab;
            public Transform audioEmpty;

            private ResourceRecycler<AudioResource> audioSources;

            private FrameEnforcer frame;

            public void Start() {
                frame = GameObject.FindGameObjectWithTag("rspTime").GetComponent<FrameEnforcer>();
                frame.AddUpdate((int)FramePriority.Service, new System.Action<int>(FrameUpdate));
                frame.AddUpdate((int)FramePriority.ActivePollingReset, new System.Action<int>(ResetActivePolling));

                audioSources = new ResourceRecycler<AudioResource>(prefab, Vector3.zero, 4, audioEmpty, resource => {
                    Debug.Log("callback called");
                    resource.Deactivate();
                });

                // This works exactly as expected.
                //AudioSource test = GameObject.Instantiate<AudioSource>(prefab, new Vector3(-33, 0, 47), Quaternion.identity);
                //test.Play();

                EventManager.TriggerEvent<Events.ServiceReady, Type>(typeof(AudioService));
            }

            private void FrameUpdate(int frameIndex) {

            }

            private void ResetActivePolling(int frameIndex) {

            }

            public AudioResource Use(Transform followTransform) {
                AudioResource resource = audioSources.UseResource();

                resource.Activate();
                return resource;

                // TODO: Create this for the functionality that I want.
            }

            private void OnInterrupt() {

            }
        }
    }
}