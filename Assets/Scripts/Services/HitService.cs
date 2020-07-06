using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Gameplay;

namespace ResonantSpark {
    namespace Service {
        public class HitService : MonoBehaviour, IHitService {

            private Dictionary<int, Hit> hitMap;

            private FightingGameService fgService;

            public void Start() {
                FrameEnforcer frame = GameObject.FindGameObjectWithTag("rspTime").GetComponent<FrameEnforcer>();
                frame.AddUpdate((int)FramePriority.LateService, new System.Action<int>(FrameLateUpdate));

                hitMap = new Dictionary<int, Hit>();

                fgService = GetComponent<FightingGameService>();

                EventManager.TriggerEvent<Events.ServiceReady, Type>(typeof(HitService));
            }

            public void RegisterHit(Hit hit) {
                hitMap.Add(hit.id, hit);
            }

            private void FrameLateUpdate(int frameIndex) {
                foreach (KeyValuePair<int, Hit> kvp in hitMap) {
                    Hit hit = kvp.Value;
                    Dictionary<InGameEntity, (List<HurtBox> hurt, List<HitBox> hit)> entityMap = hit.ParseHitInfo();

                    if (entityMap != null) {
                            // In the future, I'm going to change such that attacks can change properties based on a callback.
                        fgService.PerformHits(hit, entityMap);
                    }
                }
            }
        }
    }
}