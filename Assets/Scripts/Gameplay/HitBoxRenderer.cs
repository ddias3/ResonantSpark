using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Gameplay {
        public class HitBoxRenderer : MonoBehaviour {
            public Transform cylinder;
            public Transform spherePivot;
            public Transform sphereFar;

            private bool _enable = true;
            private float _radius = 0.25f;
            private float _height = 0.5f;

            public float radius {
                set {
                    _radius = value;

                    spherePivot.localScale = new Vector3(value * 2, value * 2, value * 2);
                    sphereFar.localScale = new Vector3(value * 2, value * 2, value * 2);
                }
            }

            public float height {
                set {
                    _height = value;

                    cylinder.localPosition = new Vector3(0, 0, value / 2);
                    cylinder.localScale = new Vector3(_radius * 2, value / 2, _radius * 2);
                    sphereFar.localPosition = new Vector3(0, 0, value);
                }
            }

            public bool enable {
                set {
                    _enable = value;

                    cylinder.gameObject.SetActive(_enable);
                    spherePivot.gameObject.SetActive(_enable);
                    sphereFar.gameObject.SetActive(_enable);
                }
                get {
                    return _enable;
                }
            }
        }
    }
}