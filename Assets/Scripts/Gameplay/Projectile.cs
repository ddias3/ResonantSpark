using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Gameplay {
        public class Projectile : MonoBehaviour {

            public bool active { get; private set; }

                // TODO: hook this up
            protected GameTimeManager gameTime;

            protected LayerMask hurtBox;
            protected LayerMask hitBox;
            protected LayerMask outOfBounds;

            private Vector3 storeLocation;

            public void Start() {
                active = false;
                storeLocation = transform.position;

                hurtBox = LayerMask.NameToLayer("HurtBox");
                hitBox = LayerMask.NameToLayer("HitBox");
                outOfBounds = LayerMask.NameToLayer("OutOfBounds");
            }

            public void FireProjectile() {
                active = true;

                //TODO: Add self to the active projectile list.
            }

            public void Store(Vector3 storeLocation) {
                active = false;
                this.storeLocation = storeLocation;
            }

                //TODO: Hook this up to the frameEnforcer
                //  It should only be called when the projectile is active.
            public void NotUpdate() {

            }
        }
    }
}