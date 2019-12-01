using UnityEngine;

namespace ResonantSpark {
    namespace Builder {
        public interface ICharacterBuilder {
            void Init();
            GameObject CreateCharacter();
        }
    }
}