using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using ResonantSpark.Utility;
using ResonantSpark.DeviceManagement;

namespace ResonantSpark {
    namespace Menu {
        public class LoadMainMenu : UnityEvent { }

        public abstract class PauseMenu : MonoBehaviour, IPauseMenu, IHookExpose {
            public abstract Dictionary<string, UnityEventBase> GetHooks();
            public abstract void Init(int numPlayers, Dictionary<PlayerLabel, GameDeviceMapping> devMaps);
        }
    }
}