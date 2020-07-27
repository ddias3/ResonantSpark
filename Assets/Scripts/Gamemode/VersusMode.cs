using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using ResonantSpark.DeviceManagement;
using ResonantSpark.Service;
using ResonantSpark.Utility;
using ResonantSpark.Menu;

namespace ResonantSpark {
    namespace Gamemode {
        public class VersusMode : OneOnOneRoundBased, IHookExpose {
            public GameObject menuPrefab;

            private Dictionary<string, UnityEventBase> hooks;
            private VersusMenuHooks versusMenu;

            private LoadMainMenu onLoadMainMenuEvent;

            public override void SetUp(PlayerService playerService, FightingGameService fgService, UiService uiService) {
                base.SetUp(playerService, fgService, uiService);

                onLoadMainMenuEvent = new LoadMainMenu();

                hooks = new Dictionary<string, UnityEventBase> {
                    { "loadMainMenu", onLoadMainMenuEvent },
                    { "gameComplete", new VersusGameComplete() },
                };

                Persistence persistence = Persistence.Get();                

                versusMenu = GameObject.Instantiate(menuPrefab).GetComponent<VersusMenuHooks>();

                versusMenu.Init(persistence.GetHumanPlayers(), persistence.CreatePlayerDeviceMap());
                HookUpMenu(versusMenu.GetHooks());

                unityEventSetPauseMenuWhenReady.Invoke(versusMenu);
                unityEventSetPauseMenuWhenReady.RemoveAllListeners();
            }

            public Dictionary<string, UnityEventBase> GetHooks() {
                return hooks;
            }

            private void HookUpMenu(Dictionary<string, UnityEventBase> hooks) {
                HookReceive hookReceive = new HookReceive(hooks);

                hookReceive.HookIn("pause", new UnityAction<bool>(PauseGame));
                hookReceive.HookIn("loadMainMenu", new UnityAction(OnLoadMainMenu));
                hookReceive.HookIn("postMatchSelect", new UnityAction<PostMatchOption>(OnPostMatchSelect));
            }

            private void OnLoadMainMenu() {
                onLoadMainMenuEvent.Invoke();
            }

            private void OnPostMatchSelect(PostMatchOption postMatchOption) {
                Persistence pers = Persistence.Get();

                switch (postMatchOption) {
                    case PostMatchOption.MainMenu:
                        onLoadMainMenuEvent.Invoke();
                        break;
                    case PostMatchOption.CharacterSelect:
                        break;
                    case PostMatchOption.LevelSelect:
                        break;
                    case PostMatchOption.Rematch:
                        stateMachine.QueueStateChange(states.Get("openingMode"));
                        versusMenu.HideMenus();
                        PauseGame(false);
                        break;
                }
            }
        }

        public class VersusGameComplete : UnityEvent<int> { }
    }
}