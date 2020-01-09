using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Gameplay {
        public class HurtBox : MonoBehaviour {
            public string hurtBoxName;

            private FightingGameCharacter fgChar;
            private LayerMask hurtBox;
            private LayerMask hitBox;

            public void Start() {
                fgChar = GetComponentInParent<FightingGameCharacter>();
                hurtBox = LayerMask.NameToLayer("HurtBox");
                hitBox = LayerMask.NameToLayer("HitBox");
            }

            public void OnTriggerEnter(Collider other) {
                if (other.gameObject.layer == hitBox) {
                    Debug.LogFormat("Hit hurtBox {0}", hurtBoxName);
                    // TODO: Call callback on fgChar;
                    //      fgChar.OnHit();
                }
            }
        }
    }
}