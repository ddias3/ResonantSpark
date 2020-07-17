using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Utility;
using ResonantSpark.Gamemode;
using ResonantSpark.Service;

namespace ResonantSpark {
    namespace GamemodeStates {
        public abstract class GamemodeBaseState : MultipassBaseState {
            protected UiService uiService;
            protected FightingGameService fgService;

            public new void Awake() {
                base.Awake();

                uiService = GameObject.FindGameObjectWithTag("rspService").GetComponent<UiService>();
                fgService = GameObject.FindGameObjectWithTag("rspService").GetComponent<FightingGameService>();
            }

            public override void ExecutePass1(int frameIndex) {
                throw new InvalidOperationException("Game mode states do not have a second execute pass");
            }
        }
    }
}