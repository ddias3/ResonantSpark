using UnityEngine;
using System.Collections;
using System;
using ResonantSpark.Utility;
using ResonantSpark.Gamemode;

namespace ResonantSpark {
    namespace GamemodeStates {
        namespace OneOnOneRoundBasedStates {
            public class FightingMode : OneOnOneRoundBasedBaseState {
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
                    uiService.SetMainScreenText("Fight");

                    currRoundTime = oneOnOne.GetRoundLength();
                    fgService.EnableControl();
                }

                public override void ExecutePass0(int frameIndex) {
                    if (elapsedTime > 1.2f && !clearedMainScreenText) {
                        uiService.HideMainScreenText();
                        clearedMainScreenText = true;
                    }

                    oneOnOne.CalculateScreenOrientation();
                    oneOnOne.SetDisplayTime(currRoundTime);

                    if (currRoundTime < 0) {
                        oneOnOne.TimeOutRound();
                    }

                    elapsedTime += gameTimeManager.DeltaTime("frameDelta", "game");
                    currRoundTime -= gameTimeManager.DeltaTime("frameDelta", "game");
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
}