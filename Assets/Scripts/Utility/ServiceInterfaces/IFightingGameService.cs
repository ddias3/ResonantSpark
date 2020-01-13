using UnityEngine;

using ResonantSpark.Gameplay;

namespace ResonantSpark {
    namespace Service {
        public interface IFightingGameService {
            Transform GetCharacterRoot(FightingGameCharacter fgChar);
        }
    }
}