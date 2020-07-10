using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Gameplay;
using ResonantSpark.Utility;
using ResonantSpark.Builder;

namespace ResonantSpark {
    namespace Builder {
        public interface IHitBoxCallbackObject {
            IHitBoxCallbackObject Prefab(HitBox hitBoxPrefab);
            IHitBoxCallbackObject Point0(Vector3 p0);
            IHitBoxCallbackObject Point1(Vector3 p1);
            IHitBoxCallbackObject Radius(float width);
            IHitBoxCallbackObject HitLocation(Vector3 hitLocation);
            IHitBoxCallbackObject Tracking(bool tracking);
            IHitBoxCallbackObject FromCollider(CapsuleCollider collider);
            IHitBoxCallbackObject Relative(Transform transform);
            IHitBoxCallbackObject InGameEntity(InGameEntity entity);
            IHitBoxCallbackObject Validate(Func<HitBox, HurtBox, bool> validateCallback);
            IHitBoxCallbackObject Validate(Func<HitBox, HitBox, bool> validateCallback);
        }
    }

    namespace CharacterProperties {
        public partial class HitBoxBuilder : IHitBoxCallbackObject {
            private HitBox hitBoxPrefab;

            public bool tracking { get; private set; }
            public Transform relativeTransform { get; private set; }
            public Vector3 hitLocation { get; private set; }
            public InGameEntity entity { get; private set; }
            public Func<HitBox, HurtBox, bool> validateOnHurtCallback { get; private set; }
            public Func<HitBox, HitBox, bool> validateOnHitCallback { get; private set; }

            private CapsuleCollider collider;

            private Vector3 point0;
            private Vector3 point1;

            private float radius = -1;

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

            public IHitBoxCallbackObject HitLocation(Vector3 hitLocation) {
                this.hitLocation = hitLocation;
                return this;
            }

            public IHitBoxCallbackObject Tracking(bool tracking) {
                this.tracking = tracking;
                return this;
            }

            public IHitBoxCallbackObject FromCollider(CapsuleCollider collider) {
                this.collider = collider;
                return this;
            }

            public IHitBoxCallbackObject Relative(Transform relativeTransform) {
                this.relativeTransform = relativeTransform;
                return this;
            }

            public IHitBoxCallbackObject InGameEntity(InGameEntity entity) {
                this.entity = entity;
                return this;
            }

            public IHitBoxCallbackObject Validate(Func<HitBox, HurtBox, bool> validateOnHurtCallback) {
                this.validateOnHurtCallback = validateOnHurtCallback;
                return this;
            }

            public IHitBoxCallbackObject Validate(Func<HitBox, HitBox, bool> validateOnHitCallback) {
                this.validateOnHitCallback = validateOnHitCallback;
                return this;
            }
        }
    }
}
