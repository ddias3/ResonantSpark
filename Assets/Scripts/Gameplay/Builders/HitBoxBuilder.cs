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
                callbacks = new Dictionary<string, Action<HitInfo>>();
                this.hitBoxService = allServices.GetService<IHitBoxService>();
                this.fgService = allServices.GetService<IFightingGameService>();
                this.hitBoxPrefab = this.hitBoxService.DefaultPrefab();
            }

            public HitBox CreateHitBox(Transform hitBoxEmptyParentTransform) {
                    // default means Vector3.zero in this case
                if (collider != null) {
                    HitBox hitBox = GameObject.Instantiate<HitBox>(hitBoxPrefab, hitBoxEmptyParentTransform.position, Quaternion.identity, hitBoxEmptyParentTransform);
                    callbacks.TryGetValue(onHurtBoxEventKey, out Action<HitInfo> onHurtBoxCallback);
                    callbacks.TryGetValue(onHitBoxEventKey, out Action<HitInfo> onHitBoxCallback);

                    hitBox.SetServices(hitBoxService, fgService);
                    hitBox.Init(transform, tracking, onHurtBoxCallback, onHitBoxCallback);

                    switch (collider.direction) {
                        case 0: // X-axis
                            hitBox.SetColliderPosition(collider.transform.position, collider.transform.position + collider.transform.right * collider.height, collider.radius);
                            break;
                        case 1: // Y-axis
                            hitBox.SetColliderPosition(collider.transform.position, collider.transform.position + collider.transform.up * collider.height, collider.radius);
                            break;
                        case 2: // Z-axis
                            hitBox.SetColliderPosition(collider.transform.position, collider.transform.position + collider.transform.forward * collider.height, collider.radius);
                            break;
                    }
                    return hitBox;
                }
                else if (radius > 0) {
                    HitBox hitBox = GameObject.Instantiate<HitBox>(hitBoxPrefab, hitBoxEmptyParentTransform.position, Quaternion.identity, hitBoxEmptyParentTransform);
                    callbacks.TryGetValue(onHurtBoxEventKey, out Action<HitInfo> onHurtBoxCallback);
                    callbacks.TryGetValue(onHitBoxEventKey, out Action<HitInfo> onHitBoxCallback);

                    hitBox.SetServices(hitBoxService, fgService);
                    hitBox.Init(transform, tracking, onHurtBoxCallback, onHitBoxCallback);

                    hitBox.SetColliderPosition(point0, point1, radius);
                    return hitBox;
                }
                else {
                    throw new NotSupportedException("Attempting to create a hitbox with no collider to base on or no values provided");
                }
            }

            public Dictionary<string, Action<HitInfo>> GetEvents() {
                return callbacks;
            }
        }
    }
}