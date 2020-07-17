using UnityEngine;
using System.Collections;
using System;
using ResonantSpark.Utility;
using ResonantSpark.Gamemode;

namespace ResonantSpark {
    namespace GamemodeStates {
        namespace TrainingModeStates {
            public class FightingMode : TrainingModeBaseState {
                private GameTimeManager gameTimeManager;
                private float elapsedTime;

                private bool clearedMainScreenText = false;

                private new void Awake() {
                    base.Awake();
                    states.Register(this, "fightingMode");

                    gameTimeManager = GameObject.FindGameObjectWithTag("rspTime").GetComponent<GameTimeManager>();
                }

                public override void Enter(int frameIndex, MultipassBaseState previousState) {
                    elapsedTime = 0.0f;
                    clearedMainScreenText = false;
                    uiService.SetMainScreenText("Fight");

                    fgService.EnableControl();
                }

                public override void ExecutePass0(int frameIndex) {
                    if (elapsedTime > 1.2f && !clearedMainScreenText) {
                        uiService.HideMainScreenText();
                        clearedMainScreenText = true;
                    }

                    training.CalculateScreenOrientation();
                    training.SetDisplayTime(-1);

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