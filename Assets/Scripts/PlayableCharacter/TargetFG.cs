using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Gameplay {
        public class TargetFG : MonoBehaviour {
            public Color lineColor;
            public float verticalOffset;

            public LineRenderer horizontal;
            public LineRenderer verticalForward;
            public LineRenderer verticalBackward;

            public Transform plane;

            public Transform targetCylinder;

            public InGameEntity targetChar;
            public Vector3 targetPos { set; get; }

            private LayerMask levelBounds;
            private float height;

            public void Awake() {
                levelBounds = LayerMask.GetMask("LevelBoundary");
                height = transform.position.y;
            }

            public void SetHeight(float height) {
                this.height = height;
            }

            public void SetNewTarget(InGameEntity newTargetChar) {
                targetChar = newTargetChar;
            }

            public void RealignTargetPos() {
                targetPos = targetChar.position;
            }

            public Vector3 ActualTargetPos() {
                return targetChar.position;
            }

            public void Update() {
                // TODO: Get back to this code to make it work, but it's ultimately not useful.
                //if (Physics.Raycast(transform.position + Vector3.up * 2.0f, Vector3.down, out RaycastHit hitInfo, Mathf.Infinity, levelBounds)) {

                //    Vector3 startPoint = hitInfo.point + verticalOffset * Vector3.up;

                //    Vector3 forward = targetPos - startPoint;
                //    forward.y = 0.0f;
                //    forward.Normalize();

                //    Debug.DrawRay(startPoint, forward, Color.yellow);

                //    RaycastHit forwardHitInfo;
                //    RaycastHit backwardHitInfo;

                //    Vector3 forwardPoint = Vector3.zero;
                //    Vector3 backwardPoint = Vector3.zero;

                //    if (Physics.Raycast(startPoint, forward, out forwardHitInfo, Mathf.Infinity, levelBounds)) {
                //        forwardPoint = forwardHitInfo.point + forwardHitInfo.normal * 0.4f;
                //        forwardPoint.y = startPoint.y;

                //        verticalForward.transform.position = forwardPoint;
                //    }

                //    if (Physics.Raycast(startPoint, -forward, out backwardHitInfo, Mathf.Infinity, levelBounds)) {
                //        backwardPoint = backwardHitInfo.point + backwardHitInfo.normal * 0.4f;
                //        backwardPoint.y = startPoint.y;

                //        verticalBackward.transform.position = backwardPoint;
                //    }

                //    Debug.DrawRay(forwardPoint, forwardHitInfo.normal, Color.red);
                //    Debug.DrawRay(backwardPoint, backwardHitInfo.normal, Color.blue);
                //    horizontal.SetPosition(0, forwardPoint - transform.position);
                //    horizontal.SetPosition(1, backwardPoint - transform.position);
                //}

                targetCylinder.transform.position = targetPos;
                plane.transform.position = new Vector3(plane.transform.position.x, height, plane.transform.position.z);
            }
        }
    }
}