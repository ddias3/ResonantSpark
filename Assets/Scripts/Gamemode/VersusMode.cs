using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using ResonantSpark.Service;
using ResonantSpark.Utility;
using ResonantSpark.Menu;
using ResonantSpark.Input;
using ResonantSpark.DeviceManagement;

namespace ResonantSpark {
    namespace Gamemode {
        public class VersusMode : OneOnOneRoundBased, IHookExpose {
            public GameObject menuPrefab;

            private int player1ControllerIndex;
            private int player2ControllerIndex;

            private Dictionary<string, UnityEventBase> hooks;
            private VersusMenuHooks versusMenu;

            private LoadMainMenu onLoadMainMenuEvent;

            public override void CreateDependencies(AllServices services) {
                base.CreateDependencies(services);

                List<string> gamemodeDetailsPlayers = (List<string>)persService.GetGamemodeDetails()["players"];
                if (gamemodeDetailsPlayers.Count != 2) {
                    throw new Exception("Versus Mode does not have 2 players");
                }

                for (int n = 0; n < gamemodeDetailsPlayers.Count; ++n) {
                    switch (gamemodeDetailsPlayers[n]) {
                        case "player":
                            if (n == 0) {
                                player1ControllerIndex = inputService.CreateController(InputControllerType.Human);
                            }
                            else if (n == 1) {
                                player2ControllerIndex = inputService.CreateController(InputControllerType.Human);
                            }
                            break;
                        case "ai":
                            if (n == 0) {
                                player1ControllerIndex = inputService.CreateController(InputControllerType.None);
                            }
                            else if (n == 1) {
                                player2ControllerIndex = inputService.CreateController(InputControllerType.None);//inputService.CreateController(InputControllerType.AI);
                            }
                            break;
                    }
                }
            }

            public override void SetUp() {
                base.SetUp();

                onLoadMainMenuEvent = new LoadMainMenu();

                hooks = new Dictionary<string, UnityEventBase> {
                    { "loadMainMenu", onLoadMainMenuEvent },
                    { "gameComplete", new VersusGameComplete() },
                };

                versusMenu = GameObject.Instantiate(menuPrefab).GetComponent<VersusMenuHooks>();

                List<GameDeviceMapping> devices = persService.GetDevices();
                versusMenu.Init(persService.GetHumanPlayers(), new Dictionary<PlayerLabel, GameDeviceMapping> {
                    { PlayerLabel.Player1, devices[0] },
                    { PlayerLabel.Player2, devices[1] },
                });
                HookUpMenu(versusMenu.GetHooks());

                unityEventSetPauseMenuWhenReady.Invoke(versusMenu);
                unityEventSetPauseMenuWhenReady.RemoveAllListeners();

                List<string> gamemodeDetailsPlayers = (List<string>)persService.GetGamemodeDetails()["players"];
                for (int n = 0; n < gamemodeDetailsPlayers.Count; ++n) {
                    if (n == 0) {
                        if (gamemodeDetailsPlayers[n] == "player") {
                            HumanInputController p1Controller = (HumanInputController)inputService.GetInputController(player1ControllerIndex);
                            p1Controller.SetDeviceMap(persService.GetDeviceMapping(n));
                            p1Controller.ConnectToCharacter(player1);
                        }
                        else if (gamemodeDetailsPlayers[n] == "ai") {
                            //AiInputController p1Controller = (AiInputController)inputService.GetInputController(player1ControllerIndex);
                            NoneInputController p1Controller = (NoneInputController)inputService.GetInputController(player1ControllerIndex);
                            p1Controller.ConnectToCharacter(player1);
                        }
                    }
                    else if (n == 1) {
                        if (gamemodeDetailsPlayers[n] == "player") {
                            HumanInputController p2Controller = (HumanInputController)inputService.GetInputController(player2ControllerIndex);
                            p2Controller.SetDeviceMap(persService.GetDeviceMapping(n));
                            p2Controller.ConnectToCharacter(player2);
                        }
                        else if (gamemodeDetailsPlayers[n] == "ai") {
                            //AiInputController p2Controller = (AiInputController)inputService.GetInputController(player2ControllerIndex);
                            NoneInputController p2Controller = (NoneInputController)inputService.GetInputController(player2ControllerIndex);
                            p2Controller.ConnectToCharacter(player2);
                        }
                    }
                }

                audioService.SetOptionsMenuHook(versusMenu.optionsMenuHooks);
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