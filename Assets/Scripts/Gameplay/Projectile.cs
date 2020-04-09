using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Utility;
using ResonantSpark.Service;
using ResonantSpark.Particle;

namespace ResonantSpark {
    namespace Gameplay {
        public abstract class Projectile : InGameEntity, IResource, IInGamePerformable {

            public bool active { get; private set; }
            public int health { get; private set; }

            // TODO: hook this up
            protected GameTimeManager gameTime;

            protected LayerMask hurtBox;
            protected LayerMask hitBox;
            protected LayerMask outOfBounds;

            private Vector3 storeLocation;
            private int frameStart;

            public void Start() {
                active = false;
                frameStart = -1;
                storeLocation = transform.position;

                hurtBox = LayerMask.NameToLayer("HurtBox");
                hitBox = LayerMask.NameToLayer("HitBox");
                outOfBounds = LayerMask.NameToLayer("OutOfBounds");
            }

            public void FireProjectile() {
                active = true;

                //TODO: Add self to the active projectile list.
            }

            protected void OnHurtBoxEnter() {
                // TODO: Hook this up
            }

            protected void OnHitBoxEnter() {
                // TODO: Hook this up
            }

            protected void OnOutOfBoundsEnter() {
                // TODO: Deactivate self and readd self to resource recycler
            }

            public void Store(Vector3 storeLocation) {
                active = false;
                this.storeLocation = storeLocation;
            }

            public bool IsActive() {
                return active;
            }

            public void Activate() {
                throw new System.NotImplementedException();
            }

            public void Deactivate() {
                throw new System.NotImplementedException();
            }

            public void StartPerformable(int frameIndex) {
                throw new System.NotImplementedException();
            }

            public bool IsCompleteRun() {
                throw new System.NotImplementedException();
            }

            public void FrameCountSanityCheck(int frameIndex) {
                throw new System.NotImplementedException();
            }

            public abstract void RunFrame();
            public abstract ParticleEffect DestroyedParticle();
        }
    }
}