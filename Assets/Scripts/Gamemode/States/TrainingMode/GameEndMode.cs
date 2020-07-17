using UnityEngine;
using System.Collections;
using System;
using ResonantSpark.Utility;
using ResonantSpark.Gamemode;

namespace ResonantSpark {
    namespace GamemodeStates {
        namespace TrainingModeStates {
            public class GameEndMode : TrainingModeBaseState {
                private GameTimeManager gameTimeManager;
                private float elapsedTime;

                private new void Awake() {
                    base.Awake();
                    states.Register(this, "gameEndMode");

                    gameTimeManager = GameObject.FindGameObjectWithTag("rspTime").GetComponent<GameTimeManager>();
                }

                public override void Enter(int frameIndex, MultipassBaseState previousState) {
                    elapsedTime = 0;
                }

                public override void ExecutePass0(int frameIndex) {
                    if (elapsedTime > 4.0f) {
                        changeState(states.Get("gameCompleteMode"));
                    }

                    elapsedTime += gameTimeManager.DeltaTime("frameDelta", "game");
                }

                public override void LateExecute(int frameIndex) {
                    training.RestrictDistance();
                }

                public override void Exit(int frameIndex) {
                    // do nothing
                }
            }
        }
    }
}