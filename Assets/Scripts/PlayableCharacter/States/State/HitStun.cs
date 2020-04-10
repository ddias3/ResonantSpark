using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Input.Combinations;
using ResonantSpark.Character;
using ResonantSpark.Gameplay;
using ResonantSpark.Input;

namespace ResonantSpark {
    namespace CharacterStates {
        public abstract class HitStun : CharacterBaseState {

            public int testLength = 20;

            protected Utility.Tracker tracker;

            public new void Awake() {
                base.Awake();
                tracker = new Utility.Tracker(testLength, new Action(OnComplete));
            }

            protected void OnComplete() {
                // TODO: Figure out what to do.
            }
        }
    }
}