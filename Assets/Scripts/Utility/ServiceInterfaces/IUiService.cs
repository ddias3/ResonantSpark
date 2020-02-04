using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Service {
        public interface IUiService {
            void SetTime(float time);
            void SetMaxHealth(int playerId, int health);
        }
    }
}
