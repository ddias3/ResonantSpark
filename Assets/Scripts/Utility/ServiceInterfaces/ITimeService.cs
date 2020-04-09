using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Service {
        public interface ITimeService {
            void PredeterminedActions<T0>(string actionName, T0 t0);
            void PredeterminedActions<T0, T1>(string actionName, T0 t0, T1 t1);
            void PredeterminedActions<T0, T1, T2>(string actionName, T0 t0, T1 t1, T2 t2);
        }
    }
}