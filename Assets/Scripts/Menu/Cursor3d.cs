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

            private float selectWaitTime = 0.3f;

            private Queue<(Action cb, float selectTime)> callbacks;

            public void Awake() {
                animator = GetComponent<Animator>();
                rigidbody = GetComponent<Rigidbody>();
                callbacks = new Queue<(Action cb, float selectTime)>();
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

                if (callbacks.Count > 0) {
                    if (Time.time > callbacks.Peek().selectTime + selectWaitTime) {
                        Action currCallback = callbacks.Dequeue().cb;
                        currCallback();
                    }
                }
            }

            public void Fade() {
                callbacks.Enqueue((PlayAnimationOnCallback("fade"), Mathf.NegativeInfinity));
            }

            public void Hide() {
                callbacks.Enqueue((PlayAnimationOnCallback("hidden"), Mathf.NegativeInfinity));
            }

            public void Highlight(Selectable selectable, Action callback = null) {
                targetSelectable = selectable;
                animator.Play("idle");

                currState = CursorState.Seeking;
                startPoint = rigidbody.position;
                endPoint = targetSelectable.transform.position;
                if (callback != null) {
                    callbacks.Enqueue((callback, Mathf.NegativeInfinity));
                }
            }

            public void Select(Selectable selectable, Action callback = null) {
                targetSelectable = selectable;
                animator.Play("select");

                currState = CursorState.Seeking;
                startPoint = rigidbody.position;
                endPoint = targetSelectable.transform.position;
                if (callback != null) {
                    callbacks.Enqueue((callback, Time.time));
                }
            }

            private Action PlayAnimationOnCallback(string animationState) {
                return () => {
                    animator.Play(animationState);
                };
            }
        }
    }
}