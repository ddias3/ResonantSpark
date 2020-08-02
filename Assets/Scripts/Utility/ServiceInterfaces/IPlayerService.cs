using System;
using System.Collections.Generic;

using ResonantSpark.Gameplay;
using ResonantSpark.Input;
using ResonantSpark.Builder;

namespace ResonantSpark {
    namespace Service {
        public interface IPlayerService {
            void CreateCharacter(string charSelection, int charColor, Action<FightingGameCharacter> fgCharCallback = null);
            void SetTag(string tag, FightingGameCharacter fgChar);
            FightingGameCharacter GetFGChar(string tag);
            void ForEach(Action<FightingGameCharacter, int> callback);
        }
    }
}