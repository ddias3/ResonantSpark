using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Gameplay;

namespace ResonantSpark {
    namespace SimplifiedPhysics {
        public class FGCharPhysicsFudge : GlobalConstraint {

            public float bias = 1.0f;

            private FightingGameCharacter char0;
            private FightingGameCharacter char1;

            private bool collision;
            private Vector3 offset;

            private Vector4 jacobian;

            public void SetRigidbodyFGs(FightingGameCharacter char0, FightingGameCharacter char1) {
                this.char0 = char0;
                this.char1 = char1;
            }

            public override void Preprocess(float deltaTime) {
                collision = false;
                if (!(char0.Grounded(out Vector3 vec0) && char1.Grounded(out Vector3 vec1))) {
                    return;
                }

                Bounds bounds0 = char0.rigidFG.collider.bounds;
                Bounds bounds1 = char1.rigidFG.collider.bounds;

                if (bounds0.Intersects(bounds1)) {
                    SphereCollider sph0 = (SphereCollider)char0.rigidFG.collider;
                    SphereCollider sph1 = (SphereCollider)char1.rigidFG.collider;
                    if (Collision.SphereSphereIntersection(sph0, sph1, out Vector3 collisionPoint)) {
                        collision = true;

                        Debug.Log("intersect");

                        Vector3 drawLine3d = char0.position - char1.position;
                        Vector2 drawLine = new Vector2(drawLine3d.x, drawLine3d.z);
                        float drawLineMag = drawLine.magnitude;

                        jacobian = new Vector4(
                            Vector2.Dot(drawLine, new Vector2(-1f, 0f)) / drawLineMag,
                            Vector2.Dot(drawLine, new Vector2(0f, -1f)) / drawLineMag,
                            Vector2.Dot(drawLine, new Vector2(1f, 0f)) / drawLineMag,
                            Vector2.Dot(drawLine, new Vector2(0f, 1f)) / drawLineMag
                        );

                        //Vector3 sph0Center = sph0.transform.rotation * (sph0.transform.position + sph0.center);
                        //Vector3 sph1Center = sph1.transform.rotation * (sph1.transform.position + sph1.center);

                        //Vector3 sph0CollisionDir = collisionPoint - sph0Center;
                        //Vector3 sph1CollisionDir = collisionPoint - sph1Center;
                        //offset = (sph0.radius - sph0CollisionDir.magnitude) * sph0CollisionDir.normalized;
                    }
                }
            }

            public override void Postprocess(float deltaTime) {
                if (!collision) return;

                offset.y = 0.0f;

                float lambda = -(Vector4.Dot(jacobian, new Vector4(char0.velocity.x, char0.velocity.z, char1.velocity.x, char1.velocity.z)) + bias) / Vector4.Dot(jacobian, jacobian);

                Vector2 velChangeChar0 = new Vector2(jacobian.x * lambda, jacobian.y * lambda);
                Vector2 velChangeChar1 = new Vector2(jacobian.z * lambda, jacobian.w * lambda);

                //char0.AddRelativeVelocity(Gameplay.VelocityPriority.Movement, -offset0.normalized * pushOutVelocity);
                //char1.AddRelativeVelocity(Gameplay.VelocityPriority.Movement, -offset1.normalized * pushOutVelocity);

                char0.rigidFG.velocity += new Vector3(velChangeChar0.x, 0f, velChangeChar0.y);
                char1.rigidFG.velocity += new Vector3(velChangeChar1.x, 0f, velChangeChar1.y);
            }
        }
    }
}