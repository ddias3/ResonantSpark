using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Builder;
using ResonantSpark.Gameplay;
using ResonantSpark.Input;
using ResonantSpark.Gamemode;

namespace ResonantSpark {
    namespace Service {
        public class Persistence {
            private static Persistence pers = null;
            public static bool Exists() { return pers != null; }
            public static Persistence GetPersistence() {
                if (pers == null) {
                    pers = new Persistence();
                }
                return pers;
            }

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

            private Persistence() {
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

        public class PersistenceService : MonoBehaviour, IPersistenceService {
            public string gamemodeStr;

            [Tooltip("List of strings with the character names, use in-house names for these")]
            public List<string> characterSelections;

            [Tooltip("The controller index for each character in the game. For an NPC, use number -1. Human players are derived from controllers")]
            public List<int> controllerIndex;

            public GameObject oneOnOneRoundBasedPrefab;

            public GameObject fightingGameCameraPrefab;

            public GameObject male0BuilderPrefab;
            public GameObject female0BuilderPrefab;
            public GameObject lawrenceBuilderPrefab;

            private Persistence persObj;

            public void Start() {
                //if (Persistence.Exists()) {
                //    persObj = Persistence.GetPersistence();

                //    gamemodeStr         = persObj.gamemode;
                //    characterSelections = persObj.characterSelection;
                //    controllerIndex     = persObj.controllerIndex;
                //}

                EventManager.TriggerEvent<Events.ServiceReady, Type>(typeof(PersistenceService));
            }

            public GameObject GetGamemode() {
                switch (gamemodeStr) {
                    case "oneOnOneRoundBased":
                        return oneOnOneRoundBasedPrefab;
                    case "training":
                        return null;
                    default:
                        return null;
                }
            }

            public GameObject GetCamera() {
                switch (gamemodeStr) {
                    case "oneOnOneRoundBased":
                    case "training":
                        return fightingGameCameraPrefab;
                    default:
                        return null;
                }
            }

            public GameObject GetSelectedCharacter(int playerIndex) {
                switch (characterSelections[playerIndex]) {
                    case "male0":
                        return male0BuilderPrefab;
                    case "lawrence":
                        return lawrenceBuilderPrefab;
                    default:
                        return null;
                }
            }

            public int GetControllerIndex(int playerIndex) {
                return controllerIndex[playerIndex];
            }

            public int GetTotalHumanPlayers() {
                int counter = 0;
                for (int n = 0; n < controllerIndex.Count; ++n) {
                    if (controllerIndex[n] < 0) {
                        ++counter;
                    }
                }
                return counter;
            }
        }
    }
}