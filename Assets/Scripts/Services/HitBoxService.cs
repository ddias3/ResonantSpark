using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Character;

namespace ResonantSpark {
    namespace Service {
        public class HitBoxService : MonoBehaviour, IHitBoxService {
            public Transform hitBoxEmpty;

            public void Start() {

            }

            public void Active(HitBox hitBox) {
                throw new System.NotImplementedException();
            }

            public HitBox DefaultPrefab() {
                throw new System.NotImplementedException();
            }

            public Transform GetEmptyHoldTransform() {
                throw new System.NotImplementedException();
            }
        }
    }
}