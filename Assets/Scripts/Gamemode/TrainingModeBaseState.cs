﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using ResonantSpark.Gamemode;
using ResonantSpark.UI;

namespace ResonantSpark {
    namespace GamemodeStates {
        public abstract class TrainingModeBaseState : GamemodeBaseState {
            protected TrainingMode training;
            protected InGameUi inGameUi;

            public new void Awake() {
                base.Awake();

                training = gameObject.GetComponentInParent<TrainingMode>();
                training.GetInGameUiWhenReady(new UnityAction<InGameUi>((obj) => {
                    inGameUi = obj;
                }));
            }
        }
    }
}