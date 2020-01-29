using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Gameplay {
        public interface IInGameEntity {
            void GetHitBy(HitBox hitBox);
            void AddSelf();
            void RemoveSelf();
        }
    }
}