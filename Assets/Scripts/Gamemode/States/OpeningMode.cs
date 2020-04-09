using UnityEngine;
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
                gameTimeManager = GameObject.FindGameObjectWithTag("rspTime").GetComponent<GameTimeManager>();
                states.Register(this, "openingMode");
            }

            public override void Enter(int frameIndex, IState previousState) {
                elapsedTime = 0;
                Debug.Log("Entered Opening mode state");
                uiService.SetOpeningText("Player 1 VS Player 2");

                // todo: remove control from players
            }

            public override void Execute(int frameIndex) {
                elapsedTime += gameTimeManager.Layer("gameTime");

                if (elapsedTime > 3) {
                    uiService.SetOpeningText("");
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
