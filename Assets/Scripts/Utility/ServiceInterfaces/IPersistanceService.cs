using UnityEngine;

using ResonantSpark.Gameplay;
using ResonantSpark.Input;
using ResonantSpark.Builder;

namespace ResonantSpark {
    namespace Service {
        public interface IPersistenceService {
            void SetGamemode(string gamemode);
            string GetGamemode();
        }
    }
}