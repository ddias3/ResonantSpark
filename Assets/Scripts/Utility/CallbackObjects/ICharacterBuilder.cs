using UnityEngine;

using ResonantSpark.Service;

namespace ResonantSpark {
    namespace Builder {
        public interface ICharacterBuilder {
            GameObject CreateCharacter(AllServices services);
        }
    }
}