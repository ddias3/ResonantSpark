using System;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Service {
        public class CameraService : MonoBehaviour, ICameraService {
            public void Start() {
                EventManager.TriggerEvent<Events.ServiceReady, Type>(typeof(CameraService));
            }

            public void PredeterminedActions<T0>(string actionName, T0 t0) {
                throw new System.NotImplementedException();
            }

            public void PredeterminedActions<T0, T1>(string actionName, T0 t0, T1 t1) {
                throw new System.NotImplementedException();
            }

            public void PredeterminedActions<T0, T1, T2>(string actionName, T0 t0, T1 t1, T2 t2) {
                throw new System.NotImplementedException();
            }
        }
    }
}
