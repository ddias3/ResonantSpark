using System;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Service {
        public interface IPhysicsService {
            void Configure<T>(Action<int, T> callbackAsConstrait);
        }
    }
}