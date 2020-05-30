using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Menu {
        public class Cursor3d : MonoBehaviour {
            public Material material;
            public List<MeshRenderer> renderers;
            public Color baseColor;
            public Color selectColor;

            public void Update() {
                //baseColor.a = (maxAlpha - minAlpha) * 0.5f * (Mathf.Sin(glowRate * Time.time * 2 * Mathf.PI) + 1.0f) + minAlpha;
                material.color = baseColor;
            }

            public void Fade() {
                
            }

            public void Highlight(Selectable selectable) {
                //TODO: kick off Animation

                //selectable.transform.position;
            }

            public void Select(Selectable selectable) {
                //TODO: kick off Animation

                //selectable.transform.position;
            }
        }
    }
}