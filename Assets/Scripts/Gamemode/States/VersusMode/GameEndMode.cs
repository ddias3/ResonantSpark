using UnityEngine;
using System.Collections;
using System;
using ResonantSpark.Utility;
using ResonantSpark.Gamemode;

namespace ResonantSpark {
    namespace GamemodeStates {
        namespace VersusModeStates {
            public class GameEndMode : VersusModeBaseState {
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

                    char0Wins = versus.GetCharNumWins(0);
                    char1Wins = versus.GetCharNumWins(1);
                }

                public override void ExecutePass0(int frameIndex) {
                    if (elapsedTime > 4.0f) {
                        changeState(states.Get("gameCompleteMode"));
                    }
                    else if (elapsedTime > 1.2f) {
                        if (char0Wins > char1Wins) {
                            inGameUi.SetMainScreenText("Player 1 Wins");
                            //uiService.SetValue(element: "mainScreenText", field: "text", value0: "Player 1 Wins");
                            // TODO: Player 1 wins.
                        }
                        else if (char1Wins > char0Wins) {
                            inGameUi.SetMainScreenText("Player 2 Wins");
                            //uiService.SetValue(element: "mainScreenText", field: "text", value0: "Player 2 Wins");
                            // TODO: Player 2 wins.
                        }
                        else {
                            inGameUi.SetMainScreenText("Draw");
                            //uiService.SetValue(element: "mainScreenText", field: "text", value0: "Draw");
                            // TODO: Draw.
                        }
                    }

                    elapsedTime += gameTimeManager.DeltaTime("frameDelta", "game");
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