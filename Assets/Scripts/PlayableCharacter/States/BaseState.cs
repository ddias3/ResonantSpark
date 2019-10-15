using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace CharacterStates {
        public abstract class BaseState : MonoBehaviour, State {

            protected StateDict states;
            protected GameTimeManager gameTime;

            protected FightingGameCharacter fgChar;
            protected FrameEnforcer frame;

            protected bool continueInputSearch = true;

            public void Start() {
                states = gameObject.GetComponentInParent<StateDict>();
                fgChar = gameObject.GetComponentInParent<FightingGameCharacter>();
                frame = GameObject.FindGameObjectWithTag("rspGamemode").GetComponent<FrameEnforcer>();
            }

            public abstract void Enter(int frameIndex, State previousState);
            public abstract void Execute(int frameIndex, Action<State> changeState);
            public abstract void Exit(int frameIndex);
        }
    }
}
