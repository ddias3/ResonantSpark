﻿using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Utility;
using ResonantSpark.Gamemode;
using ResonantSpark.Service;

namespace ResonantSpark {
    namespace GamemodeStates {
        public abstract class GamemodeBaseState : BaseState {
            protected OneOnOneRoundBased oneOnOne;
            protected UiService uiService;
            protected FightingGameService fgService;

            public new void Awake() {
                base.Awake();

                oneOnOne = gameObject.GetComponentInParent<OneOnOneRoundBased>();
                uiService = GameObject.FindGameObjectWithTag("rspService").GetComponent<UiService>();
                fgService = GameObject.FindGameObjectWithTag("rspService").GetComponent<FightingGameService>();
            }
        }
    }
}