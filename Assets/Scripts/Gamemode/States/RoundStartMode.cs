using UnityEngine;
using System.Collections.Generic;
using System;
using ResonantSpark.Utility;
using ResonantSpark.Gamemode;

namespace ResonantSpark {
    namespace GamemodeStates {
        public class RoundStartMode : GamemodeBaseState {
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

            public override void Enter(int frameIndex, IState previousState) {
                elapsedTime = 0;

                fgService.DisableControl();
                uiService.SetTime(oneOnOne.GetRoundLength());

                oneOnOne.FightingGameMode();
                oneOnOne.ResetRound();
                frameCount = 0;
            }

            public override void ExecutePass0(int frameIndex) {
                if (elapsedTime > elapsedDisplayTime[0]) {
                    changeState(states.Get("fightingMode"));
                }
                else {
                    uiService.SetMainScreenText(countdownArr[0]);
                }

                elapsedTime += gameTimeManager.DeltaTime("frameDelta", "game");
                ++frameCount;
            }

            public override void ExecutePass1(int frameIndex) {
                throw new InvalidOperationException();
            }

            public override void Exit(int frameIndex) {
                uiService.HideMainScreenText();
            }
        }
    }
}