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
        public string levelPath { get; private set; }
        public Dictionary<string, float> optionValues { get; private set; }
        public List<string> characterSelection { get; private set; }
        public List<int> colorSelection { get; private set; }

        public GameDeviceMapping player1 { get; private set; }
        public GameDeviceMapping player2 { get; private set; }

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

        public void SetDeviceMappingP1(GameDeviceMapping devMap) {
            player1 = devMap;
        }

        public void SetDeviceMappingP2(GameDeviceMapping devMap) {
            player2 = devMap;
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

        public int GetHumanPlayers() {
            int humanPlayers = 0;
            if (player1 != null) {
                humanPlayers += 1;
            }
            if (player2 != null) {
                humanPlayers += 1;
            }
            return humanPlayers;
        }

        public void MenuLoaded() {
            firstTimeLoad = false;
        }

        private Persistence() {
            firstTimeLoad = true;
            gamemode = "training";
            optionValues = new Dictionary<string, float> {
                { "masterVolume", 1.0f },
                { "effectsVolume", 1.0f },
                { "musicVolume", 1.0f },
            };
            characterSelection = new List<string> { "lawrence", "lawrence" };
            colorSelection = new List<int> { 0, 0 };
        }
    }
}