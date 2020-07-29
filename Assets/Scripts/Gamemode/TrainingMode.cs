using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using ResonantSpark.Service;
using ResonantSpark.Utility;
using ResonantSpark.Gameplay;
using ResonantSpark.Menu;

namespace ResonantSpark {
    namespace Gamemode {
        public class TrainingMode : OneOnOneRoundBased, IHookExpose {
            public GameObject menuPrefab;

            private FightingGameCharacter player;
            private TrainingDummy dummy;

            private Dictionary<string, UnityEventBase> hooks;
            private TrainingMenuHooks trainingMenu;

            private LoadMainMenu onLoadMainMenuEvent;

            public override void SetUp(PlayerService playerService, FightingGameService fgService, UiService uiService) {
                base.SetUp(playerService, fgService, uiService);

                onLoadMainMenuEvent = new LoadMainMenu();

                hooks = new Dictionary<string, UnityEventBase> {
                    { "loadMainMenu", onLoadMainMenuEvent }
                };

                Persistence persistence = Persistence.Get();

                trainingMenu = GameObject.Instantiate(menuPrefab).GetComponent<TrainingMenuHooks>();

                trainingMenu.Init(persistence.GetHumanPlayers(), persistence.CreatePlayerDeviceMap());
                HookUpMenu(trainingMenu.GetHooks());

                unityEventSetPauseMenuWhenReady.Invoke(trainingMenu);
                unityEventSetPauseMenuWhenReady.RemoveAllListeners();
            }

            public Dictionary<string, UnityEventBase> GetHooks() {
                return hooks;
            }

            private void HookUpMenu(Dictionary<string, UnityEventBase> hooks) {
                HookReceive hookReceive = new HookReceive(hooks);

                hookReceive.HookIn<bool>("pause", new UnityAction<bool>(PauseGame));
                //hookReceive.HookIn<string>("dummyBlock", new UnityAction<string>(dummy.SetBlockMode));
                hookReceive.HookIn("loadMainMenu", new UnityAction(OnLoadMainMenu));
            }

            private void OnLoadMainMenu() {
                onLoadMainMenuEvent.Invoke();
            }
        }
    }
}