using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Utility;
using ResonantSpark.Gamemode;
using ResonantSpark.Service;

namespace ResonantSpark {
    namespace GamemodeStates {
        public abstract class TrainingModeBaseState : GamemodeBaseState {
            protected TrainingMode training;

            public new void Awake() {
                base.Awake();

                training = gameObject.GetComponentInParent<TrainingMode>();
            }
        }
    }
}