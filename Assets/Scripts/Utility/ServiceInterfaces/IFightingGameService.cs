using UnityEngine;

using ResonantSpark.Gameplay;
using ResonantSpark.Gamemode;

namespace ResonantSpark {
    namespace Service {
        public interface IFightingGameService {
            void RegisterGamemode(IGamemode gamemode);
            Transform GetCharacterRoot(FightingGameCharacter fgChar);
            void RunAnimationState(FightingGameCharacter fgChar, string animationStateName);
        }
    }
}