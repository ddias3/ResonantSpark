using UnityEngine;

using ResonantSpark.Gameplay;
using ResonantSpark.Builder;

namespace ResonantSpark {
    namespace Service {
        public interface IBuildService {
            AllServices GetAllServices();
            FightingGameCharacter Build(ICharacterBuilder charBuilder);
            FightingGameCharacter GetBuildingFGChar();
        }
    }
}