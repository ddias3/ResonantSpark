using System;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Builder {
        public interface IHitBoxBuilder {
            IHitBoxBuilder Add(Vector3 v0);
            IHitBoxBuilder Event(string eventName, Action callback);
        }
    }
}
