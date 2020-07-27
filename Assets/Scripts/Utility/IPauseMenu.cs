using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.DeviceManagement;

namespace ResonantSpark {
    namespace Utility {
        public interface IPauseMenu {
            void Init(int numPlayers, Dictionary<PlayerLabel, GameDeviceMapping> devMaps);
        }
    }
}