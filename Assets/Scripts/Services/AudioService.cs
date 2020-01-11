using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Utility;

namespace ResonantSpark {
    namespace Service {
        public class AudioService : MonoBehaviour {

            public AudioResource prefab;
            public Transform audioEmpty;

            private ResourceRecycler<AudioResource> audioSources;

            public void Start() {
                audioSources = new ResourceRecycler<AudioResource>(prefab, Vector3.zero, 4, audioEmpty, resource => {
                    Debug.Log("callback called");
                    resource.Deactivate();
                });

                // This works exactly as expected.
                //AudioSource test = GameObject.Instantiate<AudioSource>(prefab, new Vector3(-33, 0, 47), Quaternion.identity);
                //test.Play();
            }
        }
    }
}