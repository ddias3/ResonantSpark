using UnityEngine;

using ResonantSpark.Gameplay;

namespace ResonantSpark {
    namespace Service {
        public interface IFightingGameService {
            void CreateGamemode();
            Transform GetCharacterRoot(FightingGameCharacter fgChar);
            void RunAnimationState(FightingGameCharacter fgChar, string animationStateName);
        }
    }
}