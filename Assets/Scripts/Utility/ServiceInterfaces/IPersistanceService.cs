using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.DeviceManagement;

namespace ResonantSpark {
    namespace Service {
        public interface IPersistenceService {
            GameObject GetGamemode();
            Dictionary<string, object> GetGamemodeDetails();
            GameObject GetSelectedCharacter(string charSelection);
            List<GameDeviceMapping> GetDevices();
            int GetHumanPlayers();
            GameDeviceMapping GetDeviceMapping(int playerIndex);
        }
    }
}