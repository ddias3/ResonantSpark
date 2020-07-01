using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Gameplay;

namespace ResonantSpark {
    namespace SimplifiedPhysics {
        public class OverlapResolve : GlobalConstraint {
            public float maxAngle = 60.0f;
            public float maxAngleRotation = 600.0f;
            public float maxWallStopAngle = 40.0f;

            private RigidbodyFG char0;
            private RigidbodyFG char1;

            private Vector3 directLine;
            private LayerMask levelBounds;

            public void Awake() {
                levelBounds = LayerMask.GetMask("LevelBoundary");
            }

            public void SetRigidbodyFGs(RigidbodyFG char0, RigidbodyFG char1) {
                this.char0 = char0;
                this.char1 = char1;
                this.directLine = char1.position - char0.position;
                this.directLine.y = 0;
            }

            public override void Restrict() {
                Vector3 newDirectLine = char1.position - char0.position;
                newDirectLine.y = 0;

                if (Vector3.Angle(this.directLine, newDirectLine) > maxAngle) {

                }
                else {
                    this.directLine = newDirectLine;
                }

                RaycastHit wallHit0;
                RaycastHit wallHit1;
                Physics.Raycast(char0.position + Vector3.up, this.directLine, out wallHit0, Mathf.Infinity, levelBounds);
                Physics.Raycast(char0.position + Vector3.up, -this.directLine, out wallHit1, Mathf.Infinity, levelBounds);

                if (Vector3.Angle(directLine, wallHit0.normal) > maxWallStopAngle) {
                    // slide off the wall
                }
                else {
                    // stop at the wall
                }
            }
        }
    }
}