using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Character {
        public enum AttackPriority : int {
            Lowest = 0,
            LightAttack = 1,
            MediumAttack = 2,
            HeavyAttack = 3,
        }

        public enum AttackForcePriority : int {
            LightAttack,
            MediumAttack,
            HeavyAttack,
        }

        public enum Block : int {
            LOW,
            HIGH,
        }
    }
}
