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
            public List<int> colorSelections;

            public GameObject versusModePrefab;
            public GameObject trainingModePrefab;

            public GameObject lawrenceBuilderPrefab;

            private Persistence persistence;

            public void Start() {
                if (!Persistence.Exists()) {
                    persistence = Persistence.Get();
                    persistence.SetDeviceMapping(0, new GameDeviceMapping(UnityEngine.InputSystem.Keyboard.current));
                    persistence.SetDeviceMapping(1, new GameDeviceMapping(UnityEngine.InputSystem.Gamepad.all[0]));
                    persistence.SetCharacterSelected(0, characterSelections[0]);
                    persistence.SetCharacterSelected(1, characterSelections[1]);
                    persistence.SetColorSelected(0, colorSelections[0]);
                    persistence.SetColorSelected(1, colorSelections[1]);
                    persistence.SetGamemode(gamemodeStr);
                }
                else {
                    persistence = Persistence.Get();
                }

                EventManager.TriggerEvent<Events.ServiceReady, Type>(typeof(PersistenceService));
            }

            public GameObject GetGamemode() {
                switch (persistence.gamemode) {
                    case "versus":
                        return versusModePrefab;
                    case "training":
                        return trainingModePrefab;
                    default:
                        return null;
                }
            }

            public Dictionary<string, object> GetGamemodeDetails() {
                return persistence.gamemodeDetails;
            }

            public GameObject GetSelectedCharacter(string charSelection) {
                switch (charSelection) {
                    case "lawrence":
                        return lawrenceBuilderPrefab;
                    default:
                        return null;
                }
            }

            public List<GameDeviceMapping> GetDevices() {
                return persistence.devices;
            }

            public int GetHumanPlayers() {
                return persistence.GetHumanPlayers();
            }

            public GameDeviceMapping GetDeviceMapping(int playerIndex) {
                return persistence.devices[playerIndex];
            }
        }
    }
}