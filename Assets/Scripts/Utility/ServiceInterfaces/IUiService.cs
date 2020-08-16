using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.UI;

namespace ResonantSpark {
    namespace Service {
        public interface IUiService {
            void RegisterElement(string id, GameUiElement element);
            void SetValue(string element, string field);
            void SetValue<T0>(string element, string field, T0 value0);
            void SetValue<T0, T1>(string element, string field, T0 value0, T1 value1);
        }
    }
}
