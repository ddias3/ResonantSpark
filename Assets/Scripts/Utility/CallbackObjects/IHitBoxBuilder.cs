using System;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Builder {
        public interface IHitBoxCallbackObject {
            IHitBoxCallbackObject Add(Vector3 v0);
            IHitBoxCallbackObject Event(string eventName, Action callback);
        }
    }
}
