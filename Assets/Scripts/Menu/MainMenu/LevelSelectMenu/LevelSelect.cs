using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Level;
using ResonantSpark.DeviceManagement;

namespace ResonantSpark {
    namespace Menu {
        public class LevelSelect : Selectable {
            public List<LevelInfo> levels;
            public Transform levelDisplayPrefab;

            public Transform forwardLevelPortrait;
            public Transform queueFront;

            public Vector3 queueOffset;

            public TMPro.TMP_Text levelName;

            private List<Transform> levelDisplays;

            private int currSelected;

            public void Start() {
                levelDisplays = new List<Transform>();
                currSelected = 0;

                for (int n = 0; n < levels.Count; ++n) {
                    Transform levelDisplay = Instantiate(levelDisplayPrefab,
                        queueFront.position + queueOffset * n,
                        queueFront.rotation,
                        transform);
                    levelDisplay.localScale = queueFront.localScale;
                    levelDisplay.GetComponent<MeshRenderer>().materials = new Material[] { levels[n].preview };

                    levelDisplays.Add(levelDisplay);
                }

                eventHandler.On("activate", () => {
                    for (int n = 0; n < levelDisplays.Count; ++n) {
                        levelDisplays[n].gameObject.SetActive(true);
                    }
                    levelDisplays[currSelected].position = forwardLevelPortrait.position;
                    levelDisplays[currSelected].rotation = forwardLevelPortrait.rotation;
                    levelDisplays[currSelected].localScale = forwardLevelPortrait.localScale;
                    levelName.text = levels[currSelected].name;
                });
                eventHandler.On("deactivate", () => {
                    for (int n = 0; n < levelDisplays.Count; ++n) {
                        levelDisplays[n].gameObject.SetActive(false);
                    }
                });

                On("left", (GameDeviceMapping devMap) => {
                    currSelected -= 1;
                    if (currSelected < 0) {
                        currSelected = levels.Count - 1;
                    }
                    MovePortraits();
                });
                On("right", (GameDeviceMapping devMap) => {
                    // NEED to pass in controller information to determine which one moves right.
                    currSelected += 1;
                    if (currSelected >= levels.Count) {
                        currSelected = 0;
                    }
                    MovePortraits();
                });
            }

            public LevelInfo GetSelection() {
                return levels[currSelected];
            }

            public override Transform GetTransform() {
                return transform;
            }

            public override Vector3 Offset() {
                return Vector3.forward;
            }

            public override float Width() {
                return 0.0f;
            }

            private void MovePortraits() {
                levelDisplays[currSelected].position = forwardLevelPortrait.position;
                levelDisplays[currSelected].rotation = forwardLevelPortrait.rotation;
                levelDisplays[currSelected].localScale = forwardLevelPortrait.localScale;
                levelName.text = levels[currSelected].name;

                for (int n = 1; n < levels.Count; ++n) {
                    int index = (currSelected + n) % levels.Count;
                    levelDisplays[index].position = queueFront.position + queueOffset * (n - 1);
                    levelDisplays[index].rotation = queueFront.rotation;
                    levelDisplays[index].localScale = queueFront.localScale;
                }
            }
        }
    }
}