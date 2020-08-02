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

            public SphereCollider debugCollider;

            public AnimationCurve overlapDistance;
            public float maxOverlapAngle = 30.0f;

            private Transform cameraTransform;
            private new Rigidbody rigidbody;

            private Transform char0;
            private Transform char1;

            private LayerMask staticLevelGeometry;
            private LayerMask inStageDebris;

            private LayerMask staticLevelGeometryMask;
            private LayerMask sphereOverlapMask;

            private Transform startTransform;
            private List<Transform> levelBoundaries;

            private List<MeshRenderer> currDisabledRenderers;
            private List<MeshRenderer> prevDisabledRenderers;

            private Vector2 facingRight;
            private Vector2 facingLeft;
            private Vector2 ambiguous;

            private Collider[] colliderBuffer;
            private RaycastHit[] raycastBuffer;

            private Dictionary<GameObject, MeshRenderer> meshRendererCache;

            public void Awake() {
                this.enabled = false;
                facingRight = new Vector2(1.0f, 1.0f);
                facingLeft = new Vector2(-1.0f, 1.0f);
                ambiguous = new Vector2(0.0f, 1.0f);

                colliderBuffer = new Collider[1024];
                raycastBuffer = new RaycastHit[512];

                meshRendererCache = new Dictionary<GameObject, MeshRenderer>();

                currDisabledRenderers = new List<MeshRenderer>();
                prevDisabledRenderers = new List<MeshRenderer>();

                rigidbody = GetComponent<Rigidbody>();

                staticLevelGeometry = LayerMask.NameToLayer("StaticLevelGeometry");
                inStageDebris = LayerMask.NameToLayer("InStageDebris");

                staticLevelGeometryMask = LayerMask.GetMask("StaticLevelGeometry");
                sphereOverlapMask = LayerMask.GetMask("InStageDebris", "StaticLevelGeometry");
            }

            public void SetUpCamera(FightingGameService fgService) {
                this.startTransform = fgService.GetCameraStart();
                this.levelBoundaries = fgService.GetLevelBoundaries();

                cameraTransform = camera.transform;
                camera.fieldOfView = cameraFov;

                GameObject serviceObj = GameObject.FindGameObjectWithTag("rspService");

                PlayerService playerService = serviceObj.GetComponent<PlayerService>();

                char0 = playerService.GetFGChar("player1").transform;
                char1 = playerService.GetFGChar("player2").transform;
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

            private void LateUpdate() {
                Vector3 direction = char1.position - char0.position;
                Vector3 midPoint = char0.position + (direction / 2);

                Vector3 currPosition = rigidbody.position;

                Vector3 dirLeft = Vector3.Cross(direction, Vector3.up).normalized;
                Vector3 dirRight = Vector3.Cross(-direction, Vector3.up).normalized;

                float charsDistanceNorm = Vector3.Distance(char1.position, char0.position) / maxCharacterDistanceApart;

                float cameraHeight = Mathf.Lerp(cameraHeightClosest, cameraHeightFarthest, cameraDistanceFunc.Evaluate(charsDistanceNorm));
                float cameraDistance = Mathf.Lerp(cameraDistanceClosest, cameraDistanceFarthest, cameraDistanceFunc.Evaluate(charsDistanceNorm));

                Vector3 desiredPosition;
                if (((midPoint + dirLeft) - rigidbody.position).sqrMagnitude < ((midPoint + dirRight) - rigidbody.position).sqrMagnitude) {
                    desiredPosition = midPoint + dirLeft * cameraDistance + Vector3.up * cameraHeight;
                }
                else {
                    desiredPosition = midPoint + dirRight * cameraDistance + Vector3.up * cameraHeight;
                }

                Vector3 newPosition = Vector3.Lerp(currPosition, desiredPosition, dampTime * Time.deltaTime);
                Vector3 characterCenterPoint = midPoint + (cameraHeightClosest - raycastVerticalOffset) * Vector3.up;

                rigidbody.position = newPosition;

                if (!InBoundary(desiredPosition, levelBoundaries, out float distanceOutOfBounds)) {
                    float outOfBoundaryCameraHeight = outOfBoundsMaxHeight * outOfBoundsMovementFuncY.Evaluate(distanceOutOfBounds / maxOutOfBoundsDistance);

                    cameraTransform.localPosition = new Vector3(0.0f, outOfBoundaryCameraHeight, 0.0f);
                    cameraTransform.localRotation = Quaternion.Euler(20f * outOfBoundsMovementFuncY.Evaluate(distanceOutOfBounds / maxOutOfBoundsDistance), 0.0f, 0.0f);
                }
                else {
                    cameraTransform.localPosition = Vector3.zero;
                    cameraTransform.localRotation = Quaternion.identity;
                }

                PerformOverlapClearing(cameraTransform.position, midPoint + Vector3.up * 0.8f, char0.position, char1.position);
                PerformRaycasts(cameraTransform.position, characterCenterPoint, direction);
                ResetActivePolling();

                //cameraTransform.LookAt(midPoint + Vector3.up * heightLook);
                transform.LookAt(midPoint + Vector3.up * heightLook);
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

            private void DisableRenderer(Collider collider) {
                MeshRenderer rend;
                if (meshRendererCache.ContainsKey(collider.gameObject)) {
                    rend = meshRendererCache[collider.gameObject];
                }
                else {
                    rend = collider.gameObject.GetComponent<MeshRenderer>();
                    meshRendererCache[collider.gameObject] = rend;
                }

                if (!currDisabledRenderers.Contains(rend)) {
                    currDisabledRenderers.Add(rend);
                }
            }

            private Vector3 ClosestPointToLine(Vector3 point, Vector3 a, Vector3 b) {
                return a + Vector3.Project(point - a, b - a);
            }

            private void PerformOverlapClearing(Vector3 cameraPos, Vector3 charactersCenter, Vector3 charPos0, Vector3 charPos1) {
                Vector3 targetDir = 1.3f * (charactersCenter - cameraPos);
                Vector3 targetChar0 = charPos0 - cameraPos;
                Vector3 targetChar1 = charPos1 - cameraPos;

                //float angle = Mathf.Tan((charactersDistance * 0.5f) / targetDir.magnitude) * Mathf.Rad2Deg;

                int numCollisions = Physics.OverlapSphereNonAlloc(cameraPos, targetDir.magnitude, colliderBuffer, sphereOverlapMask, QueryTriggerInteraction.Ignore);
                debugCollider.radius = targetDir.magnitude;
                debugCollider.transform.position = cameraPos;

                for (int n = 0; n < numCollisions; ++n) {
                    Collider curr = colliderBuffer[n];
                    if (curr.gameObject.layer == inStageDebris) {
                        float x0 = Vector3.Angle(curr.transform.position - cameraPos, targetChar0);
                        float x1 = Vector3.Angle(curr.transform.position - cameraPos, targetChar1);
                        float angle = Mathf.Min(x0, x1);

                        float dist0 = Vector3.Distance(curr.transform.position, cameraPos);
                        float x2 = overlapDistance.Evaluate(angle / maxOverlapAngle);
                        if (dist0 < targetDir.magnitude * x2) {
                            DisableRenderer(curr);
                        }
                    }
                    else if (curr.gameObject.layer == staticLevelGeometry) {
                        if (Vector3.Angle(curr.transform.position - cameraPos, targetDir) < 60.0f) {
                            Vector3 closestPoint0;
                            Vector3 closestPoint1;
                            if (Math3d.ClosestPointsOnTwoLines(out closestPoint0, out closestPoint1, cameraPos, curr.transform.position, charPos0, charPos1)) {
                                Vector3 targetPoint = closestPoint0 + closestPoint1 * 0.5f;

                                if (Vector3.Distance(curr.transform.position, cameraPos) < Vector3.Distance(targetPoint, cameraPos)) {
                                    DisableRenderer(curr);
                                }
                            }
                        }
                    }
                }
            }

            private void PerformRaycasts(Vector3 cameraPos, Vector3 characterCenterPoint, Vector3 direction) {
                Vector3 cameraCheckDirection;

                // Check for large objects directly in front of the playable characters. These objects may be so large that
                //   their center and the raycast position are far apart that the sphere overlap method doesn't work.
                for (int charId = 0; charId < 2; ++charId) {

                    if (charId == 0) {
                        cameraCheckDirection = (char0.position + 0.5f * Vector3.up) - cameraPos;
                    }
                    else {
                        cameraCheckDirection = (char1.position + 0.5f * Vector3.up) - cameraPos;
                    }

                    int numCollisions = Physics.SphereCastNonAlloc(
                        cameraPos, 0.25f, cameraCheckDirection.normalized, raycastBuffer, cameraCheckDirection.magnitude, staticLevelGeometryMask, QueryTriggerInteraction.Ignore);

                    Debug.DrawLine(cameraPos, cameraPos + cameraCheckDirection, Color.blue);

                    for (int n = 0; n < numCollisions; ++n) {
                        Collider curr = raycastBuffer[n].collider;
                        if (curr.gameObject.layer == staticLevelGeometry) {
                            DisableRenderer(curr);
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