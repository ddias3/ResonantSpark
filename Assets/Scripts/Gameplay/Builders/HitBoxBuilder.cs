using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Builder;
using ResonantSpark.Gameplay;
using ResonantSpark.Service;
using ResonantSpark.Utility;

namespace ResonantSpark {
    namespace CharacterProperties {
        public partial class HitBoxBuilder : IHitBoxCallbackObject {
            private const string onHitBoxEventKey = "onHitBox";
            private const string onHurtBoxEventKey = "onHurtBox";

            private IHitBoxService hitBoxService;
            private IFightingGameService fgService;

            public HitBoxBuilder(AllServices allServices) {
                eventCallbacks = new Dictionary<string, Action<HitInfo>>();

                this.hitBoxService = allServices.GetService<IHitBoxService>();
                this.fgService = allServices.GetService<IFightingGameService>();
                this.hitBoxPrefab = this.hitBoxService.DefaultPrefab();

                tracking = false;
            }

            public HitBoxComponent CreateHitBox(HitBox hitBox, Transform hitBoxEmptyParentTransform, Action<InGameEntity, Collider, Vector3> hitBoxCallback) {
                    // default means Vector3.zero in this case
                if (collider != null) {
                    HitBoxComponent hitBoxComp = GameObject.Instantiate<HitBoxComponent>(hitBoxPrefab, hitBoxEmptyParentTransform.position, Quaternion.identity, hitBoxEmptyParentTransform);

                    hitBoxComp.SetServices(hitBoxService, fgService);
                    hitBoxComp.Init(hitBox, transform, hitLocation, hitBoxCallback);

                    switch (collider.direction) {
                        case 0: // X-axis
                            hitBoxComp.SetColliderPosition(collider.transform.position, collider.transform.position + collider.transform.right * collider.height, collider.radius);
                            break;
                        case 1: // Y-axis
                            hitBoxComp.SetColliderPosition(collider.transform.position, collider.transform.position + collider.transform.up * collider.height, collider.radius);
                            break;
                        case 2: // Z-axis
                            hitBoxComp.SetColliderPosition(collider.transform.position, collider.transform.position + collider.transform.forward * collider.height, collider.radius);
                            break;
                    }
                    return hitBoxComp;
                }
                else if (radius > 0) {
                    HitBoxComponent hitBoxComp = GameObject.Instantiate<HitBoxComponent>(hitBoxPrefab, hitBoxEmptyParentTransform.position, Quaternion.identity, hitBoxEmptyParentTransform);

                    hitBoxComp.SetServices(hitBoxService, fgService);
                    hitBoxComp.Init(hitBox, transform, hitLocation, hitBoxCallback);

                    hitBoxComp.SetColliderPosition(point0, point1, radius);
                    return hitBoxComp;
                }
                else {
                    throw new NotSupportedException("Attempting to create a hitbox with no collider to base on or no values provided");
                }
            }

            public Dictionary<string, Action<HitInfo>> GetEvents() {
                return eventCallbacks;
            }
        }
    }
}