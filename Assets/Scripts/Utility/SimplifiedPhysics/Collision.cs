using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace SimplifiedPhysics {
        public class Collision {
            public static bool SphereSphereIntersection(SphereCollider sph0, SphereCollider sph1) {
                return SphereSphereIntersection(sph0, sph1, out Vector3 collisionPoint);
            }

            public static bool SphereSphereIntersection(SphereCollider sph0, SphereCollider sph1, out Vector3 collisionPoint, float sphereExpansion = 1e-3f) {
                Vector3 sph0Center = sph0.transform.position + sph0.transform.rotation * sph0.center;
                Vector3 sph1Center = sph1.transform.position + sph1.transform.rotation * sph1.center;
                Debug.Log(sph0Center.ToString("F2") + " -- " + sph1Center.ToString("F2"));

                float distance = Vector3.Distance(sph0Center, sph1Center);
                if (distance < sph0.radius + sph1.radius + (sphereExpansion * 2.0f)) {
                    collisionPoint = (sph0Center - sph1Center) * 0.5f;
                    return true;
                }
                else {
                    collisionPoint = Vector3.zero;
                    return false;
                }
            }

            public static void CapsuleCapsuleIntersection(CapsuleCollider cap0, CapsuleCollider cap1) {
                Quaternion cap0Rot = cap0.transform.rotation;
                Quaternion cap1Rot = cap1.transform.rotation;

                Vector3 cap0a = default;
                Vector3 cap0b = default;
                switch (cap0.direction) {
                    case 0: //X-Axis
                        cap0a = cap0.transform.position + cap0Rot * (cap0.center + (cap0.height - 2.0f * cap0.radius) * 0.5f * Vector3.right);
                        cap0b = cap0.transform.position + cap0Rot * (cap0.center - (cap0.height - 2.0f * cap0.radius) * 0.5f * Vector3.right);
                        break;
                    case 1: //Y-Axis
                        cap0a = cap0.transform.position + cap0Rot * (cap0.center + (cap0.height - 2.0f * cap0.radius) * 0.5f * Vector3.up);
                        cap0b = cap0.transform.position + cap0Rot * (cap0.center - (cap0.height - 2.0f * cap0.radius) * 0.5f * Vector3.up);
                        break;
                    case 2: //Z-Axis
                        cap0a = cap0.transform.position + cap0Rot * (cap0.center + (cap0.height - 2.0f * cap0.radius) * 0.5f * Vector3.forward);
                        cap0b = cap0.transform.position + cap0Rot * (cap0.center - (cap0.height - 2.0f * cap0.radius) * 0.5f * Vector3.forward);
                        break;
                }

                Vector3 cap1a = default;
                Vector3 cap1b = default;
                switch (cap1.direction) {
                    case 0: //X-Axis
                        cap1a = cap1.transform.position + cap1Rot * (cap1.center + (cap1.height - 2.0f * cap1.radius) * 0.5f * Vector3.right);
                        cap1b = cap1.transform.position + cap1Rot * (cap1.center - (cap1.height - 2.0f * cap1.radius) * 0.5f * Vector3.right);
                        break;
                    case 1: //Y-Axis
                        cap1a = cap1.transform.position + cap1Rot * (cap1.center + (cap1.height - 2.0f * cap1.radius) * 0.5f * Vector3.up);
                        cap1b = cap1.transform.position + cap1Rot * (cap1.center - (cap1.height - 2.0f * cap1.radius) * 0.5f * Vector3.up);
                        break;
                    case 2: //Z-Axis
                        cap1a = cap1.transform.position + cap1Rot * (cap1.center + (cap1.height - 2.0f * cap1.radius) * 0.5f * Vector3.forward);
                        cap1b = cap1.transform.position + cap1Rot * (cap1.center - (cap1.height - 2.0f * cap1.radius) * 0.5f * Vector3.forward);
                        break;
                }

                Vector3 closest0, closest1;
                    // This only works if it checks for line segments, which I don't think this does...
                if (Math3d.ClosestPointsOnTwoLines(out closest0, out closest1, cap0a, cap0b, cap1a, cap1b)) {
                    float distance = Vector3.Distance(closest0, closest1);
                    if (distance < cap0.radius + cap1.radius) {
                        // collision
                    }
                    else {
                        // no collision
                    }
                }
                else { // lines are parallel

                }
            }

            public static void CapsuleSphereIntersection(CapsuleCollider cap0, SphereCollider sph1) {
                Quaternion cap0Rot = cap0.transform.rotation;
                Quaternion sph1Rot = sph1.transform.rotation;


            }

            public static void CapsuleBoxIntersection(CapsuleCollider cap0, BoxCollider box1) {
                Quaternion cap0Rot = cap0.transform.rotation;
                Quaternion box1Rot = box1.transform.rotation;


            }
        }
    }
}