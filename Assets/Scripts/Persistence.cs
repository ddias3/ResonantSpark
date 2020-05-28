using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    public class Persistence {
        private static Persistence pers = null;
        public static bool Exists() { return pers != null; }
        public static Persistence GetPersistence() {
            if (pers == null) {
                pers = new Persistence();
            }
            return pers;
        }

        public bool firstTimeLoad { get; private set; }
        public string gamemode { get; set; }
        public Dictionary<string, float> optionValues { get; private set; }
        public List<string> characterSelection { get; private set; }
        public List<int> colorSelection { get; private set; }
        public List<int> controllerIndex { get; private set; }

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

        public void SetControllerSelected(int index, int controllerIndex) {
            if (this.controllerIndex.Count <= index) {
                for (int n = this.controllerIndex.Count; n < index + 1; ++n) {
                    this.controllerIndex.Add(-1);
                }
            }

            this.controllerIndex[index] = controllerIndex;
        }

        public void SetOptionValue(string key, float value) {
            optionValues[key] = value;
        }

        public float GetOptionValue(string key) {
            return optionValues[key];
        }

        public void MenuLoaded() {
            firstTimeLoad = false;
        }

        private Persistence() {
            firstTimeLoad = true;
            optionValues = new Dictionary<string, float> {
                { "masterVolume", 1.0f },
                { "effectsVolume", 1.0f },
                { "musicVolume", 1.0f },
            };
            characterSelection = new List<string>();
            colorSelection = new List<int> { 0, 0 };
            controllerIndex = new List<int> { 0, -1 };
        }
    }
}