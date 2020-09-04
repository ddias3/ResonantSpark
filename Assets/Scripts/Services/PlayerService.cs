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
            private FightingGameService fgService;
            private InputService inputService;

            private List<FightingGameCharacter> fgChars;
            private Dictionary<string, FightingGameCharacter> fgCharTags;
            private Dictionary<int, ICharacterBuilder> selectedFGChars;

            public void Start() {
                buildService = GetComponent<BuildService>();
                persistenceService = GetComponent<PersistenceService>();
                fgService = GetComponent<FightingGameService>();
                inputService = GetComponent<InputService>();

                fgChars = new List<FightingGameCharacter>();
                fgCharTags = new Dictionary<string, FightingGameCharacter>();
                selectedFGChars = new Dictionary<int, ICharacterBuilder>();

                EventManager.TriggerEvent<Events.ServiceReady, Type>(typeof(PlayerService));
            }

            public void CreateCharacter(string charSelection, int charColor, Action<FightingGameCharacter> fgCharCallback = null) {
                GameObject charBuilderPrefab = persistenceService.GetSelectedCharacter(charSelection);
                GameObject newChar = GameObject.Instantiate(charBuilderPrefab);

                FightingGameCharacter builtFGChar = buildService.Build(newChar.GetComponent<ICharacterBuilder>(), charColor);

                fgChars.Add(builtFGChar);

                //HumanInputController humanInputController = inputService.GetInputController(playerId);
                //if (humanInputController != null) {
                //    humanInputController.ConnectToCharacter(builtFGChar);
                //}

                fgCharCallback?.Invoke(builtFGChar);
            }

            public void SetTag(string tag, FightingGameCharacter fgChar) {
                fgCharTags.Add(tag, fgChar);
            }

            public FightingGameCharacter GetFGChar(string tag) {
                return fgCharTags[tag];
            }

            public void ForEach(Action<FightingGameCharacter, int> callback) {
                for (int n = 0; n < fgChars.Count; ++n) {
                    callback(fgChars[n], n);
                }
            }
        }
    }
}