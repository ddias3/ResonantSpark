using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Utility {
        public class TargetUtil {
            public static Vector2 LineIntersection(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3) {
                float normalizer = (p0.x - p1.x) * (p2.y - p3.y) - (p0.y - p1.y) * (p2.x - p3.x);
                return new Vector2(
                    ((p0.x * p1.y - p0.y * p1.x) * (p2.x - p3.x) - (p0.x - p1.x) * (p2.x * p3.y - p2.y * p3.x))
                    / normalizer,
                    ((p0.x * p1.y - p0.y * p1.x) * (p2.y - p3.y) - (p0.y - p1.y) * (p2.x * p3.y - p2.y * p3.x))
                    / normalizer
                );
            }

            public static Vector3 MoveTargetLimited(Vector3 playerPos, Vector3 currPos, Vector3 targetPos, float angleLimit) {
                targetPos.y = 0.0f;
                currPos.y = 0.0f;
                playerPos.y = 0.0f;

                Vector3 desiredMove = targetPos - currPos;

                Vector3 targetDir = targetPos - playerPos;
                Vector3 currDir = currPos - playerPos;

                float angleDesired = Vector3.SignedAngle(targetDir, currDir, Vector3.up);

                if (Mathf.Abs(angleDesired) < angleLimit) {
                    return targetPos;
                }
                else {
                    Vector3 limitedDir = Quaternion.Euler(0.0f, Mathf.Sign(angleDesired) * angleLimit, 0.0f) * currDir;
                    Vector2 intersec = LineIntersection(
                        new Vector2(playerPos.x, playerPos.z), new Vector2((playerPos + limitedDir).x, (playerPos + limitedDir).z),
                        new Vector2(currPos.x, currPos.z), new Vector2(targetPos.x, targetPos.z)
                    );
                    Vector3 limitedPos = new Vector3(intersec.x, 0.0f, intersec.y);

                    return limitedPos;
                }
            }
        }
    }
}