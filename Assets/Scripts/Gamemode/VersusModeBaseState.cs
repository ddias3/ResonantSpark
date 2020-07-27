using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using ResonantSpark.Gamemode;
using ResonantSpark.UI;
using ResonantSpark.Menu;

namespace ResonantSpark {
    namespace GamemodeStates {
        public abstract class VersusModeBaseState : GamemodeBaseState {
            protected VersusMode versus;
            protected InGameUi inGameUi;
            protected VersusMenuHooks versusMenu;

            public new void Awake() {
                base.Awake();

                versus = gameObject.GetComponentInParent<VersusMode>();
                versus.GetInGameUiWhenReady(new UnityAction<InGameUi>((obj) => {
                    inGameUi = obj;
                }));
                versus.GetPauseMenuWhenReady(new UnityAction<Menu.PauseMenu>((obj) => {
                    versusMenu = (VersusMenuHooks) obj;
                }));
            }
        }
    }
}