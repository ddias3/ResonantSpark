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
                frame.AddUpdate((int)FramePriority.ServiceHit, new System.Action<int>(FrameUpdate));

                hitMap = new Dictionary<int, Hit>();

                EventManager.TriggerEvent<Events.ServiceReady, Type>(typeof(HitService));
            }

            public void RegisterHit(Hit hit) {
                hitMap.Add(hit.id, hit);
            }

            private void FrameUpdate(int frameIndex) {
                foreach (KeyValuePair<int, Hit> kvp in hitMap) {
                    Hit hit = kvp.Value;
                    Dictionary<InGameEntity, (List<HurtBox> hurt, List<HitBox> hit)> entityMap = hit.ParseHitInfo();

                    if (entityMap != null) {
                            // In the future, I'm going to change such that attacks can change properties based on a callback.
                        foreach (KeyValuePair<InGameEntity, (List<HurtBox> hurt, List<HitBox> hit)> entitiesKvp in entityMap) {
                            InGameEntity inGameEntity = entitiesKvp.Key;
                            hit.InvokeCallback(inGameEntity, entitiesKvp.Value.hurt, entitiesKvp.Value.hit);
                        }
                    }

                    hit.ClearHitQueue();
                }
            }
        }
    }
}