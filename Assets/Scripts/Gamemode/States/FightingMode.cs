using UnityEngine;
using System.Collections;
using System;
using ResonantSpark.Utility;
using ResonantSpark.Gamemode;

namespace ResonantSpark {
    namespace GamemodeStates {
        public class FightingMode : GamemodeBaseState {
            private GameTimeManager gameTimeManager;
            private float elapsedTime;

            // Use this for initialization
            private new void Awake() {
                base.Awake();
                gameTimeManager = GameObject.FindGameObjectWithTag("rspTime").GetComponent<GameTimeManager>();
                states.Register(this, "fightingMode");
            }

            public override void Enter(int frameIndex, IState previousState) {
                elapsedTime = 0;
                Debug.Log("Entered Fighting mode state");
            }

            public override void Execute(int frameIndex) {
                elapsedTime += gameTimeManager.Layer("gameTime");
            }

            public override void Exit(int frameIndex) {
                // do nothing
            }
        }
    }
}