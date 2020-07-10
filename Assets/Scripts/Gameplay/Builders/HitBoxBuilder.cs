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
            private IHitBoxService hitBoxService;
            private IFightingGameService fgService;

            public HitBoxBuilder(AllServices allServices) {
                this.hitBoxService = allServices.GetService<IHitBoxService>();
                this.fgService = allServices.GetService<IFightingGameService>();
                this.hitBoxPrefab = this.hitBoxService.DefaultPrefab();

                tracking = false;
                hitLocation = Vector3.up;
                entity = null;
                validateOnHitCallback = DefaultValidateOnHitCallback;
                validateOnHurtCallback = DefaultValidateOnHurtCallback;
            }

            public void SetColliderPosition(HitBox hitBox) {
                if (collider != null) {
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
                }
                else if (radius > 0) {
                    hitBox.SetColliderPosition(point0, point1, radius);
                }
                else {
                    throw new NotSupportedException("Attempting to create a hitbox with no collider to base on or no values provided");
                }
            }

            private bool DefaultValidateOnHitCallback(HitBox checkingHitBox, HitBox otherHitBox) {
                return true;
            }

            private bool DefaultValidateOnHurtCallback(HitBox checkingHitBox, HurtBox otherHurtBox) {
                return true;
            }
        }
    }
}