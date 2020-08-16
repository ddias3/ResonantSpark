using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Gameplay;

namespace ResonantSpark {
    namespace SimplifiedPhysics {
        public class FGCharPhysicsFudge : GlobalConstraint {

            public float pushOutVelocity;

            private FightingGameCharacter char0;
            private FightingGameCharacter char1;

            private bool collision;
            private Vector3 offset0;
            private Vector3 offset1;

            public void SetRigidbodyFGs(FightingGameCharacter char0, FightingGameCharacter char1) {
                this.char0 = char0;
                this.char1 = char1;
            }

            public override void Preprocess(float deltaTime) {
                Bounds bounds0 = char0.rigidFG.collider.bounds;
                Bounds bounds1 = char1.rigidFG.collider.bounds;
                collision = false;

                if (bounds0.Intersects(bounds1)) {
                    SphereCollider sph0 = (SphereCollider)char0.rigidFG.collider;
                    SphereCollider sph1 = (SphereCollider)char1.rigidFG.collider;
                    if (Collision.SphereSphereIntersection(sph0, sph1, out Vector3 collisionPoint)) {
                        collision = true;

                        Debug.Log("intersect");

                        Vector3 sph0Center = sph0.transform.rotation * (sph0.transform.position + sph0.center);
                        Vector3 sph1Center = sph1.transform.rotation * (sph1.transform.position + sph1.center);

                        Vector3 sph0CollisionDir = collisionPoint - sph0Center;
                        Vector3 sph1CollisionDir = collisionPoint - sph1Center;
                        offset0 = (sph0.radius - sph0CollisionDir.magnitude) * sph0CollisionDir.normalized;
                        offset1 = (sph1.radius - sph1CollisionDir.magnitude) * sph1CollisionDir.normalized;
                    }
                }
            }

            public override void Postprocess(float deltaTime) {
                if (!collision) return;

                offset0.y = 0.0f;
                offset1.y = 0.0f;

                Matrix4x4 M = new Matrix4x4(new Vector4(), new Vector4(), new Vector4(), new Vector4());

                char0.AddRelativeVelocity(Gameplay.VelocityPriority.Movement, -offset0.normalized * pushOutVelocity);
                char1.AddRelativeVelocity(Gameplay.VelocityPriority.Movement, -offset1.normalized * pushOutVelocity);
            }
        }
    }
}