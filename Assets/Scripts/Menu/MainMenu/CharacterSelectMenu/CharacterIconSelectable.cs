using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Menu {
        public class CharacterIconSelectable : Selectable {
            public MeshRenderer portraitQuad;

            public void Start() {
                eventHandler.On("submit", () => {
                    Debug.Log("Display a Character highlighted/Selected");
                });

                eventHandler.On("deactivate", () => {
                    gameObject.SetActive(false);
                });
                eventHandler.On("activate", () => {
                    gameObject.SetActive(true);
                });
            }

            public void SetMaterial(Material newMaterial) {
                portraitQuad.materials = new Material[] { newMaterial };
            }

            public override float Width() {
                return 0.0f;
            }

            public override Transform GetTransform() {
                return transform;
            }

            public override Vector3 Offset() {
                return Vector3.zero;
            }
        }
    }
}