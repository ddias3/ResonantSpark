using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Gameplay;

namespace ResonantSpark {
    namespace CharacterStates {
        public class StateHelper : MonoBehaviour {
            protected GameTimeManager gameTime;

            protected FightingGameCharacter fgChar;
            protected FrameEnforcer frame;

            protected Action<string> changeState;

            public void Start() {
                fgChar = gameObject.GetComponentInParent<FightingGameCharacter>();
                frame = GameObject.FindGameObjectWithTag("rspGamemode").GetComponent<FrameEnforcer>();
            }

            public void SetChangeState(Action<string> changeState) {
                this.changeState = changeState;
            }
        }
    }
}