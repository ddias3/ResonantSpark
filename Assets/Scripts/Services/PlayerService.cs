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

            private int maxTotalPlayers = 0;

            private Dictionary<int, ICharacterBuilder> selectedFGChars;
            private int numHumanPlayers = 0;
            private Dictionary<int, HumanInputController> humanInputControllerMap;

            public void Start() {
                buildService = GetComponent<BuildService>();

                selectedFGChars = new Dictionary<int, ICharacterBuilder>();
                humanInputControllerMap = new Dictionary<int, HumanInputController>();

                EventManager.TriggerEvent<Events.ServiceReady, Type>(typeof(PlayerService));
            }

            public void StartCharacterBuild(Action<FightingGameCharacter> fgCharCallback) {
                foreach (KeyValuePair<int, ICharacterBuilder> charBuilder in selectedFGChars) {
                    int playerId = charBuilder.Key;
                    FightingGameCharacter builtFGChar = buildService.Build(charBuilder.Value);

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