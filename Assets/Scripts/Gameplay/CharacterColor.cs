using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Gameplay {
        public class CharacterColor : MonoBehaviour {
            public MaterialMapper mapper;
            public SkinnedMeshRenderer model;

            public void SetMapper(MaterialMapper mapper) {
                this.mapper = mapper;
                UpdateMaterials();
            }

            public void UpdateMaterials() {
                Material[] materials = model.materials;

                mapper.ForEach((index, mat) => {
                    materials[index] = mat;
                });

                model.materials = materials;
            }
        }
    }
}