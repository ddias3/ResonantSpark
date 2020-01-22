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

            private Vector3 deactivatedPosition;

            private Action<IFightingGameCharacter> onHurtBoxEnter;
            private Action<IFightingGameCharacter> onHitBoxEnter;

            private IHitBoxService hitBoxService;
            private IFightingGameService fgService;

            public HitBox Init(Transform relativeTransform, bool tracking, Action<IFightingGameCharacter> onHurtBoxEnter, Action<IFightingGameCharacter> onHitBoxEnter) {
                this.id = HitBox.hitBoxCounter++;

                this.relativeTransform = relativeTransform;
                //this.offset = relativeTransform.position - transform.position;
                this.offset = Vector3.zero;

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
                    onHurtBoxEnter?.Invoke(null);
                }
                else if (other.gameObject.layer == hitBox) {
                    Debug.Log(other.gameObject);
                    Debug.Log("Hitbox hit other hitbox");
                    onHitBoxEnter?.Invoke(null);
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

            public void Active() {
                hitBoxService.Active(this);
            }

            public bool IsActive() {
                return collider.enabled;
            }

            public void Activate() {
                //Debug.Break();
                collider.enabled = true;

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