using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ResonantSpark {
    namespace UI {
        public abstract class GameUiElement : MonoBehaviour {

            protected GameTimeManager gameTime;

            protected Dictionary<string, UnityAction> fieldMap;
            protected Dictionary<string, UnityAction<object>> fieldMap1Value;
            protected Dictionary<string, UnityAction<object, object>> fieldMap2Value;

            public void Awake() {
                gameTime = GameObject.FindGameObjectWithTag("rspTime").GetComponent<GameTimeManager>();
            }

                // TODO: Look into this as a possibility.
            //public abstract void Serialize();

            public abstract void SetValue(string field);
            public abstract void SetValue(string field, object value0);
            public abstract void SetValue(string field, object value0, object value1);
            //public abstract void SetValue<T0, T1, T2>(string field, T0 value0, T1 value1, T2 value2);
            //public abstract void SetValue<T0, T1, T2, T3>(string field, T0 value0, T1 value1, T2 value2, T3 value3);
        }
    }
}