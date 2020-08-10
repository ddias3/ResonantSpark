using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Gameplay;

namespace ResonantSpark {
    namespace Service {
        public class CameraService : MonoBehaviour, ICameraService, IPredeterminedActions {
            public void Start() {
                EventManager.TriggerEvent<Events.ServiceReady, Type>(typeof(CameraService));
            }

            public void PredeterminedActions(string actionName) {
                throw new System.NotImplementedException();
            }

            public void PredeterminedActions(string actionName, params object[] objs) {
                throw new System.NotImplementedException();
            }
        }
    }
}
