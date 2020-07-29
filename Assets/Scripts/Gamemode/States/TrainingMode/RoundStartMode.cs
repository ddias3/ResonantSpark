﻿using UnityEngine;
using System.Collections.Generic;
using System;
using ResonantSpark.Utility;
using ResonantSpark.Gamemode;

namespace ResonantSpark {
    namespace GamemodeStates {
        namespace TrainingModeStates {
            public class RoundStartMode : TrainingModeBaseState {
                private GameTimeManager gameTimeManager;
                private float elapsedTime;
                private int frameCount;

                private List<string> countdownArr;
                private List<float> elapsedDisplayTime;

                private new void Awake() {
                    base.Awake();
                    states.Register(this, "roundStartMode");

                    countdownArr = new List<string> { "Ready" };
                    elapsedDisplayTime = new List<float> { 0.8f };

                    gameTimeManager = GameObject.FindGameObjectWithTag("rspTime").GetComponent<GameTimeManager>();
                }

                public override void Enter(int frameIndex, MultipassBaseState previousState) {
                    elapsedTime = 0;

                    fgService.DisableControl();
                    inGameUi.SetNoTime();
                    //uiService.SetValue(element: "roundTimer", field: "noTime");

                    training.FightingGameMode();
                    training.ResetRound();
                    frameCount = 0;
                }

                public override void ExecutePass0(int frameIndex) {
                    if (elapsedTime > elapsedDisplayTime[0]) {
                        changeState(states.Get("fightingMode"));
                    }
                    else {
                        inGameUi.SetMainScreenText(countdownArr[0]);
                        //uiService.SetValue(element: "mainScreenText", field: "text", value0: countdownArr[0]);
                    }

                    elapsedTime += gameTimeManager.DeltaTime("frameDelta", "game");
                    ++frameCount;
                }

                public override void LateExecute(int frameIndex) {
                    training.RestrictDistance();
                }

                public override void Exit(int frameIndex) {
                    inGameUi.HideMainScreenText();
                    //uiService.SetValue(element: "mainScreenText", field: "hide");
                }
            }
        }
    }
}