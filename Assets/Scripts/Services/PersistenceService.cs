using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.DeviceManagement;

namespace ResonantSpark {
    namespace Service {
        public class PersistenceService : MonoBehaviour, IPersistenceService {
            public string gamemodeStr;

            [Tooltip("List of strings with the character names, use in-house names for these")]
            public List<string> characterSelections;

            public List<GameDeviceMapping> deviceMapping;

            public GameObject versusModePrefab;
            public GameObject trainingModePrefab;

            public GameObject fightingGameCameraPrefab;

            public GameObject lawrenceBuilderPrefab;

            private Persistence persObj;

            public void Start() {
                deviceMapping = new List<GameDeviceMapping>();

                if (Persistence.Exists()) {
                    persObj = Persistence.Get();

                    gamemodeStr = persObj.gamemode;
                    characterSelections = persObj.characterSelection;

                    deviceMapping.Add(persObj.player1);
                    deviceMapping.Add(persObj.player2);
                }
                else {
                    deviceMapping.Add(new GameDeviceMapping(UnityEngine.InputSystem.Keyboard.current));
                    deviceMapping.Add(new GameDeviceMapping(UnityEngine.InputSystem.Gamepad.all[0]));
                }

                EventManager.TriggerEvent<Events.ServiceReady, Type>(typeof(PersistenceService));
            }

            public GameObject GetGamemode() {
                switch (gamemodeStr) {
                    case "versus":
                        return versusModePrefab;
                    case "training":
                        return trainingModePrefab;
                    default:
                        return null;
                }
            }

            public GameObject GetCamera() {
                switch (gamemodeStr) {
                    case "versus":
                    case "training":
                        return fightingGameCameraPrefab;
                    default:
                        return null;
                }
            }

            public GameObject GetSelectedCharacter(int playerIndex) {
                switch (characterSelections[playerIndex]) {
                    case "lawrence":
                        return lawrenceBuilderPrefab;
                    default:
                        return null;
                }
            }

            public GameDeviceMapping GetDeviceMapping(int playerIndex) {
                return deviceMapping[playerIndex];
            }

            public int GetTotalHumanPlayers() {
                return persObj.GetHumanPlayers();
            }
        }
    }
}