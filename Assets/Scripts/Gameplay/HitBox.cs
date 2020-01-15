using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Gameplay;
using ResonantSpark.Service;

namespace ResonantSpark {
    namespace Character {
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

            private Action<IFightingGameCharacter> onHurtBoxEnter;
            private Action<IFightingGameCharacter> onHitBoxEnter;

            private IHitBoxService hitBoxService;
            private IFightingGameService fgService;

            public HitBox Init(Transform relativeTransform, bool tracking, Action<IFightingGameCharacter> onHurtBoxEnter, Action<IFightingGameCharacter> onHitBoxEnter) {
                this.id = HitBox.hitBoxCounter++;

                this.relativeTransform = relativeTransform;
                this.offset = relativeTransform.position - transform.position;

                this.onHurtBoxEnter = onHurtBoxEnter;
                this.onHitBoxEnter = onHitBoxEnter;

                hitBoxService.RegisterHitBox(this);

                return this;
            }

            public void SetServices(IHitBoxService hitBoxService, IFightingGameService fgService) {
                this.hitBoxService = hitBoxService;
                this.fgService = fgService;
            }

            public void Start() {
                hurtBox = LayerMask.NameToLayer("hurtBox");
                hitBox = LayerMask.NameToLayer("hitBox");
                collider = GetComponentInChildren<CapsuleCollider>();
                rigidbody = GetComponent<Rigidbody>();
                colliderTransform = collider.transform;
            }

            public void OnTriggerEnter(Collider other) {
                if (other.gameObject.layer == hurtBox) {
                    Debug.Log("Hitbox hit a hurtbox");
                    onHurtBoxEnter?.Invoke(null);
                }
                else if (other.gameObject.layer == hitBox) {
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
                collider.enabled = true;

                rigidbody.rotation = relativeTransform.rotation;
                rigidbody.position = relativeTransform.rotation * offset + relativeTransform.position;
            }

            public void Deactivate() {
                collider.enabled = false;

                    // TODO: Provide an actual location to place this hitbox when deactivated.
                rigidbody.position = new Vector3(0, -100, 0);
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