using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.UI;

namespace ResonantSpark {
    namespace Service {
        public class UiService : MonoBehaviour, IUiService {

            private Dictionary<string, GameUiElement> elements = new Dictionary<string, GameUiElement>();

            public void Start() {
                EventManager.TriggerEvent<Events.ServiceReady, Type>(typeof(UiService));
            }

            public void RegisterElement(string id, GameUiElement element) {
                elements.Add(id, element);
                Debug.Log("Registering UI Element: " + id);
            }

            public void SetValue(string element, string field) {
                elements[element].SetValue(field);
            }

            public void SetValue<T0>(string element, string field, T0 value0) {
                elements[element].SetValue(field, value0);
            }

            public void SetValue<T0, T1>(string element, string field, T0 value0, T1 value1) {
                elements[element].SetValue(field, value0, value1);
            }

            //public void SetValue<T0, T1, T2>(string element, string field, T0 value0, T1 value1, T2 value2) {
            //    elements[element].SetValue(field, value0, value1, value2);
            //}

            //public void SetValue<T0, T1, T2, T3>(string element, string field, T0 value0, T1 value1, T2 value2, T3 value3) {
            //    elements[element].SetValue(field, value0, value1, value2, value3);
            //}
        }
    }
}