using System;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Service {
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
                if (Persistence.Exists()) {
                    persObj = Persistence.GetPersistence();

                    gamemodeStr         = persObj.gamemode;
                    characterSelections = persObj.characterSelection;
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