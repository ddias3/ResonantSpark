using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using ResonantSpark.Service;

namespace ResonantSpark {
    namespace Camera {
        public class InStageCamera : MonoBehaviour {

            public float cameraHeightClosest;
            public float cameraHeightFarthest;
            public float heightLook;

            public float cameraDistanceClosest;
            public float cameraDistanceFarthest;

            public AnimationCurve cameraDistanceFunc;

            public float maxCharacterDistanceApart;

            public float cameraFov = 40f;
            public float dampTime = 4f;

            public float whiskerLength = 2.0f;

            public float orientationCrossThreshold = 0.04f;

            private Transform char0;
            private Transform char1;

            private new UnityEngine.Camera camera;

            private Transform startTransform;

            private Vector2 facingRight;
            private Vector2 facingLeft;
            private Vector2 ambiguous;

            private GameObject testTransform0;
            private GameObject testTransform1;
            private GameObject testTransform2;

            public void Awake() {
                this.enabled = false;
                facingRight = new Vector2(1.0f, 1.0f);
                facingLeft = new Vector2(-1.0f, 1.0f);
                ambiguous = new Vector2(0.0f, 1.0f);
            }

            public void SetUpCamera(Transform startTransform) {
                this.startTransform = startTransform;

                camera = GetComponent<UnityEngine.Camera>();
                camera.fieldOfView = cameraFov;

                GameObject serviceObj = GameObject.FindGameObjectWithTag("rspService");

                PlayerService playerService = serviceObj.GetComponent<PlayerService>();

                char0 = playerService.GetFGChar(0).transform;
                char1 = playerService.GetFGChar(1).transform;

                testTransform0 = GameObject.CreatePrimitive(PrimitiveType.Cube);
                testTransform0.GetComponent<Collider>().enabled = false;
                testTransform0.transform.localScale = new Vector3(0.1f, 0.2f, 0.1f);

                testTransform1 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                testTransform1.GetComponent<Collider>().enabled = false;
                testTransform1.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

                testTransform2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                testTransform2.GetComponent<Collider>().enabled = false;
                testTransform2.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            }

            public void ResetCameraPosition() {
                this.enabled = true;
                transform.position = startTransform.position;
                transform.rotation = startTransform.rotation;
            }

            //TODO: Validate that this math is correct.
            public Vector2 ScreenOrientation(Transform candidateTransform) {
                Vector3 camToPlayer;
                Vector3 playerToPlayer;

                if (candidateTransform == char0) {
                    camToPlayer = char0.position - transform.position;
                    playerToPlayer = char1.position - char0.position;
                }
                else {
                    camToPlayer = char1.position - transform.position;
                    playerToPlayer = char0.position - char1.position;
                }

                Vector3 crossProduct = Vector3.Cross(camToPlayer, playerToPlayer);

                if (Mathf.Abs(crossProduct.y) > orientationCrossThreshold) {
                    if (crossProduct.y > 0) {
                        return facingRight;
                    }
                    else {
                        return facingLeft;
                    }
                }
                else {
                    return ambiguous;
                }
            }

            void LateUpdate() {
                Vector3 direction = char1.position - char0.position;
                Vector3 midPoint = char0.position + (direction / 2);

                Vector3 currPosition = transform.position;

                testTransform0.transform.position = midPoint;

                Vector3 dirLeft = Vector3.Cross(direction, Vector3.up).normalized;
                Vector3 dirRight = Vector3.Cross(-direction, Vector3.up).normalized;

                float charsDistanceNorm = Vector3.Distance(char1.position, char0.position) / maxCharacterDistanceApart;

                //float cameraDistance = cameraDistanceClosest + cameraDistanceFunc.Evaluate(charsDistanceNorm) * cameraDistanceFarthest;
                //float cameraHeight = cameraHeightClosest + cameraDistanceFunc.Evaluate(charsDistanceNorm) * cameraHeightFarthest;

                float cameraDistance = Mathf.Lerp(cameraDistanceClosest, cameraDistanceFarthest, cameraDistanceFunc.Evaluate(charsDistanceNorm));
                float cameraHeight = Mathf.Lerp(cameraHeightClosest, cameraHeightFarthest, cameraDistanceFunc.Evaluate(charsDistanceNorm));

                Vector3 desiredPosition;
                if (((midPoint + dirLeft) - transform.position).sqrMagnitude < ((midPoint + dirRight) - transform.position).sqrMagnitude) {
                    testTransform1.transform.position = midPoint + dirLeft;
                    desiredPosition = midPoint + dirLeft * cameraDistance + Vector3.up * cameraHeight;
                }
                else {
                    testTransform1.transform.position = midPoint + dirRight;
                    desiredPosition = midPoint + dirRight * cameraDistance + Vector3.up * cameraHeight;
                }

                Vector3 newPosition = Vector3.Lerp(currPosition, desiredPosition, dampTime * Time.deltaTime);

                //Vector3 whiskerDirection = newPosition - currPosition;
                //Vector3 whiskerDirection = (currPosition - (midPoint + cameraHeightClosest * Vector3.up));

                //RaycastHit whiskerHit;
                //if (Physics.Raycast(currPosition, whiskerDirection, out whiskerHit, whiskerLength)) {
                //    newPosition = Vector3.Lerp(currPosition, desiredPosition, (whiskerHit.distance / whiskerLength) * dampTime * Time.deltaTime);
                //}

                Vector3 cameraCheckDirection = (newPosition) - (midPoint + cameraHeightClosest * Vector3.up);
                //cameraCheckDirection -= 0.2f * cameraCheckDirection.normalized;

                RaycastHit cameraHit;
                if (Physics.Raycast(midPoint + cameraHeightClosest * Vector3.up, cameraCheckDirection, out cameraHit, cameraCheckDirection.magnitude)) {
                    //newPosition = cameraHit.point - 0.2f * cameraHit.normal;
                    newPosition = cameraHit.point + cameraCheckDirection.normalized * 0.2f;
                }

                transform.position = newPosition;
                transform.LookAt(midPoint + Vector3.up * heightLook);
            }
        }
    }
}