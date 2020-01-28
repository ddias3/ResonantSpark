using System;
using System.Collections.Generic;

using ResonantSpark.Gameplay;
using ResonantSpark.Input;
using ResonantSpark.Builder;

namespace ResonantSpark {
    namespace Service {
        public interface IPlayerService {
            void SetUpCharacters();
            void SetMaxPlayers(int maxTotalPlayers);
            void StartCharacterBuild(Action<FightingGameCharacter> fgCharCallback = null);
            void SetCharacterSelected(int playerId, ICharacterBuilder charSelected);
            FightingGameCharacter GetFGChar(int playerIndex);
            void EachFGChar(Action<int, FightingGameCharacter> callback);
            void OneToOthers(Action<int, FightingGameCharacter, List<FightingGameCharacter>> callback);
        }
    }
}