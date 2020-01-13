using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Builder;
using ResonantSpark.Character;
using ResonantSpark.Gameplay;
using ResonantSpark.Service;

namespace ResonantSpark {
    namespace CharacterProperties {
        public class HitBoxBuilder : IHitBoxCallbackObject {
            private const string onHitBoxEventKey = "onHitBox";
            private const string onHurtBoxEventKey = "onHurtBox";

            private HitBox hitBoxPrefab;

            private Dictionary<string, Action<IFightingGameCharacter>> callbacks;

            private CapsuleCollider collider;
            private Transform transform;

            private Vector3 point0;
            private Vector3 point1;

            private float radius = -1;
            private bool tracking = false;

            public HitBoxBuilder(IHitBoxService hitBoxService) {
                callbacks = new Dictionary<string, Action<IFightingGameCharacter>>();
                this.hitBoxPrefab = hitBoxService.DefaultPrefab();
            }

            public IHitBoxCallbackObject Prefab(HitBox hitBoxPrefab) {
                this.hitBoxPrefab = hitBoxPrefab;
                return this;
            }

            public IHitBoxCallbackObject Point0(Vector3 p0) {
                this.point0 = p0;
                return this;
            }

            public IHitBoxCallbackObject Point1(Vector3 p1) {
                this.point1 = p1;
                return this;
            }

            public IHitBoxCallbackObject Radius(float width) {
                this.radius = width;
                return this;
            }

            public IHitBoxCallbackObject Tracking(bool tracking) {
                this.tracking = tracking;
                return this;
            }

            public IHitBoxCallbackObject Event(string eventName, Action<IFightingGameCharacter> callback) {
                callbacks.Add(eventName, callback);
                return this;
            }

            public IHitBoxCallbackObject FromCollider(CapsuleCollider collider) {
                this.collider = collider;
                return this;
            }

            public IHitBoxCallbackObject Relative(Transform transform) {
                this.transform = transform;
                return this;
            }

            public HitBox CreateHitBox(Transform hitBoxEmptyParentTransform) {
                    // default means Vector3.zero in this case
                if (collider != null) {
                    // TODO: supply the HitBox root prefab.
                    HitBox hitBox = GameObject.Instantiate<HitBox>(hitBoxPrefab, hitBoxEmptyParentTransform.position, Quaternion.identity, hitBoxEmptyParentTransform);
                    callbacks.TryGetValue(onHurtBoxEventKey, out Action<IFightingGameCharacter> onHurtBoxCallback);
                    callbacks.TryGetValue(onHitBoxEventKey, out Action<IFightingGameCharacter> onHitBoxCallback);
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
                else if (radius > 0 && point0 != default && point1 != default) {
                    // TODO: supply the HitBox root prefab.
                    HitBox hitBox = GameObject.Instantiate<HitBox>(hitBoxPrefab, hitBoxEmptyParentTransform.position, Quaternion.identity, hitBoxEmptyParentTransform);
                    callbacks.TryGetValue(onHurtBoxEventKey, out Action<IFightingGameCharacter> onHurtBoxCallback);
                    callbacks.TryGetValue(onHitBoxEventKey, out Action<IFightingGameCharacter> onHitBoxCallback);
                    hitBox.Init(transform, tracking, onHurtBoxCallback, onHitBoxCallback);

                    hitBox.SetColliderPosition(point0, point1, radius);
                    return hitBox;
                }
                else {
                    throw new NotSupportedException("Attempting to create a hitbox with no collider to base on or no values provided");
                }
            }

            public Dictionary<string, Action<IFightingGameCharacter>> GetEvents() {
                return callbacks;
            }
        }
    }
}