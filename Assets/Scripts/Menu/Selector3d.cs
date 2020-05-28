using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Menu {
        public class Selector3d : MonoBehaviour {
            public Material material;
            public List<MeshRenderer> renderers;
            public Color baseColor;
            public Color selectColor;
            public float glowRate;
            public float maxAlpha = 0.5f;
            public float minAlpha = 0.1f;

            public void Update() {
                baseColor.a = (maxAlpha - minAlpha) * 0.5f * (Mathf.Sin(glowRate * Time.time * 2 * Mathf.PI) + 1.0f) + minAlpha;
                material.color = baseColor;
            }

            public void Select() {
                //TODO: kick off Animation
            }
        }
    }
}