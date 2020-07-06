using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Service;
using ResonantSpark.Utility;
using ResonantSpark.CharacterProperties;
using ResonantSpark.Builder;

namespace ResonantSpark {
    namespace Gameplay {
        public class HitBox : IEquatable<HitBox> {
            private static int hitBoxCounter = 0;

            public int id { get; private set; }
            public Hit hit { get; set; }

            private LayerMask hitableLayers;
            private LayerMask hurtBoxLayer;
            private LayerMask hitBoxLayer;

            private Collider[] colliderBuffer;

            private HitBoxComponent hitBoxComponent;
            private Transform relativeTransform;
            private Vector3 localHitLocation;

            private bool tracking;

            private Dictionary<string, Action<HitInfo>> eventCallbacks;

            private IHitBoxService hitBoxService;
            private IFightingGameService fgService;

            private Action<IHitBoxCallbackObject> buildCallback;

            public HitBox(Action<IHitBoxCallbackObject> buildCallback) {
                this.id = HitBox.hitBoxCounter++;
                this.buildCallback = buildCallback;

                colliderBuffer = new Collider[128];
            }

            public HitBox Build(AllServices services, Hit hit) {
                this.id = HitBox.hitBoxCounter++;

                this.hitBoxService = services.GetService<IHitBoxService>();
                this.fgService = services.GetService<IFightingGameService>();

                this.hit = hit;

                hitBoxService.RegisterHitBox(this);

                hitableLayers = LayerMask.GetMask("HurtBox", "HitBox");
                hurtBoxLayer = LayerMask.NameToLayer("HurtBox");
                hitBoxLayer = LayerMask.NameToLayer("HitBox");

                HitBoxBuilder builder = new HitBoxBuilder(services);
                buildCallback(builder);
                hitBoxComponent = builder.CreateHitBox(this, hitBoxService.GetEmptyHoldTransform());

                relativeTransform = builder.relativeTransform;
                localHitLocation = builder.hitLocation;
                tracking = builder.tracking;
                eventCallbacks = builder.eventCallbacks;

                return this;
            }

            public HitBox Init(Transform relativeTransform, bool tracking, Action<HitInfo> onHurtBoxEnter, Action<HitInfo> onHitBoxEnter) {
                Deactivate();

                return this;
            }

            public void InvokeEvent(string eventName, HitInfo hitInfo) {
                Action<HitInfo> callback;
                if (eventCallbacks.TryGetValue(eventName, out callback)) {
                    callback(hitInfo);
                }
            }

            public void Active() {
                hitBoxService.Active(this);
                int numCollisions = 0;
                if ((numCollisions =
                        Physics.OverlapCapsuleNonAlloc(relativeTransform.position + relativeTransform.rotation * hitBoxComponent.point0,
                            relativeTransform.position + relativeTransform.rotation * hitBoxComponent.point1,
                            hitBoxComponent.radius,
                            colliderBuffer,
                            hitableLayers,
                            QueryTriggerInteraction.Collide)) > 0) {
                    for (int n = 0; n < numCollisions; ++n) {
                        if (colliderBuffer[n].gameObject.layer == hurtBoxLayer || colliderBuffer[n].gameObject.layer == hitBoxLayer) {
                            HurtBox hurtBox;
                            if ((hurtBox = colliderBuffer[n].gameObject.GetComponent<HurtBox>()) != null) {
                                InGameEntity gameEntity = hurtBox.GetEntity();
                                Debug.LogFormat("Type {0}, Entity {1}", "HurtBox", gameEntity);
                                hit.RegisterHitInfo(this, relativeTransform.position + relativeTransform.rotation * localHitLocation, hurtBox);
                            }

                            //HitBox otherHitBox;
                            //if ((otherHitBox = colliderBuffer[n].gameObject.GetComponent<HitBox>()) != null) {
                            //    InGameEntity gameEntity = otherHitBox.GetEntity();
                            //    Debug.LogFormat("Type {0}, Entity {1}", "HitBox", gameEntity);
                            //    hit.RegisterHitInfo(this, relativeTransform.position + relativeTransform.rotation * localHitLocation, otherHitBox);
                            //}
                        }
                    }
                }
                Debug.LogFormat("### Num Collisions {0}", numCollisions);
            }

            public InGameEntity GetEntity() {
                return null;
            }

            public bool IsActive() {
                return hitBoxComponent.IsActive();
            }

            public void Activate() {
                hitBoxComponent.Activate();
            }

            public void Deactivate() {
                hitBoxComponent.Deactivate();
            }

            public bool Equals(HitBox other) {
                return id == other.id;
            }

            public override int GetHashCode() {
                return id;
            }
        }
    }
}