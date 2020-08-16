using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Gameplay;

namespace ResonantSpark {
    namespace SimplifiedPhysics {
        public class SoftEnforce2d : GlobalConstraint {
            public float maxAngle = 60.0f;
            public float maxAngleRotation = 600.0f;
            public float maxWallStopAngle = 40.0f;

            private RigidbodyFG char0;
            private RigidbodyFG char1;

            private Vector3 directLine;
            private LayerMask levelBounds;

            private Vector3 gameplay2dLine;

            public void Awake() {
                levelBounds = LayerMask.GetMask("LevelBoundary");
                char0 = null;
                char1 = null;
            }

            public void SetRigidbodyFGs(RigidbodyFG char0, RigidbodyFG char1) {
                this.char0 = char0;
                this.char1 = char1;
                this.directLine = char1.position - char0.position;
                this.directLine.y = 0;
            }

            public override void Preprocess(float deltaTime) {
                gameplay2dLine = char1.position - char0.position;
                gameplay2dLine.y = 0;
            }

            public override void Postprocess(float deltaTime) {
                    // What was I thinking, this code doesn't make sense.
                if (Vector3.Angle(this.directLine, gameplay2dLine) > maxAngle) {

                }
                else {
                    this.directLine = gameplay2dLine;
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