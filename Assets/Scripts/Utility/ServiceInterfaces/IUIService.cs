using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Service {
        public interface IUIService {
            void SetTime(float time);
            void SetMaxHealth(int playerId, int health);
        }
    }
}
