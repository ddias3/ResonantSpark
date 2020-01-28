using UnityEngine;

using ResonantSpark.Gameplay;
using ResonantSpark.Input;
using ResonantSpark.Builder;
using ResonantSpark.Gamemode;

namespace ResonantSpark {
    namespace Service {
        public interface IPersistenceService {
            GameObject GetGamemode();
            GameObject GetCamera();
            GameObject GetSelectedCharacter(int playerIndex);
            int GetControllerIndex(int playerIndex);
            int GetTotalHumanPlayers();
        }
    }
}