using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Gameplay;
using ResonantSpark.Utility;
using ResonantSpark.Builder;

namespace ResonantSpark {
    namespace Builder {
        public interface IHitBoxCallbackObject {
            IHitBoxCallbackObject Prefab(HitBoxComponent hitBoxPrefab);
            IHitBoxCallbackObject Point0(Vector3 p0);
            IHitBoxCallbackObject Point1(Vector3 p1);
            IHitBoxCallbackObject Radius(float width);
            IHitBoxCallbackObject HitLocation(Vector3 hitLocation);
            IHitBoxCallbackObject Tracking(bool tracking);
            IHitBoxCallbackObject FromCollider(CapsuleCollider collider);
            IHitBoxCallbackObject Relative(Transform transform);
            IHitBoxCallbackObject Event(string eventName, Action<HitInfo> callback);
        }
    }

    namespace CharacterProperties {
        public partial class HitBoxBuilder : IHitBoxCallbackObject {
            private HitBoxComponent hitBoxPrefab;

            public Dictionary<string, Action<HitInfo>> eventCallbacks { get; private set; }
            public bool tracking { get; private set; }

            private Vector3 hitLocation = Vector3.up;

            private CapsuleCollider collider;
            private Transform transform;

            private Vector3 point0;
            private Vector3 point1;

            private float radius = -1;

            public IHitBoxCallbackObject Prefab(HitBoxComponent hitBoxPrefab) {
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

            public IHitBoxCallbackObject Event(string eventName, Action<HitInfo> callback) {
                eventCallbacks.Add(eventName, callback);
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
