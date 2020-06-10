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

            private bool selectAnimationPlaying = false;
            private float selectTime = -Mathf.Infinity;
            private float selectWaitTime = 0.3f;

            private Action currCallback;

            public void Awake() {
                animator = GetComponent<Animator>();
                rectTransform = GetComponent<RectTransform>();
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
                startPoint = transform.position;
                endPoint = ((RectTransform)targetSelectable.GetTransform()).position;
            }

            public void Select(Selectable selectable) {
                targetSelectable = selectable;
                animator.Play("select");
                selectAnimationPlaying = true;
                selectTime = Time.time;

                currState = CursorState.Seeking;
                startPoint = transform.position;
                endPoint = ((RectTransform)targetSelectable.GetTransform()).position;
            }

            private Action PlayAnimationOnCallback(string animationState) {
                return () => {
                    animator.Play(animationState);
                };
            }
        }
    }
}