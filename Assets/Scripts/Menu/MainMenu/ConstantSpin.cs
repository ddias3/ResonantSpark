using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Menu {
        [RequireComponent(typeof(Rigidbody))]
        public class ConstantSpin : MonoBehaviour {
            public float spinSpeed;

            private Rigidbody rb;

            public void Start() {
                rb = GetComponent<Rigidbody>();
            }

            public void Update() {
                rb.rotation *= (Quaternion.Euler(0, spinSpeed * Time.deltaTime, 0));
            }
        }
    }
}
