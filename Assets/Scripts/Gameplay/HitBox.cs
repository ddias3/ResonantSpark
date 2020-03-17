using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Service;
using ResonantSpark.Utility;

namespace ResonantSpark {
    namespace Gameplay {
        public class HitBox : MonoBehaviour, IEquatable<HitBox> {
            private static int hitBoxCounter = 0;

            public int id { get; private set; }

            private LayerMask hurtBox;
            private LayerMask hitBox;

            private Transform relativeTransform;
            private Vector3 offset;
            private new Rigidbody rigidbody;

            private new CapsuleCollider collider;
            private Transform colliderTransform;

            private Vector3 localHitLocation;

            private Vector3 deactivatedPosition;

            private List<InGameEntity> hitEntities;

            private Action<HitInfo> onHurtBoxEnter;
            private Action<HitInfo> onHitBoxEnter;

            private IHitBoxService hitBoxService;
            private IFightingGameService fgService;

            public HitBox Init(Transform relativeTransform, bool tracking, Action<HitInfo> onHurtBoxEnter, Action<HitInfo> onHitBoxEnter) {
                this.id = HitBox.hitBoxCounter++;

                this.relativeTransform = relativeTransform;
                //this.offset = relativeTransform.position - transform.position;
                this.offset = Vector3.zero;

                // TODO: Set Local Hit Location
                localHitLocation = Vector3.up;

                hitEntities = new List<InGameEntity>();

                this.onHurtBoxEnter = onHurtBoxEnter;
                this.onHitBoxEnter = onHitBoxEnter;

                hitBoxService.RegisterHitBox(this);

                collider = GetComponentInChildren<CapsuleCollider>();
                rigidbody = GetComponent<Rigidbody>();
                colliderTransform = collider.transform;

                deactivatedPosition = hitBoxService.GetEmptyHoldTransform().position;
                Deactivate();

                return this;
            }

            public void SetServices(IHitBoxService hitBoxService, IFightingGameService fgService) {
                this.hitBoxService = hitBoxService;
                this.fgService = fgService;
            }

            public void Awake() {
                hurtBox = LayerMask.NameToLayer("HurtBox");
                hitBox = LayerMask.NameToLayer("HitBox");
            }

            public void OnTriggerEnter(Collider other) {
                if (other.gameObject.layer == hurtBox) {
                    Debug.Log(other.gameObject);
                    Debug.Log("Hitbox hit a hurtbox");

                    InGameEntity gameEntity = other.gameObject.GetComponentInParent<InGameEntity>();
                    if (gameEntity != null && !hitEntities.Contains(gameEntity)) {
                        hitEntities.Add(gameEntity);
                        HitInfo hitInfo = new HitInfo(this, gameEntity, transform.position + transform.rotation * localHitLocation, -1);
                        onHurtBoxEnter?.Invoke(hitInfo);
                    }
                }
                else if (other.gameObject.layer == hitBox) {
                    Debug.Log(other.gameObject);
                    Debug.Log("Hitbox hit other hitbox");
                    InGameEntity gameEntity = other.gameObject.GetComponentInParent<InGameEntity>();
                    if (gameEntity != null && !hitEntities.Contains(gameEntity)) {
                        hitEntities.Add(gameEntity);
                        HitInfo hitInfo = new HitInfo(this, gameEntity, transform.position + transform.rotation * localHitLocation, -1);
                        onHitBoxEnter?.Invoke(hitInfo);
                    }
                }
            }

            public void SetColliderPosition(Vector3 point0, Vector3 point1, float radius) {
                colliderTransform.LookAt(point1 - point0, Vector3.up);

                float cylinderHeight = Vector3.Distance(point0, point1);

                collider.radius = radius;
                collider.direction = 2; // Z-axis
                collider.height = cylinderHeight + 2 * radius;
                collider.center = (cylinderHeight / 2 + radius) * Vector3.forward;
            }

            public void InvokeEvent(string eventName, HitInfo hitInfo) {
                // TODO: Invoke event callbacks
            }

            public void Active() {
                hitBoxService.Active(this);
            }

            public bool IsActive() {
                return collider.enabled;
            }

            public void Activate() {
                //Debug.Break();
                collider.enabled = true;
                hitEntities.Clear();

                    // I'm not sure witch method of movement is better for this situation. They are technically different.
                //rigidbody.rotation = relativeTransform.rotation;
                //rigidbody.position = relativeTransform.rotation * offset + relativeTransform.position;
                rigidbody.MoveRotation(relativeTransform.rotation);
                rigidbody.MovePosition(relativeTransform.rotation * offset + relativeTransform.position);
            }

            public void Deactivate() {
                collider.enabled = false;

                    // I'm not sure witch method of movement is better for this situation. They are technically different.
                //rigidbody.position = deactivatedPosition;
                rigidbody.MovePosition(deactivatedPosition);
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