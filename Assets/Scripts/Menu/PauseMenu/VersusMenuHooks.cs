using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using ResonantSpark.Utility;
using ResonantSpark.DeviceManagement;

namespace ResonantSpark {
    namespace Menu {
        public class VersusMenuHooks : PauseMenu {

            public PauseMenuRunner pauseMenuRunner;
            public PostMatchMenu postMatchMenu;

            public OptionsMenuHooks optionsMenuHooks;

            private MenuManager menuManager;

            private Dictionary<string, UnityEventBase> hooks;

            public void Awake() {
                menuManager = GetComponent<MenuManager>();

                hooks = new Dictionary<string, UnityEventBase> {
                    { "pause", pauseMenuRunner.pauseEvent },
                    { "loadMainMenu", pauseMenuRunner.loadMainMenuEvent },
                    { "postMatchSelect", postMatchMenu.postMatchSelect },
                };
            }

            public override Dictionary<string, UnityEventBase> GetHooks() {
                return hooks;
            }

            public override void Init(int numPlayers, Dictionary<PlayerLabel, GameDeviceMapping> devMaps) {
                menuManager.InitMenus();

                if (numPlayers == 1) {
                    if (devMaps[PlayerLabel.Player1] != null) {
                        postMatchMenu.SetDisplaySingleMenu(1);
                    }
                    else if (devMaps[PlayerLabel.Player2] != null) {
                        postMatchMenu.SetDisplaySingleMenu(2);
                    }
                }
                else if (numPlayers == 2) {
                    postMatchMenu.SetDisplayBothMenus();
                }

                postMatchMenu.SetDeviceMaps(devMaps[PlayerLabel.Player1], devMaps[PlayerLabel.Player2]);
            }

            public void DisplayPostMatchMenu() {
                postMatchMenu.DisplayPostMatchMenu();
            }

            public void HideMenus() {
                pauseMenuRunner.ChangeState("inactive");
            }
        }
    }
}