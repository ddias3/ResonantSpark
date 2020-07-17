using UnityEngine;
using System.Collections;
using System;
using ResonantSpark.Utility;
using ResonantSpark.Gamemode;

namespace ResonantSpark {
    namespace GamemodeStates {
        namespace OneOnOneRoundBasedStates {
            public class GameEndMode : OneOnOneRoundBasedBaseState {
                private GameTimeManager gameTimeManager;
                private float elapsedTime;

                private int char0Wins;
                private int char1Wins;

                private new void Awake() {
                    base.Awake();
                    states.Register(this, "gameEndMode");

                    gameTimeManager = GameObject.FindGameObjectWithTag("rspTime").GetComponent<GameTimeManager>();
                }

                public override void Enter(int frameIndex, MultipassBaseState previousState) {
                    elapsedTime = 0;

                    char0Wins = oneOnOne.GetCharNumWins(0);
                    char1Wins = oneOnOne.GetCharNumWins(1);
                }

                public override void ExecutePass0(int frameIndex) {
                    if (elapsedTime > 4.0f) {
                        changeState(states.Get("gameCompleteMode"));
                    }
                    else if (elapsedTime > 1.2f) {
                        if (char0Wins > char1Wins) {
                            uiService.SetMainScreenText("Player 1 Wins");
                        }
                        else if (char1Wins > char0Wins) {
                            uiService.SetMainScreenText("Player 2 Wins");
                        }
                        else {
                            uiService.SetMainScreenText("Draw");
                        }
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
}