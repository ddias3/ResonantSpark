using UnityEngine;

using ResonantSpark.Gameplay;
using ResonantSpark.Input;
using ResonantSpark.Builder;
using ResonantSpark.Gamemode;

namespace ResonantSpark {
    namespace Service {
        public interface IPersistenceService {
            void SetGamemode(string gamemode);
            IGamemode GetGamemode();
            ICharacterBuilder GetSelectedCharacter(int playerIndex);
        }
    }
}