using System;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Menu {
        public enum CursorState : int {
            Stopped,
            Following,
            Seeking,
        }

        public class Cursor3d : MonoBehaviour {
            public Material material;
            public List<MeshRenderer> renderers;
            public Color baseColor;
            public Color selectColor;

            public AnimationCurve easeIn;
            public float easeInDistance;
            public AnimationCurve easeOut;
            public float easeOutDistance;

            public float maxSpeed = 10.0f;

            private Animator animator;
            private Rigidbody rigidbody;

            private CursorState currState;
            private Selectable targetSelectable;
            private Vector3 startPoint;
            private Vector3 endPoint;

            private bool selectAnimationPlaying = false;
            private float selectTime = -Mathf.Infinity;
            private float selectWaitTime = 0.3f;

            private Action currCallback;

            public void Awake() {
                animator = GetComponent<Animator>();
                rigidbody = GetComponent<Rigidbody>();
            }

            public void Update() {
                //baseColor.a = (maxAlpha - minAlpha) * 0.5f * (Mathf.Sin(glowRate * Time.time * 2 * Mathf.PI) + 1.0f) + minAlpha;
                material.color = baseColor;
                if (targetSelectable != null) {
                    Transform targetRectTransform = targetSelectable.GetTransform();

                    switch (currState) {
                        case CursorState.Stopped:
                            // do nothing
                            break;
                        case CursorState.Following:
                            rigidbody.position = targetRectTransform.position;
                            break;
                        case CursorState.Seeking:
                            float distance = 0.0f;
                            if ((distance = Vector3.Distance(rigidbody.position, startPoint)) < easeInDistance) {
                                rigidbody.position += (endPoint - rigidbody.position) * easeIn.Evaluate(distance / easeInDistance) * maxSpeed * Time.deltaTime;
                            }
                            else if ((distance = Vector3.Distance(rigidbody.position, endPoint)) < easeOutDistance) {
                                rigidbody.position += (endPoint - rigidbody.position) * easeOut.Evaluate(distance / easeOutDistance) * maxSpeed * Time.deltaTime;
                            }
                            else {
                                rigidbody.position += (endPoint - rigidbody.position) * maxSpeed * Time.deltaTime;
                            }

                            if (Vector3.Distance(rigidbody.position, endPoint) < 0.01f) {
                                rigidbody.position = endPoint;
                                currState = CursorState.Following;
                            }
                            break;
                    }
                }

                if (currCallback != null) {
                    if (Time.time > selectTime + selectWaitTime) {
                        currCallback();
                        currCallback = null;
                    }
                }
            }

            public void Fade() {
                currCallback = PlayAnimationOnCallback("fade");
            }

            public void Hide() {
                currCallback = PlayAnimationOnCallback("hidden");
            }

            public void Highlight(Selectable selectable) {
                targetSelectable = selectable;
                animator.Play("idle");

                currState = CursorState.Seeking;
                startPoint = rigidbody.position;
                endPoint = targetSelectable.transform.position;
            }

            public void Select(Selectable selectable) {
                targetSelectable = selectable;
                animator.Play("select");
                selectAnimationPlaying = true;
                selectTime = Time.time;

                currState = CursorState.Seeking;
                startPoint = rigidbody.position;
                endPoint = targetSelectable.transform.position;
            }

            private Action PlayAnimationOnCallback(string animationState) {
                return () => {
                    animator.Play(animationState);
                };
            }
        }
    }
}