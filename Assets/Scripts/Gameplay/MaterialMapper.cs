using System;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Gameplay {
        public class MaterialMapper : MonoBehaviour {
            public List<Material> materials;
            public List<int> materialElementIndex;

            public void ForEach(Action<int, Material> callback) {
                for (int n = 0; n < materials.Count; ++n) {
                    callback(materialElementIndex[n], materials[n]);
                }
            }
        }
    }
}