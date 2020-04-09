using UnityEngine;
using System.Collections;
using System;
using ResonantSpark.Utility;
using ResonantSpark.Gamemode;

namespace ResonantSpark {
    namespace GamemodeStates {
        public class RoundStartMode : GamemodeBaseState {
            private GameTimeManager gameTimeManager;
            private float elapsedTime;
            private int frameCount;

            private string[] countdownArr = { "3", "2", "1", "GO!" };

            // Use this for initialization
            private new void Awake() {
                base.Awake();
                gameTimeManager = GameObject.FindGameObjectWithTag("rspTime").GetComponent<GameTimeManager>();
                states.Register(this, "roundStartMode");
            }

            public override void Enter(int frameIndex, IState previousState) {
                elapsedTime = 0;
                Debug.Log("Entered Round Start mode state");
                oneOnOne.ResetRound();
                frameCount = 0;
            }

            public override void Execute(int frameIndex) {
                elapsedTime += gameTimeManager.Layer("gameTime");
                int idx = Convert.ToInt32(Math.Truncate(elapsedTime));

                // after start mode animation, switch to fighting mode
                if (idx >= countdownArr.Length) {
                    uiService.SetOpeningText("");
                    changeState(states.Get("fightingMode"));
                }
                else {
                    uiService.SetOpeningText(countdownArr[idx]);
                }

                ++frameCount;
            }

            public override void Exit(int frameIndex) {
                // do nothing
            }
        }
    }
}