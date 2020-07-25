using UnityEngine;
using System.Collections;
using System;
using ResonantSpark.Utility;
using ResonantSpark.Gamemode;

namespace ResonantSpark {
    namespace GamemodeStates {
        namespace VersusModeStates {
            public class FightingMode : VersusModeBaseState {
                private GameTimeManager gameTimeManager;
                private float currRoundTime;
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
                    inGameUi.SetMainScreenText("Fight");
                    //uiService.SetValue(element: "mainScreenText", field: "text", value0: "Fight");

                    currRoundTime = versus.GetRoundLength();
                    fgService.EnableControl();
                }

                public override void ExecutePass0(int frameIndex) {
                    if (elapsedTime > 1.2f && !clearedMainScreenText) {
                        inGameUi.HideMainScreenText();
                        //uiService.SetValue(element: "mainScreenText", field: "hide");
                        clearedMainScreenText = true;
                    }

                    versus.CalculateScreenOrientation();
                    versus.SetDisplayTime(currRoundTime);

                    if (currRoundTime < 0) {
                        versus.TimeOutRound();
                    }

                    elapsedTime += gameTimeManager.DeltaTime("frameDelta", "game");
                    currRoundTime -= gameTimeManager.DeltaTime("frameDelta", "game");
                }

                public override void LateExecute(int frameIndex) {
                    versus.RestrictDistance();
                }

                public override void Exit(int frameIndex) {
                    // do nothing
                }
            }
        }
    }
}