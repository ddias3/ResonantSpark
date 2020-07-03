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
            public Vector3 point0 { get; private set; }
            public Vector3 point1 { get; private set; }
            public float radius { get; private set; }

            private bool active = false;

            private Transform relativeTransform;
            private Vector3 offset;
            private new Rigidbody rigidbody;

            private new CapsuleCollider collider;
            private Transform colliderTransform;

            private new HitBoxRenderer renderer;
            private Transform rendererTransform;

            private Vector3 localHitLocation;

            private Vector3 deactivatedPosition;

            private IHitBoxService hitBoxService;
            private IFightingGameService fgService;

            public void Init(HitBox hitBox, Transform relativeTransform, Vector3 hitLocation) {
                this.hitBox = hitBox;
                this.relativeTransform = relativeTransform;

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
        }
    }
}