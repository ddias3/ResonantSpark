using UnityEngine;

using ResonantSpark.Service;
using ResonantSpark.Gameplay;

namespace ResonantSpark {
    namespace Builder {
        public interface ICharacterBuilder {
            FightingGameCharacter CreateCharacter(AllServices services);
            void Build(AllServices services);
        }
    }
}