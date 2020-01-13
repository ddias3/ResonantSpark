using UnityEngine;

using ResonantSpark.Gameplay;
using ResonantSpark.Service;

namespace ResonantSpark {
    namespace CharacterProperties {
        namespace Male0 {
            public class Hadouken : Projectile {

                public float speed;

                public override void RunFrame(IHitBoxService hitBoxServ, IProjectileService projectServ, IAudioService audioServ) {
                    transform.position += speed * transform.forward * gameTime.Layer("gameTime");
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
            }
        }
    }
}