using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using ResonantSpark.Gameplay;
using ResonantSpark.Service;
using ResonantSpark.Utility;

namespace ResonantSpark {
    namespace Gamemode {
        public class VersusMode : OneOnOneRoundBased, IHookExpose {
            public GameObject menuPrefab;

            private Dictionary<string, UnityEventBase> hooks;

            public override void SetUp(PlayerService playerService, FightingGameService fgService, UiService uiService) {
                base.SetUp(playerService, fgService, uiService);

                hooks = new Dictionary<string, UnityEventBase> {
                    { "gameComplete", new VersusGameComplete() },
                };

                GameObject versusMenus = GameObject.Instantiate(menuPrefab);
                HookUpMenu(versusMenus.GetComponent<IHookExpose>().GetHooks());
            }

            public Dictionary<string, UnityEventBase> GetHooks() {
                return hooks;
            }

            private void HookUpMenu(Dictionary<string, UnityEventBase> hooks) {
                HookReceive hookReceive = new HookReceive(hooks);

                hookReceive.HookIn<bool>("pause", new UnityAction<bool>(PauseGame));
            }
        }

        public class VersusGameComplete : UnityEvent<int> { }
    }
}