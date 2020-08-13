using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Gameplay;
using ResonantSpark.Builder;
using ResonantSpark.CharacterProperties;

namespace ResonantSpark {
    namespace Service {
        public class HitService : MonoBehaviour, IHitService {

            private BuildService buildService;

            private Dictionary<int, Hit> hitMap;

            public void Start() {
                FrameEnforcer frame = GameObject.FindGameObjectWithTag("rspTime").GetComponent<FrameEnforcer>();
                frame.AddUpdate((int)FramePriority.ServiceHit, new System.Action<int>(FrameUpdate));
                frame.AddUpdate((int)FramePriority.LateService, new System.Action<int>(LateFrameUpdate));

                buildService = GetComponent<BuildService>();
                hitMap = new Dictionary<int, Hit>();

                AllServices allServices = new AllServices();
                allServices.AddServiceAs<IHitBoxService>(this);

                EventManager.TriggerEvent<Events.ServiceReady, Type>(typeof(HitService));
            }

            public Hit Create(Action<IHitCallbackObject> buildCallback) {
                HitBuilder hitBuilder = new HitBuilder(buildService.GetAllServices());
                buildCallback(hitBuilder);
                return hitBuilder.CreateHit();
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
                        // This HAS to run on the LateFrameUpdate or else it clears important dictionaries before they are used.
                    //hit.ClearHitQueue();
                }
            }

            private void LateFrameUpdate(int frameIndex) {
                foreach (KeyValuePair<int, Hit> kvp in hitMap) {
                    Hit hit = kvp.Value;
                    hit.ClearHitQueue();
                }
            }
        }
    }
}