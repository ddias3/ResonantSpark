using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.CharacterProperties;
using ResonantSpark.Input;

namespace ResonantSpark {
    namespace Character {
        public class CharacterState : MonoBehaviour {
            
            public Orientation orientation { get; private set; }
            public GroundRelation ground { get; private set; }
            public Attack attack { get; private set; }
        }

        public enum Orientation : int {
            SOUTHPAW,
            ORTHODOX,
            BACKTURN,
        }

        public enum GroundRelation : int {
            AMBIGUOUS = 0,
            GROUNDED,
            AIRBORNE,
            DOWN,
        }
    }
}