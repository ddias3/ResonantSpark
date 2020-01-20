using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Builder;
using ResonantSpark.Gameplay;
using ResonantSpark.Input;

namespace ResonantSpark {
    namespace Service {
        public class PlayerService : MonoBehaviour, IPlayerService {

            private BuildService buildService;
            private PersistenceService persistenceService;
            private InputService inputService;

            private int maxTotalPlayers = 0;

            private Dictionary<int, FightingGameCharacter> fgChars;
            private Dictionary<int, ICharacterBuilder> selectedFGChars;

            public void Start() {
                buildService = GetComponent<BuildService>();
                persistenceService = GetComponent<PersistenceService>();
                inputService = GetComponent<InputService>();

                fgChars = new Dictionary<int, FightingGameCharacter>();
                selectedFGChars = new Dictionary<int, ICharacterBuilder>();

                EventManager.TriggerEvent<Events.ServiceReady, Type>(typeof(PlayerService));
            }

            public void SetUpCharacters() {
                for (int n = 0; n < maxTotalPlayers; ++n) {
                    GameObject charBuilderPrefab = persistenceService.GetSelectedCharacter(n);
                    GameObject newGameMode = GameObject.Instantiate(charBuilderPrefab);
                    newGameMode.name = "Player" + n;

                    SetCharacterSelected(n, newGameMode.GetComponent<ICharacterBuilder>());
                }
            }

            public void StartCharacterBuild(Action<FightingGameCharacter> fgCharCallback = null) {
                foreach (KeyValuePair<int, ICharacterBuilder> charBuilder in selectedFGChars) {
                    int playerId = charBuilder.Key;
                    FightingGameCharacter builtFGChar = buildService.Build(charBuilder.Value);

                    fgChars.Add(playerId, builtFGChar);

                    HumanInputController humanInputController = inputService.GetInputController(playerId);
                    if (humanInputController != null) {
                        humanInputController.ConnectToCharacter(builtFGChar);
                    }

                    fgCharCallback?.Invoke(builtFGChar);
                }
            }

            public void SetMaxPlayers(int maxTotalPlayers) {
                this.maxTotalPlayers = maxTotalPlayers;
            }

            public int GetMaxPlayers() {
                return maxTotalPlayers;
            }

            public void SetCharacterSelected(int playerId, ICharacterBuilder charSelected) {
                selectedFGChars.Add(playerId, charSelected);
            }

            public FightingGameCharacter GetFGChar(int playerIndex) {
                return fgChars[playerIndex];
            }
        }
    }
}