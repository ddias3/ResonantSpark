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

            private string _gamemode;
            private string _playerSelection;
            private string _humanPlayers;

            public string gamemode {
                get { return _gamemode; }
                set {
                    PlayerPrefs.SetString("gamemode", value);
                    _gamemode = value;
                }
            }

            public string playerSelection {
                get { return _playerSelection; }
                set {
                    PlayerPrefs.SetString("playerSelection", value);
                    _playerSelection = value;
                }
            }

            public string humanPlayers {
                get { return _humanPlayers; }
                set {
                    PlayerPrefs.SetString("humanPlayers", value);
                    _humanPlayers = value;
                }
            }

            private Persistence() {
                gamemode = PlayerPrefs.GetString("gamemode");
                playerSelection = PlayerPrefs.GetString("playerSelection");
                humanPlayers = PlayerPrefs.GetString("humanPlayers");
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

            private IGamemode gamemode;
            private Persistence persObj;

            public void Start() {
                if (Persistence.Exists()) {
                    persObj = Persistence.GetPersistence();

                    gamemodeStr = persObj.gamemode;
                }

                switch (gamemodeStr) {
                    case "oneOnOneRoundBased":
                        GameObject newGameMode = GameObject.Instantiate(oneOnOneRoundBasedPrefab);
                        newGameMode.name = "Gamemode";
                        this.gamemode = newGameMode.GetComponent<OneOnOneRoundBased>();
                        break;
                    case "training":
                        break;
                }

                EventManager.TriggerEvent<Events.ServiceReady, Type>(typeof(PersistenceService));
            }

            public IGamemode GetGamemode() {
                return gamemode;
            }

            public void SetGamemode(string gamemode) {
                persObj.gamemode = gamemode;
            }

            public ICharacterBuilder GetSelectedCharacter(int playerIndex) {
                throw new NotImplementedException();
            }
        }
    }
}