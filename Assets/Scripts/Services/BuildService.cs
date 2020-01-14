using UnityEngine;

using ResonantSpark.Gameplay;
using ResonantSpark.Builder;

namespace ResonantSpark {
    namespace Service {
        public class BuildService : MonoBehaviour, IBuildService {
            public FightingGameCharacter Build(ICharacterBuilder charBuilder) {
                GameObject char0 = charBuilder.CreateCharacter();
                return char0.GetComponent<FightingGameCharacter>();
            }

            public FightingGameCharacter GetBuildingFGChar() {
                throw new System.NotImplementedException();
            }
        }
    }
}