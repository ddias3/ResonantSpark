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
            private int numHumanPlayers = 0;
            private Dictionary<int, HumanInputController> humanInputControllerMap;

            public void Start() {
                buildService = GetComponent<BuildService>();
                persistenceService = GetComponent<PersistenceService>();
                inputService = GetComponent<InputService>();

                fgChars = new Dictionary<int, FightingGameCharacter>();
                selectedFGChars = new Dictionary<int, ICharacterBuilder>();
                humanInputControllerMap = new Dictionary<int, HumanInputController>();

                EventManager.TriggerEvent<Events.ServiceReady, Type>(typeof(PlayerService));
            }

            public void SetUpCharacters() {
                SetCharacterSelected(0, persistenceService.GetSelectedCharacter(0));
                SetCharacterSelected(1, persistenceService.GetSelectedCharacter(1));

                SetNumberHumanPlayers(1);

                    // TODO: properly associate characters with controllers.
                AssociateHumanInput(0, inputService.GetInputController(0));
            }

            public void StartCharacterBuild(Action<FightingGameCharacter> fgCharCallback) {
                foreach (KeyValuePair<int, ICharacterBuilder> charBuilder in selectedFGChars) {
                    int playerId = charBuilder.Key;
                    FightingGameCharacter builtFGChar = buildService.Build(charBuilder.Value);

                    fgChars.Add(playerId, builtFGChar);

                    if (humanInputControllerMap.TryGetValue(playerId, out HumanInputController inputController)) {
                        // TODO: Properly associate with the correct controller
                        inputController.SetControllerId(0);
                        inputController.ConnectToCharacter(builtFGChar);
                    }

                    fgCharCallback(builtFGChar);
                }

                EventManager.TriggerEvent<Events.FightingGameCharsReady>();
            }

            public void SetMaxPlayers(int maxTotalPlayers) {
                this.maxTotalPlayers = maxTotalPlayers;
            }

            public void SetNumberHumanPlayers(int numHumanPlayers) {
                this.numHumanPlayers = numHumanPlayers;
            }

            public void AssociateHumanInput(int playerIndex, HumanInputController inputController) {
                humanInputControllerMap.Add(playerIndex, inputController);
            }

            public void SetCharacterSelected(int playerId, ICharacterBuilder charSelected) {
                selectedFGChars.Add(playerId, charSelected);
            }
        }
    }
}