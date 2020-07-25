using System.Collections.Generic;
using UnityEngine.Events;

namespace ResonantSpark {
    namespace Utility {
        public interface IHookExpose {
            Dictionary<string, UnityEventBase> GetHooks();
        }
    }
}
