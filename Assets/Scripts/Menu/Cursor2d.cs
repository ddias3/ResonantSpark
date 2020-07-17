using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ResonantSpark {
    namespace Menu {
        public class Cursor2d : MonoBehaviour {
            public Image image;
            public Color baseColor;
            public Color selectColor;

            public AnimationCurve easeIn;
            public float easeInDistance;
            public AnimationCurve easeOut;
            public float easeOutDistance;

            public float maxSpeed = 10.0f;

            private Animator animator;
            private RectTransform rectTransform;

            private CursorState currState;
            private Selectable targetSelectable;
            private Vector2 startPoint;
            private Vector2 endPoint;

            private float selectWaitTime = 0.3f;

            private Queue<(Action cb, float selectTime)> callbacks;

            public void Awake() {
                animator = GetComponent<Animator>();
                rectTransform = GetComponent<RectTransform>();
                callbacks = new Queue<(Action cb, float selectTime)>();
            }

            public void Update() {
                //baseColor.a = (maxAlpha - minAlpha) * 0.5f * (Mathf.Sin(glowRate * Time.time * 2 * Mathf.PI) + 1.0f) + minAlpha;
                image.color = baseColor;
                if (targetSelectable != null) {
                    RectTransform targetRectTransform = (RectTransform)targetSelectable.GetTransform();

                    switch (currState) {
                        case CursorState.Stopped:
                            // do nothing
                            break;
                        case CursorState.Following:
                            rectTransform.anchoredPosition = targetRectTransform.anchoredPosition;
                            break;
                        case CursorState.Seeking:
                            float distance = 0.0f;
                            if ((distance = Vector2.Distance(transform.position, startPoint)) < easeInDistance) {
                                rectTransform.anchoredPosition += (endPoint - rectTransform.anchoredPosition) * easeIn.Evaluate(distance / easeInDistance) * maxSpeed * Time.deltaTime;
                            }
                            else if ((distance = Vector2.Distance(transform.position, endPoint)) < easeOutDistance) {
                                rectTransform.anchoredPosition += (endPoint - rectTransform.anchoredPosition) * easeOut.Evaluate(distance / easeOutDistance) * maxSpeed * Time.deltaTime;
                            }
                            else {
                                rectTransform.anchoredPosition += (endPoint - rectTransform.anchoredPosition) * maxSpeed * Time.deltaTime;
                            }

                            if (Vector2.Distance(transform.position, endPoint) < 0.01f) {
                                transform.position = endPoint;
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
                startPoint = transform.position;
                endPoint = ((RectTransform)targetSelectable.GetTransform()).position;
                if (callback != null) {
                    callbacks.Enqueue((callback, Mathf.NegativeInfinity));
                }
            }

            public void Select(Selectable selectable, Action callback = null) {
                targetSelectable = selectable;
                animator.Play("select");

                currState = CursorState.Seeking;
                startPoint = transform.position;
                endPoint = ((RectTransform)targetSelectable.GetTransform()).position;
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