using UnityEngine;
using System.Collections;
using System;
using ResonantSpark.Utility;
using ResonantSpark.Gamemode;

namespace ResonantSpark {
    namespace GamemodeStates {
        public class LoadInMode : GamemodeBaseState {
            private GameTimeManager gameTimeManager;
            private float elapsedTime;

            // Use this for initialization
            private new void Awake() {
                base.Awake();
                states.Register(this, "LoadInMode");

                gameTimeManager = GameObject.FindGameObjectWithTag("rspTime").GetComponent<GameTimeManager>();
            }

            public override void Enter(int frameIndex, IState previousState) {
                elapsedTime = 0;
            }

            public override void Execute(int frameIndex) {
                if (elapsedTime > 0.5f) {
                    changeState(states.Get("openingMode"));
                }
                elapsedTime += gameTimeManager.Layer("gameTime");
            }

            public override void Exit(int frameIndex) {
                // do nothing
            }
        }
    }
}
