using UnityEngine;

using ResonantSpark.Service;
using ResonantSpark.Gameplay;

namespace ResonantSpark {
    namespace Builder {
        public interface ICharacterBuilder {
            FightingGameCharacter CreateCharacter(AllServices services, int characterColor);
            void Build(AllServices services);
        }
    }
}