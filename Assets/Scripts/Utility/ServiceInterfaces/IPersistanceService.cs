using UnityEngine;

using ResonantSpark.DeviceManagement;

namespace ResonantSpark {
    namespace Service {
        public interface IPersistenceService {
            GameObject GetGamemode();
            GameObject GetCamera();
            GameObject GetSelectedCharacter(int playerIndex);
            GameDeviceMapping GetDeviceMapping(int playerIndex);
            int GetTotalHumanPlayers();
        }
    }
}