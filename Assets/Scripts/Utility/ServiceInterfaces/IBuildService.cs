using UnityEngine;

using ResonantSpark.Gameplay;

namespace ResonantSpark {
    namespace Service {
        public interface IBuildService {
            FightingGameCharacter GetBuildingFGChar();
        }
    }
}