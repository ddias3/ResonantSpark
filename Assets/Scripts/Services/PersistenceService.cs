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
            public List<string> playerSelection { get; private set; }
            public List<int> controllerIndex { get; private set; }

            public void SetCharacterSelected(int index, string characterName) {
                if (playerSelection.Count <= index) {
                    for (int n = playerSelection.Count; n > index; ++n) {
                        playerSelection.Add("");
                    }
                }

                playerSelection[index] = characterName;
            }

            public void SetControllerSelected(int index, int controllerIndex) {
                if (this.controllerIndex.Count <= index) {
                    for (int n = this.controllerIndex.Count; n > index; ++n) {
                        this.controllerIndex.Add(-1);
                    }
                }

                this.controllerIndex[index] = controllerIndex;
            }

            private Persistence() {
                playerSelection = new List<string>();
                controllerIndex = new List<int>();
            }
        }

        public class PersistenceService : MonoBehaviour, IPersistenceService {
            public string gamemodeStr;

            [Tooltip("List of strings with the character names, use in-house names for these")]
            public List<string> characterSelections;

            [Tooltip("The controller index for each character in the game. For an NPC, use number -1. Human players are derived from controllers")]
            public List<int> controllerIndex;

            public GameObject oneOnOneRoundBasedPrefab;

            public GameObject male0BuilderPrefab;
            public GameObject female0BuilderPrefab;

            private Persistence persObj;

            public void Start() {
                if (Persistence.Exists()) {
                    persObj = Persistence.GetPersistence();

                    gamemodeStr         = persObj.gamemode;
                    characterSelections = persObj.playerSelection;
                    controllerIndex     = persObj.controllerIndex;
                }

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

            public GameObject GetSelectedCharacter(int playerIndex) {
                switch (characterSelections[playerIndex]) {
                    case "male0":
                        return male0BuilderPrefab;
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