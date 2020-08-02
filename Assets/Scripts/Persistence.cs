using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.DeviceManagement;

namespace ResonantSpark {
    public class Persistence {
        private static Persistence pers = null;
        public static bool Exists() { return pers != null; }
        public static Persistence Get() {
            if (pers == null) {
                pers = new Persistence();
            }
            return pers;
        }

        public bool firstTimeLoad { get; private set; }
        public string gamemode { get; private set; }
        public Dictionary<string, object> gamemodeDetails { get; private set; }
        public List<GameDeviceMapping> devices { get; private set; }
        public Dictionary<string, float> optionValues { get; private set; }
        public string levelPath { get; private set; }
        public List<string> playerInfo { get; private set; }
        public List<string> characterSelection { get; private set; }
        public List<int> colorSelection { get; private set; }

        public void SetCharacterSelected(int index, string characterName) {
            if (characterSelection.Count <= index) {
                for (int n = characterSelection.Count; n < index + 1; ++n) {
                    characterSelection.Add("");
                }
            }

            characterSelection[index] = characterName;
        }

        public void SetColorSelected(int index, int colorIndex) {
            if (colorSelection.Count <= index) {
                for (int n = colorSelection.Count; n < index + 1; ++n) {
                    colorSelection.Add(0);
                }
            }

            colorSelection[index] = colorIndex;
        }

        public void SetPlayerInfo(int index, string data) {
            if (playerInfo.Count <= index) {
                for (int n = playerInfo.Count; n < index + 1; ++n) {
                    playerInfo.Add("");
                }
            }

            playerInfo[index] = data;
        }

        public void SetOptionValue(string key, float value) {
            optionValues[key] = value;
        }

        public void SetGamemode(string gamemode) {
            this.gamemode = gamemode;
        }

        public void SetLevel(string levelPath) {
            this.levelPath = levelPath;
        }

        public float GetOptionValue(string key) {
            return optionValues[key];
        }

        public void SetDeviceMapping(int index, GameDeviceMapping devMap) {
            if (devices.Count <= index) {
                for (int n = devices.Count; n < index + 1; ++n) {
                    devices.Add(null);
                }
            }

            devices[index] = devMap;
        }

        public int GetHumanPlayers() {
            int humanPlayers = 0;
            foreach (string data in playerInfo) {
                if (data == "player") {
                    ++humanPlayers;
                }
            }
            return humanPlayers;
        }

        public bool IsAlreadySetDeviceMap(GameDeviceMapping devMap) {
            return devices.Contains(devMap);
        }

        public void MenuLoaded() {
            firstTimeLoad = false;
        }

        private Persistence() {
            firstTimeLoad = true;
            gamemode = "training";
            gamemodeDetails = new Dictionary<string, object> {
                { "players", playerInfo = new List<string> { "player", "dummy" } },
                { "winRounds", 3 },
                { "characters", new Dictionary<string, object> {
                    { "selection", characterSelection = new List<string> { "lawrence", "lawrence" } },
                    { "color", colorSelection = new List<int> { 0, 1 } },
                } },
            };
            devices = new List<GameDeviceMapping> { null, null };
            optionValues = new Dictionary<string, float> {
                { "masterVolume", 1.0f },
                { "effectsVolume", 1.0f },
                { "musicVolume", 1.0f },
            };
            levelPath = "";
        }
    }
}