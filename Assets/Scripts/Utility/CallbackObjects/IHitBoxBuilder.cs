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
            IHitBoxCallbackObject Tracking(bool tracking);
            IHitBoxCallbackObject FromCollider(CapsuleCollider collider);
            IHitBoxCallbackObject Relative(Transform transform);
            IHitBoxCallbackObject Event(string eventName, Action<HitInfo> callback);
        }
    }

    namespace CharacterProperties {
        public partial class HitBoxBuilder : IHitBoxCallbackObject {
            private HitBox hitBoxPrefab;

            private Dictionary<string, Action<HitInfo>> callbacks;

            private CapsuleCollider collider;
            private Transform transform;

            private Vector3 point0;
            private Vector3 point1;

            private float radius = -1;
            private bool tracking = false;

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

            public IHitBoxCallbackObject Event(string eventName, Action<HitInfo> callback) {
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
        }
    }
}
