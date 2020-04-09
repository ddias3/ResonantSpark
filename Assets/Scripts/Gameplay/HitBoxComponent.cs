using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Service;
using ResonantSpark.Utility;
using ResonantSpark.CharacterProperties;
using ResonantSpark.Builder;

namespace ResonantSpark {
    namespace Gameplay {
        public class HitBoxComponent : MonoBehaviour {
            public HitBox hitBox { get; private set; }

            private LayerMask hurtBoxLayer;
            private LayerMask hitBoxLayer;

            private Transform relativeTransform;
            private Vector3 offset;
            private new Rigidbody rigidbody;

            private new CapsuleCollider collider;
            private Transform colliderTransform;

            private new HitBoxRenderer renderer;
            private Transform rendererTransform;

            private Vector3 localHitLocation;

            private Vector3 deactivatedPosition;

            Action<InGameEntity, Collider, Vector3> hitBoxCallback;

            private IHitBoxService hitBoxService;
            private IFightingGameService fgService;

            public void Init(HitBox hitBox, Transform relativeTransform, Vector3 hitLocation, Action<InGameEntity, Collider, Vector3> hitBoxCallback) {
                this.hitBox = hitBox;
                this.relativeTransform = relativeTransform;
                this.hitBoxCallback = hitBoxCallback;

                //this.offset = relativeTransform.position - transform.position;
                this.offset = Vector3.zero;

                localHitLocation = hitLocation;

                collider = GetComponentInChildren<CapsuleCollider>();
                renderer = GetComponentInChildren<HitBoxRenderer>();
                rigidbody = GetComponent<Rigidbody>();
                colliderTransform = collider.transform;
                rendererTransform = renderer.transform;

                deactivatedPosition = hitBoxService.GetEmptyHoldTransform().position;
                Deactivate();
            }

            public void SetServices(IHitBoxService hitBoxService, IFightingGameService fgService) {
                this.hitBoxService = hitBoxService;
                this.fgService = fgService;
            }

            public void Awake() {
                hurtBoxLayer = LayerMask.NameToLayer("HurtBox");
                hitBoxLayer = LayerMask.NameToLayer("HitBox");
            }

            public void OnTriggerEnter(Collider other) {
                if (other.gameObject.layer == hurtBoxLayer || other.gameObject.layer == hitBoxLayer) {
                    InGameEntity gameEntity = other.gameObject.GetComponentInParent<InGameEntity>();
                    hitBoxCallback(gameEntity, other, transform.position + transform.rotation * localHitLocation);
                }
            }

            public void SetColliderPosition(Vector3 point0, Vector3 point1, float radius) {
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
        }
    }
}