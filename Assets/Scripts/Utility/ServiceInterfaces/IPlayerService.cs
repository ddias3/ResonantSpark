using System;

using ResonantSpark.Gameplay;
using ResonantSpark.Input;
using ResonantSpark.Builder;

namespace ResonantSpark {
    namespace Service {
        public interface IPlayerService {
            void SetUpCharacters();
            void SetMaxPlayers(int maxTotalPlayers);
            void StartCharacterBuild(Action<FightingGameCharacter> fgCharCallback);
            void SetCharacterSelected(int playerId, ICharacterBuilder charSelected);
        }
    }
}