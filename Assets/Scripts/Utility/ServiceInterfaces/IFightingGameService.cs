using UnityEngine;

using ResonantSpark.Gameplay;

namespace ResonantSpark {
    namespace Service {
        public interface IFightingGameService {
            void CreateGamemode();
            void SetUpGamemode();
            Transform GetSpawnPoint();
            float GetSpawnPointOffset();
        }
    }
}