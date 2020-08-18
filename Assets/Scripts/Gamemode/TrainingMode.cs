using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using ResonantSpark.Service;
using ResonantSpark.Utility;
using ResonantSpark.Gameplay;
using ResonantSpark.Menu;
using ResonantSpark.Input;
using ResonantSpark.DeviceManagement;

namespace ResonantSpark {
    namespace Gamemode {
        public class TrainingMode : OneOnOneRoundBased, IHookExpose {
            public GameObject menuPrefab;
            public TrainingDummy trainingDummyPrefab;

            private TrainingDummy dummy = null;

            private int playerControllerIndex;
            private int dummyControllerIndex;

            private int player1ControllerIndex;
            private int player2ControllerIndex;

            private Dictionary<string, UnityEventBase> hooks;
            private TrainingMenuHooks trainingMenu;

            private LoadMainMenu onLoadMainMenuEvent;

            public override void CreateDependencies(AllServices services) {
                base.CreateDependencies(services);

                List<string> gamemodeDetailsPlayers = (List<string>)persService.GetGamemodeDetails()["players"];
                if (gamemodeDetailsPlayers.Count != 2) {
                    throw new Exception("Training Mode does not have 2 players");
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
                        case "dummy":
                            dummyControllerIndex = inputService.CreateController(InputControllerType.Dummy);
                            dummy = GameObject.Instantiate<TrainingDummy>(trainingDummyPrefab, this.transform);
                            if (n == 0) {
                                player1ControllerIndex = dummyControllerIndex;
                            }
                            else if (n == 1) {
                                player2ControllerIndex = dummyControllerIndex;
                            }
                            break;
                    }
                }
            }

            public override void SetUp() {
                base.SetUp();

                onLoadMainMenuEvent = new LoadMainMenu();

                hooks = new Dictionary<string, UnityEventBase> {
                    { "loadMainMenu", onLoadMainMenuEvent }
                };

                Persistence persistence = Persistence.Get();

                trainingMenu = GameObject.Instantiate(menuPrefab).GetComponent<TrainingMenuHooks>();

                List<GameDeviceMapping> devices = persService.GetDevices();
                trainingMenu.Init(persService.GetHumanPlayers(), new Dictionary<PlayerLabel, GameDeviceMapping> {
                    { PlayerLabel.Player1, devices[0] },
                    { PlayerLabel.Player2, devices[1] },
                });
                HookUpMenu(trainingMenu.GetHooks());

                unityEventSetPauseMenuWhenReady.Invoke(trainingMenu);
                unityEventSetPauseMenuWhenReady.RemoveAllListeners();

                List<string> gamemodeDetailsPlayers = (List<string>)persService.GetGamemodeDetails()["players"];
                for (int n = 0; n < gamemodeDetailsPlayers.Count; ++n) {
                    switch (gamemodeDetailsPlayers[n]) {
                        case "dummy":
                            if (dummy.fgChar != null) {
                                throw new Exception("Training Mode has multiple dummies");
                            }
                            dummy.fgChar = playerService.GetFGChar("player" + (n + 1));
                            break;
                    }
                }
                for (int n = 0; n < gamemodeDetailsPlayers.Count; ++n) {
                    if (n == 0) {
                        if (gamemodeDetailsPlayers[n] == "player") {
                            HumanInputController p1Controller = (HumanInputController)inputService.GetInputController(player1ControllerIndex);
                            p1Controller.SetDeviceMap(persService.GetDeviceMapping(n));
                            p1Controller.ConnectToCharacter(player1);
                        }
                        else if (gamemodeDetailsPlayers[n] == "dummy") {
                            dummy.Init((DummyInputController)inputService.GetInputController(dummyControllerIndex), fgService, opponent: player2);
                            DummyInputController p1Controller = (DummyInputController)inputService.GetInputController(player1ControllerIndex);
                            p1Controller.ConnectToCharacter(player1);
                        }
                    }
                    else if (n == 1) {
                        if (gamemodeDetailsPlayers[n] == "player") {
                            HumanInputController p2Controller = (HumanInputController)inputService.GetInputController(player2ControllerIndex);
                            p2Controller.SetDeviceMap(persService.GetDeviceMapping(n));
                            p2Controller.ConnectToCharacter(player2);
                        }
                        else if (gamemodeDetailsPlayers[n] == "dummy") {
                            dummy.Init((DummyInputController)inputService.GetInputController(dummyControllerIndex), fgService, opponent: player1);
                            DummyInputController p2Controller = (DummyInputController)inputService.GetInputController(player2ControllerIndex);
                            p2Controller.ConnectToCharacter(player2);
                        }
                    }
                }

                audioService.SetOptionsMenuHook(trainingMenu.optionsMenuHooks);

                //dummy.Init((DummyInputController) inputService.GetInputController(dummyControllerIndex), fgService, player);

                //DummyInputController dummyController = (DummyInputController)inputService.GetInputController(dummyControllerIndex);
                //dummyController.ConnectToCharacter(dummy.fgChar);

                //HumanInputController playerController = (HumanInputController)inputService.GetInputController(playerControllerIndex);
                //playerController.SetDeviceMap(persService.GetDeviceMapping(playerControllerIndex)); // this is technically wrong.
                //playerController.ConnectToCharacter(player1);
            }

            public Dictionary<string, UnityEventBase> GetHooks() {
                return hooks;
            }

            private void HookUpMenu(Dictionary<string, UnityEventBase> hooks) {
                HookReceive hookReceive = new HookReceive(hooks);

                hookReceive.HookIn<bool>("pause", new UnityAction<bool>(PauseGame));
                if (dummy != null) {
                    hookReceive.HookIn<string>("dummyBlock", new UnityAction<string>(dummy.SetBlockMode));
                }
                hookReceive.HookIn("loadMainMenu", new UnityAction(OnLoadMainMenu));
            }

            private void OnLoadMainMenu() {
                onLoadMainMenuEvent.Invoke();
            }
        }
    }
}