using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using ResonantSpark.Gamemode;
using ResonantSpark.UI;

namespace ResonantSpark {
    namespace GamemodeStates {
        public abstract class VersusModeBaseState : GamemodeBaseState {
            protected VersusMode versus;
            protected InGameUi inGameUi;

            public new void Awake() {
                base.Awake();

                versus = gameObject.GetComponentInParent<VersusMode>();
                versus.GetInGameUiWhenReady(new UnityAction<InGameUi>((obj) => {
                    inGameUi = obj;
                }));
            }
        }
    }
}