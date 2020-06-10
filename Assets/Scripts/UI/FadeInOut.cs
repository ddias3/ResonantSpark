using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using ResonantSpark.Utility;

namespace ResonantSpark {
    namespace UI {
        public class FadeInOut : MonoBehaviour {

            public float fadeInTime = 0.5f;
            public float fadeOutTime = 1.0f;

            private RawImage image;
            private Color fadeInColor;

            private List<Action> callbacks;

            private float startTime = 0.0f;
            private bool fadeIn;

            public void Awake() {
                callbacks = new List<Action>();

                image = GetComponent<RawImage>();
                fadeInColor = image.color;

                this.enabled = false;
            }

            public void FadeOut() {
                gameObject.SetActive(true);
                this.enabled = true;
                startTime = Time.time;

                fadeInColor.a = 0.0f;
                fadeIn = false;
            }

            public void FadeIn() {
                gameObject.SetActive(true);
                this.enabled = true;
                startTime = Time.time;

                fadeInColor.a = 1.0f;
                fadeIn = true;
            }

            public void OnComplete(Action callback) {
                callbacks.Add(callback);
            }

            public void Update() {
                if (fadeIn) {
                    fadeInColor.a = 1.0f - (Time.time - startTime) / fadeInTime;

                    if (Time.time - startTime > fadeInTime) {
                        Complete();
                    }
                }
                else {
                    fadeInColor.a = (Time.time - startTime) / fadeOutTime;

                    if (Time.time - startTime > fadeInTime) {
                        Complete();
                    }
                }

                image.color = fadeInColor;
            }

            private void Complete() {
                for (int n = 0; n < callbacks.Count; ++n) {
                    callbacks[n].Invoke();
                }
            }
        }
    }
}