using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Character {
        public enum AttackPriority : int {
            None = 0,

            ContinuousLightAttack,
            ContinuousMediumAttack,
            ContinuousHeavyAttack,

            LightAttack,
            MediumAttack,
            HeavyAttack,
        }

        public enum AttackForcePriority : int {
            LightAttack,
            MediumAttack,
            HeavyAttack,
        }
    }
}
