using UnityEngine;
using System.Collections;
using System;
using ResonantSpark.Utility;
using ResonantSpark.Gamemode;

namespace ResonantSpark {
    namespace GamemodeStates {
        public class RoundEndMode : GamemodeBaseState {
            private GameTimeManager gameTimeManager;
            private float elapsedTime;
            private int char0Wins;
            private int char1Wins;

            private new void Awake() {
                base.Awake();
                states.Register(this, "roundEndMode");

                gameTimeManager = GameObject.FindGameObjectWithTag("rspTime").GetComponent<GameTimeManager>();
            }

            public override void Enter(int frameIndex, IState previousState) {
                elapsedTime = 0;
                char0Wins = oneOnOne.GetCharNumWins(0);
                char1Wins = oneOnOne.GetCharNumWins(1);

                uiService.SetRoundWins(0, char0Wins);
                uiService.SetRoundWins(1, char1Wins);
            }

            public override void ExecutePass0(int frameIndex) {
                if (elapsedTime > 2.0f) {
                    if (char0Wins >= 3 || char1Wins >= 3) {
                        changeState(states.Get("gameEndMode"));
                    }
                    else {
                        changeState(states.Get("roundStartMode"));
                    }
                }

                elapsedTime += gameTimeManager.DeltaTime("frameDelta", "game"); ;
            }

            public override void ExecutePass1(int frameIndex) {
                throw new InvalidOperationException();
            }

            public override void Exit(int frameIndex) {
                // do nothing
            }
        }
    }
}
