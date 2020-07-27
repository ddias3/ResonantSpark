using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using ResonantSpark.Utility;
using ResonantSpark.DeviceManagement;

namespace ResonantSpark {
    namespace Menu {
        public class TrainingMenuHooks : PauseMenu {

            public ScrollSelect dummyBlock;
            public PauseMenuRunner pauseMenuRunner;

            private MenuManager menuManager;

            private Dictionary<string, UnityEventBase> hooks;

            public void Awake() {
                menuManager = GetComponent<MenuManager>();

                dummyBlock.options = new List<ScrollSelectOption<string>> {
                    new ScrollSelectOption<string> { name = "None", callbackData = "none" },
                    new ScrollSelectOption<string> { name = "All", callbackData = "all" },
                    new ScrollSelectOption<string> { name = "Low", callbackData = "low" },
                    new ScrollSelectOption<string> { name = "High", callbackData = "high" },
                };
                dummyBlock.AddListener(new UnityAction<string>(DummyBlock));

                hooks = new Dictionary<string, UnityEventBase> {
                    { "dummyBlock", dummyBlock.scrollEvent },
                    { "pause", pauseMenuRunner.pauseEvent },
                    { "loadMainMenu", pauseMenuRunner.loadMainMenuEvent },
                };
            }

            public void DummyBlock(string data) {
                Debug.Log("Dummy Block: " + data);
            }

            public override Dictionary<string, UnityEventBase> GetHooks() {
                return hooks;
            }

            public override void Init(int numPlayers, Dictionary<PlayerLabel, GameDeviceMapping> devMaps) {
                menuManager.InitMenus();

                if (numPlayers == 1) {
                    Debug.Log("Number of players == 1");
                }
                else if (numPlayers == 2) {
                    Debug.Log("Number of players == 2");
                }
            }
        }
    }
}