using UnityEngine;
using System.Collections;
using System;
using ResonantSpark.Utility;
using ResonantSpark.Gamemode;

namespace ResonantSpark {
    namespace GamemodeStates {
        public class LoadInMode : GamemodeBaseState {
            private GameTimeManager gameTimeManager;
            private float elapsedTime;

            // Use this for initialization
            private new void Awake() {
                base.Awake();
                states.Register(this, "LoadInMode");

                gameTimeManager = GameObject.FindGameObjectWithTag("rspTime").GetComponent<GameTimeManager>();
            }

            public override void Enter(int frameIndex, MultipassBaseState previousState) {
                elapsedTime = 0;
            }

            public override void ExecutePass0(int frameIndex) {
                if (elapsedTime > 0.5f) {
                    changeState(states.Get("openingMode"));
                }
                elapsedTime += gameTimeManager.DeltaTime("frameDelta", "game");
            }

            public override void LateExecute(int frameIndex) {
                oneOnOne.RestrictDistance();
            }

            public override void Exit(int frameIndex) {
                // do nothing
            }
        }
    }
}
