using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Gameplay;

namespace ResonantSpark {
    namespace Service {
        public class HitService : MonoBehaviour, IHitService {

            private Dictionary<int, Hit> hitMap;

            public void Start() {
                FrameEnforcer frame = GameObject.FindGameObjectWithTag("rspTime").GetComponent<FrameEnforcer>();
                frame.AddUpdate((int)FramePriority.LateService, new System.Action<int>(FrameLateUpdate));

                hitMap = new Dictionary<int, Hit>();

                EventManager.TriggerEvent<Events.ServiceReady, Type>(typeof(HitService));
            }

            public void RegisterHit(Hit hit) {
                hitMap.Add(hit.id, hit);
            }

            private void FrameLateUpdate(int frameIndex) {
                foreach (KeyValuePair<int, Hit> kvp in hitMap) {
                    Hit hit = kvp.Value;
                    hit.InvokeQueuedEvents();
                }
            }
        }
    }
}