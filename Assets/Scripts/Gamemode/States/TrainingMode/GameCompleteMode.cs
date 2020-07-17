using UnityEngine;
using System.Collections;
using System;
using ResonantSpark.Utility;
using ResonantSpark.Gamemode;

namespace ResonantSpark {
    namespace GamemodeStates {
        namespace TrainingModeStates {
            public class GameCompleteMode : TrainingModeBaseState {
                private GameTimeManager gameTimeManager;
                private float elapsedTime;

                private new void Awake() {
                    base.Awake();
                    states.Register(this, "gameCompleteMode");

                    gameTimeManager = GameObject.FindGameObjectWithTag("rspTime").GetComponent<GameTimeManager>();
                }

                public override void Enter(int frameIndex, MultipassBaseState previousState) {
                    uiService.HideMainScreenText();
                }

                public override void ExecutePass0(int frameIndex) {
                    // do nothing
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