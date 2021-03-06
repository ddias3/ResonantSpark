﻿using UnityEngine;
using System.Collections;
using System;
using ResonantSpark.Utility;
using ResonantSpark.Gamemode;

namespace ResonantSpark {
    namespace GamemodeStates {
        public class OpeningMode : GamemodeBaseState {
            private GameTimeManager gameTimeManager;
            private float elapsedTime;

            public new void Awake() {
                base.Awake();
                states.Register(this, "openingMode");

                gameTimeManager = GameObject.FindGameObjectWithTag("rspTime").GetComponent<GameTimeManager>();
            }

            public override void Enter(int frameIndex, IState previousState) {
                oneOnOne.OpeningMode();
                oneOnOne.ResetGame();
                oneOnOne.ResetRound();

                elapsedTime = 0;
                uiService.SetMainScreenText("Versus");
                fgService.DisableControl();
            }

            public override void Execute(int frameIndex) {
                elapsedTime += gameTimeManager.Layer("gameTime");

                if (elapsedTime > 3.0f) {
                    uiService.HideMainScreenText();

                    // after VS animation, switch to the RoundStart state
                    changeState(states.Get("roundStartMode"));
                }
            }

            public override void Exit(int frameIndex) {
                // do nothing
            }
        }
    }
}