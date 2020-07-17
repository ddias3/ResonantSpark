using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Utility;
using ResonantSpark.Gamemode;
using ResonantSpark.Service;

namespace ResonantSpark {
    namespace GamemodeStates {
        public abstract class OneOnOneRoundBasedBaseState : GamemodeBaseState {
            protected OneOnOneRoundBased oneOnOne;

            public new void Awake() {
                base.Awake();

                oneOnOne = gameObject.GetComponentInParent<OneOnOneRoundBased>();
            }
        }
    }
}