using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Gameplay;
using ResonantSpark.Character;

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
            IHitBoxCallbackObject Event(string eventName, Action<HitBox, IInGameEntity> callback);
        }
    }
}
