using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;
using ResonantSpark.Utility;
using ResonantSpark.Gamemode;

namespace ResonantSpark {
    namespace GamemodeStates {
        public class GameCompleteMode : GamemodeBaseState {
            private GameTimeManager gameTimeManager;
            private float elapsedTime;

            private int char0Wins;
            private int char1Wins;

            private new void Awake() {
                base.Awake();
                states.Register(this, "gameCompleteMode");

                gameTimeManager = GameObject.FindGameObjectWithTag("rspTime").GetComponent<GameTimeManager>();
            }

            public override void Enter(int frameIndex, IState previousState) {
                uiService.HideMainScreenText();
                SceneManager.LoadScene("Scenes/Menu/MainMenu");
            }

            public override void Execute(int frameIndex) {
                // do nothing
            }

            public override void Exit(int frameIndex) {
                // do nothing
            }
        }
    }
}