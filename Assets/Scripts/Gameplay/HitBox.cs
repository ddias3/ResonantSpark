using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Character {
        public class HitBox : MonoBehaviour {

            public Vector3 relativeLocation { get; private set; }

            private LayerMask hurtBox;

            public HitBox Init(Vector3 relLoc) {
                this.relativeLocation = relLoc;
                return this;
            }

            public void Start() {
                hurtBox = LayerMask.NameToLayer("hurtBox");
            }

            public void OnTriggerEnter(Collider other) {
                if (other.gameObject.layer == hurtBox) {
                    Debug.Log("Hitbox hit a hurtbox");
                }
            }
        }
    }
}