using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using ResonantSpark.Service;

namespace ResonantSpark {
    namespace Camera {
        public class StageLeakCamera : MonoBehaviour {

            public new UnityEngine.Camera camera;

            public float cameraHeightClosest;
            public float cameraHeightFarthest;
            public float heightLook;

            public float cameraDistanceClosest;
            public float cameraDistanceFarthest;

            public AnimationCurve cameraDistanceFunc;

            public float maxCharacterDistanceApart;

            public float cameraFov = 40f;
            public float dampTime = 4f;

            public float orientationCrossThreshold = 0.04f;

            public int raycastNumSubdivisions = 10;
            public float raycastVerticalOffset = 0.6f;
            public float raycastDirectionScaling = 1.5f;
            public float raycastDistance = 4.0f;

            public AnimationCurve outOfBoundsMovementFuncY;
            public float outOfBoundsMaxHeight = 3.0f;
            public float maxOutOfBoundsDistance = 6.0f;

            private Transform cameraTransform;
            private new Rigidbody rigidbody;

            private Transform char0;
            private Transform char1;

            private LayerMask staticLevel;
            private LayerMask cameraLeak;

            private Transform startTransform;
            private List<Transform> levelBoundaries;

            private List<MeshRenderer> currDisabledRenderers;
            private List<MeshRenderer> prevDisabledRenderers;

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

                currDisabledRenderers = new List<MeshRenderer>();
                prevDisabledRenderers = new List<MeshRenderer>();

                rigidbody = GetComponent<Rigidbody>();

                staticLevel = LayerMask.NameToLayer("StaticLevelGeometry");
                cameraLeak = LayerMask.NameToLayer("CameraLeakGeometry");
            }

            public void SetUpCamera(FightingGameService fgService) {
                this.startTransform = fgService.GetCameraStart();
                this.levelBoundaries = fgService.GetLevelBoundaries();

                cameraTransform = camera.transform;
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
                rigidbody.position = startTransform.position;
                rigidbody.rotation = startTransform.rotation;
            }

            //TODO: Validate that this math is correct.
            public Vector2 ScreenOrientation(Transform candidateTransform) {
                Vector3 camToPlayer;
                Vector3 playerToPlayer;

                if (candidateTransform == char0) {
                    camToPlayer = char0.position - rigidbody.position;
                    playerToPlayer = char1.position - char0.position;
                }
                else {
                    camToPlayer = char1.position - rigidbody.position;
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

                // Not using OnTriggerEnter/Exit because it doesn't pair nicely with the raycast method
            public void OnTriggerStay(Collider other) {
                if (other.gameObject.layer == cameraLeak) {
                    MeshRenderer rend = other.gameObject.GetComponent<MeshRenderer>();

                    if (!currDisabledRenderers.Contains(rend)) {
                        currDisabledRenderers.Add(rend);
                    }
                }
            }

            private void LateUpdate() {
                Vector3 direction = char1.position - char0.position;
                Vector3 midPoint = char0.position + (direction / 2);

                Vector3 currPosition = rigidbody.position;

                testTransform0.transform.position = midPoint;

                Vector3 dirLeft = Vector3.Cross(direction, Vector3.up).normalized;
                Vector3 dirRight = Vector3.Cross(-direction, Vector3.up).normalized;

                float charsDistanceNorm = Vector3.Distance(char1.position, char0.position) / maxCharacterDistanceApart;

                float cameraHeight = Mathf.Lerp(cameraHeightClosest, cameraHeightFarthest, cameraDistanceFunc.Evaluate(charsDistanceNorm));
                float cameraDistance = Mathf.Lerp(cameraDistanceClosest, cameraDistanceFarthest, cameraDistanceFunc.Evaluate(charsDistanceNorm));

                Vector3 desiredPosition;
                if (((midPoint + dirLeft) - rigidbody.position).sqrMagnitude < ((midPoint + dirRight) - rigidbody.position).sqrMagnitude) {
                    testTransform1.transform.position = midPoint + dirLeft;
                    desiredPosition = midPoint + dirLeft * cameraDistance + Vector3.up * cameraHeight;
                }
                else {
                    testTransform1.transform.position = midPoint + dirRight;
                    desiredPosition = midPoint + dirRight * cameraDistance + Vector3.up * cameraHeight;
                }

                Vector3 newPosition = Vector3.Lerp(currPosition, desiredPosition, dampTime * Time.deltaTime);
                Vector3 characterCenterPoint = midPoint + (cameraHeightClosest - raycastVerticalOffset) * Vector3.up;

                rigidbody.position = newPosition;

                Vector3 cameraTransformPosition = cameraTransform.position;
                if (!InBoundary(desiredPosition, levelBoundaries, out float distanceOutOfBounds)) {
                    float outOfBoundaryCameraHeight = outOfBoundsMaxHeight * outOfBoundsMovementFuncY.Evaluate(distanceOutOfBounds / maxOutOfBoundsDistance);

                    cameraTransform.localPosition = new Vector3(0.0f, outOfBoundaryCameraHeight, 0.0f);
                    cameraTransform.localRotation = Quaternion.Euler(20f * outOfBoundsMovementFuncY.Evaluate(distanceOutOfBounds / maxOutOfBoundsDistance), 0.0f, 0.0f);

                    cameraTransformPosition = cameraTransform.position;
                }
                else {
                    cameraTransform.localPosition = Vector3.zero;
                    cameraTransform.localRotation = Quaternion.identity;
                }

                PerformRaycasts(newPosition, characterCenterPoint, direction);
                ResetActivePolling();

                //cameraTransform.LookAt(midPoint + Vector3.up * heightLook);
                transform.LookAt(midPoint + Vector3.up * heightLook);   // This moves the local Transform if it's offset.
                cameraTransform.position = cameraTransformPosition;     // Reset it back to where it used to be.
            }

            private bool InBoundary(Vector3 cameraPos, List<Transform> boundaries, out float distanceOutOfBounds) {
                for (int n = 0; n < boundaries.Count; ++n) {
                    //currLine = boundaries[n].right;
                    Vector3 currLine;
                    Vector3 centerPoint;
                    if (n == 0) {
                        currLine = (-boundaries[0].position + boundaries[boundaries.Count - 1].position).normalized;
                        centerPoint = (boundaries[0].position + boundaries[boundaries.Count - 1].position) / 2;
                    }
                    else {
                        currLine = (boundaries[n - 1].position - boundaries[n].position).normalized;
                        centerPoint = (boundaries[n - 1].position + boundaries[n].position) / 2;
                    }
                    currLine.y = 0;

                    Vector3 dir = cameraPos - centerPoint;
                    dir.y = 0;
                    Vector3 cross = Vector3.Cross(currLine, dir);
                    if (cross.y > 0) {
                        Vector3 point;
                        if (n == 0) {
                            point = cameraPos - boundaries[boundaries.Count - 1].position;
                        }
                        else {
                            point = cameraPos - boundaries[n - 1].position;
                        }
                        point.y = 0;
                        distanceOutOfBounds = Vector3.Cross(currLine, point).magnitude;
                        return false;
                    }
                }
                distanceOutOfBounds = 0.0f;
                return true;
            }

            private void PerformRaycasts(Vector3 newPosition, Vector3 characterCenterPoint, Vector3 direction) {
                Vector3 cameraCheckDirection;
                RaycastHit[] currHits;

                for (int n = 0; n < raycastNumSubdivisions; ++n) {

                    for (int leftRight = 0; leftRight < 2; ++leftRight) {
                        if (n == 0 && leftRight != 0) continue;

                        int counter = n;
                        if (leftRight == 0) {
                            counter = -n;
                        }

                        Vector3 currCameraCheckDirection = (characterCenterPoint + counter * direction * raycastDirectionScaling * 0.5f / raycastNumSubdivisions) - newPosition;
                        currHits = Physics.RaycastAll(
                            newPosition,
                            currCameraCheckDirection,
                            raycastDistance,
                            LayerMask.GetMask("CameraLeakGeometry", "StaticLevelGeometry"),
                            QueryTriggerInteraction.Ignore);

                        Debug.DrawLine(newPosition + currCameraCheckDirection.normalized * raycastDistance, newPosition + currCameraCheckDirection.normalized * raycastDistance + Vector3.up * 0.1f, Color.red);
                        Debug.DrawLine(newPosition, newPosition + currCameraCheckDirection.normalized * raycastDistance, Color.cyan);

                        for (int hitCounter = 0; hitCounter < currHits.Length; ++hitCounter) {
                            if (currHits[hitCounter].collider.gameObject.layer == staticLevel || currHits[hitCounter].collider.gameObject.layer == cameraLeak) {
                                MeshRenderer rend = currHits[hitCounter].collider.gameObject.GetComponent<MeshRenderer>();
                                if (!currDisabledRenderers.Contains(rend)) {
                                    currDisabledRenderers.Add(rend);
                                }
                            }
                        }
                    }
                }

                for (int charId = 0; charId < 2; ++charId) {

                    if (charId == 0) {
                        cameraCheckDirection = (char0.position + 0.5f * Vector3.up) - newPosition;
                    }
                    else {
                        cameraCheckDirection = (char1.position + 0.5f * Vector3.up) - newPosition;
                    }

                    currHits = Physics.SphereCastAll(
                        newPosition,
                        0.25f,
                        cameraCheckDirection,
                        cameraCheckDirection.magnitude,
                        LayerMask.GetMask("CameraLeakGeometry", "StaticLevelGeometry"),
                        QueryTriggerInteraction.Ignore);

                    Debug.DrawLine(newPosition, newPosition + cameraCheckDirection, Color.blue);

                    for (int hitCounter = 0; hitCounter < currHits.Length; ++hitCounter) {
                        if (currHits[hitCounter].collider.gameObject.layer == staticLevel || currHits[hitCounter].collider.gameObject.layer == cameraLeak) {
                            MeshRenderer rend = currHits[hitCounter].collider.gameObject.GetComponent<MeshRenderer>();
                            if (!currDisabledRenderers.Contains(rend)) {
                                currDisabledRenderers.Add(rend);
                            }
                        }
                    }
                }
            }

            private void ResetActivePolling() {
                foreach (MeshRenderer rend in currDisabledRenderers) {
                    if (!prevDisabledRenderers.Contains(rend)) {
                        rend.enabled = false;
                    }
                }

                foreach (MeshRenderer rend in prevDisabledRenderers) {
                    if (!currDisabledRenderers.Contains(rend)) {
                        rend.enabled = true;
                    }
                }

                prevDisabledRenderers.Clear();
                foreach (MeshRenderer rend in currDisabledRenderers) {
                    prevDisabledRenderers.Add(rend);
                }
                currDisabledRenderers.Clear();
            }
        }
    }
}