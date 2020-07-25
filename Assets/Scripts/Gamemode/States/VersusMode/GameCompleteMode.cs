using UnityEngine;
using System.Collections;
using System;
using ResonantSpark.Utility;
using ResonantSpark.Gamemode;

namespace ResonantSpark {
    namespace GamemodeStates {
        namespace VersusModeStates {
            public class GameCompleteMode : VersusModeBaseState {
                private GameTimeManager gameTimeManager;
                private float elapsedTime;

                private int char0Wins;
                private int char1Wins;

                private new void Awake() {
                    base.Awake();
                    states.Register(this, "gameCompleteMode");

                    gameTimeManager = GameObject.FindGameObjectWithTag("rspTime").GetComponent<GameTimeManager>();
                }

                public override void Enter(int frameIndex, MultipassBaseState previousState) {
                    inGameUi.HideMainScreenText();
                    //uiService.SetValue(element: "mainScreenText", field: "hide");
                }

                public override void ExecutePass0(int frameIndex) {
                    // do nothing
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