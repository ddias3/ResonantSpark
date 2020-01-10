using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Utility;

namespace ResonantSpark {
    namespace Service {
        public class AudioService : MonoBehaviour {

            public AudioResource prefab;
            //public AudioSource test;

            private ResourceRecycler<AudioResource> audioSources;

            public void Start() {
                audioSources = ScriptableObject.CreateInstance<ResourceRecycler<AudioResource>>();
                audioSources.Init(prefab, Vector3.zero, 4, null, resource => {
                    Debug.Log("callback called");
                    resource.Deactivate();
                });

                    // This works exactly as expected.
                //test = GameObject.Instantiate<AudioSource>(prefab, new Vector3(-33, 0, 47), Quaternion.identity);
                //test.Play();
            }
        }
    }
}