using UnityEngine;

using ResonantSpark.Gameplay;
using ResonantSpark.Service;
using ResonantSpark.Particle;

namespace ResonantSpark {
    namespace CharacterProperties {
        namespace Lawrence {
            public class Hadouken : Projectile {

                public float speed;

                public override void RunFrame() {
                    transform.position += speed * transform.forward * gameTime.DeltaTime("frameDelta", "game");
                }

                public void OnTriggerEnter(Collider other) {
                    if (other.gameObject.layer == hurtBox) {
                        // TODO: Call an OnHurtBox
                        Debug.Log("HurtBox Collided");

                        OnHurtBoxEnter();
                    }
                    else if (other.gameObject.layer == hitBox) {
                        // TODO: Call an OnHitBox

                        OnHitBoxEnter();
                    }
                    else if (other.gameObject.layer == outOfBounds) {
                        // TODO: Call an OnOutOfBounds
                        Debug.Log("HurtBox Collided");

                        OnOutOfBoundsEnter();
                    }
                }

                public override ParticleEffect DestroyedParticle() {
                    throw new System.NotImplementedException();
                }

                public override string HitBoxEventType(HitBox hitBox) {
                    return "onHitProjectile";
                }

                public override void AddSelf() {
                    throw new System.NotImplementedException();
                }

                public override void RemoveSelf() {
                    throw new System.NotImplementedException();
                }

                public override ComboState GetComboState() {
                    throw new System.NotImplementedException();
                }
            }
        }
    }
}