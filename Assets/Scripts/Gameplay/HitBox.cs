using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Service;
using ResonantSpark.CharacterProperties;
using ResonantSpark.Builder;

namespace ResonantSpark {
    namespace Gameplay {
        public class HitBox : MonoBehaviour, IEquatable<HitBox> {
            private static int hitBoxCounter = 0;

            public int id { get; private set; }
            public Hit hit { get; set; }

            public Vector3 point0 { get; private set; }
            public Vector3 point1 { get; private set; }
            public float radius { get; private set; }

            private LayerMask hitableLayers;
            private LayerMask hurtBoxLayer;
            private LayerMask hitBoxLayer;

            private Collider[] colliderBuffer;

            private InGameEntity entity;
            private Func<HitBox, HurtBox, bool> validateOnHurtCallback;
            private Func<HitBox, HitBox, bool> validateOnHitCallback;

            private Transform relativeTransform;
            private Vector3 localHitLocation;
            private Vector3 offset;

            private bool tracking;

            private IHitBoxService hitBoxService;
            private IFightingGameService fgService;

            private Action<IHitBoxCallbackObject> buildCallback;

            private bool active = false;

            private new Rigidbody rigidbody;

            private new CapsuleCollider collider;
            private Transform colliderTransform;

            private new HitBoxRenderer renderer;
            private Transform rendererTransform;

            private Vector3 deactivatedPosition;

            public void Init(Action<IHitBoxCallbackObject> buildCallback) {
                this.id = HitBox.hitBoxCounter++;
                this.buildCallback = buildCallback;

                hitableLayers = LayerMask.GetMask("HurtBox", "HitBox");
                hurtBoxLayer = LayerMask.NameToLayer("HurtBox");
                hitBoxLayer = LayerMask.NameToLayer("HitBox");

                colliderBuffer = new Collider[128];
            }

            public HitBox Build(AllServices services, Hit hit) {
                this.hitBoxService = services.GetService<IHitBoxService>();
                this.fgService = services.GetService<IFightingGameService>();

                this.hit = hit;

                collider = GetComponentInChildren<CapsuleCollider>();
                renderer = GetComponentInChildren<HitBoxRenderer>();
                rigidbody = GetComponent<Rigidbody>();
                colliderTransform = collider.transform;
                rendererTransform = renderer.transform;

                if (Persistence.Get().gamemode == "versus") {
                    renderer.enable = false;
                }
                else {
                    renderer.enable = true;
                }

                collider.enabled = true;
                collider.isTrigger = true;

                deactivatedPosition = hitBoxService.GetEmptyHoldTransform().position;
                Deactivate();

                hitBoxService.RegisterHitBox(this);

                HitBoxBuilder builder = new HitBoxBuilder(services);
                buildCallback(builder);
                builder.SetColliderPosition(this);

                entity = builder.entity;
                relativeTransform = builder.relativeTransform;
                localHitLocation = builder.hitLocation;
                tracking = builder.tracking;
                validateOnHitCallback = builder.validateOnHitCallback;
                validateOnHurtCallback = builder.validateOnHurtCallback;

                return this;
            }

            public void SetColliderPosition(Vector3 point0, Vector3 point1, float radius) {
                this.point0 = point0;
                this.point1 = point1;
                this.radius = radius;

                if (Mathf.Abs(Vector3.Cross(point1 - point0, Vector3.up).sqrMagnitude) > 0.01f) {
                    colliderTransform.localRotation = Quaternion.LookRotation(point1 - point0, Vector3.up);
                    rendererTransform.localRotation = Quaternion.LookRotation(point1 - point0, Vector3.up);
                }
                else {
                    colliderTransform.localRotation = Quaternion.LookRotation(point1 - point0, Vector3.right);
                    rendererTransform.localRotation = Quaternion.LookRotation(point1 - point0, Vector3.right);
                }

                colliderTransform.localPosition = point0;
                rendererTransform.localPosition = point0;

                float cylinderHeight = Vector3.Distance(point0, point1);

                collider.radius = radius;
                collider.direction = 2; // Z-axis
                collider.height = cylinderHeight + 2 * radius;
                collider.center = cylinderHeight / 2 * Vector3.forward;

                renderer.radius = radius;
                renderer.height = cylinderHeight;
            }

            public void PerformOverlapCheck() {
                int numCollisions = 0;
                if ((numCollisions =
                        Physics.OverlapCapsuleNonAlloc(relativeTransform.position + relativeTransform.rotation * point0,
                            relativeTransform.position + relativeTransform.rotation * point1,
                            radius,
                            colliderBuffer,
                            hitableLayers,
                            QueryTriggerInteraction.Collide)) > 0) {
                    for (int n = 0; n < numCollisions; ++n) {
                        if (colliderBuffer[n].gameObject.layer == hurtBoxLayer || colliderBuffer[n].gameObject.layer == hitBoxLayer) {
                            HurtBox hurtBox;
                            if ((hurtBox = colliderBuffer[n].gameObject.GetComponent<HurtBox>()) != null) {
                                InGameEntity gameEntity = hurtBox.GetEntity();
                                Debug.LogFormat("Type {0}, Entity {1}", "HurtBox", gameEntity);
                                if (validateOnHurtCallback(this, hurtBox)) {
                                    hit.RegisterHitInfo(this, relativeTransform.position + relativeTransform.rotation * localHitLocation, hurtBox);
                                }
                            }

                            HitBox otherHitBox;
                            if ((otherHitBox = colliderBuffer[n].gameObject.GetComponent<HitBox>()) != null) {
                                InGameEntity gameEntity = otherHitBox.GetEntity();
                                Debug.LogFormat("Type {0}, Entity {1}", "HitBox", gameEntity);
                                if (validateOnHitCallback(this, otherHitBox)) {
                                    hit.RegisterHitInfo(this, relativeTransform.position + relativeTransform.rotation * localHitLocation, otherHitBox);
                                }
                            }
                        }
                    }
                }
                Debug.LogFormat("### Num Collisions {0}", numCollisions);
            }

            public InGameEntity GetEntity() {
                return entity;
            }

            public void Active() {
                hitBoxService.Active(this);
            }

            public bool IsActive() {
                return active; //collider.enabled;
            }

            public void Activate() {
                active = true;
                //collider.enabled = true;
                rigidbody.rotation = relativeTransform.rotation;
                rigidbody.position = relativeTransform.rotation * offset + relativeTransform.position;
            }

            public void Deactivate() {
                active = false;
                //collider.enabled = false;
                rigidbody.position = deactivatedPosition;
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