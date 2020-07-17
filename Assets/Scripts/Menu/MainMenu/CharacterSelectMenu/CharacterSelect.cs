using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Character;

namespace ResonantSpark {
    namespace Menu {
        public class CharacterSelect : Selectable {
            public GameObject characterSelectIconPrefab;
            public Cursor3d cursor3d;

            public List<Character.CharacterInfo> charInfos;
            public List<Transform> charPortraitLocations;

            private List<CharacterIconSelectable> selectableIcons;

            private int currSelected;

            public void Start() {
                selectableIcons = new List<CharacterIconSelectable>();
                for (int n = 0; n < charInfos.Count; ++n) {
                    CharacterIconSelectable charIcon = Instantiate(characterSelectIconPrefab,
                            charPortraitLocations[n].position,
                            Quaternion.identity,
                            transform)
                        .GetComponent<CharacterIconSelectable>();
                    charIcon.SetMaterial(charInfos[n].preview);

                    selectableIcons.Add(charIcon);
                }

                currSelected = 1;

                eventHandler.On("activate", () => {
                    cursor3d.Highlight(selectableIcons[currSelected]);
                    for (int n = 0; n < selectableIcons.Count; ++n) {
                        selectableIcons[n].TriggerEvent("activate");
                    }
                });
                eventHandler.On("deactivate", () => {
                    cursor3d.Fade();
                    for (int n = 0; n < selectableIcons.Count; ++n) {
                        selectableIcons[n].TriggerEvent("deactivate");
                    }
                });

                eventHandler.On("up", () => {
                    // These could be used for up and down, but there is only 1 character
                });

                eventHandler.On("down", () => {
                    // These could be used for up and down, but there is only 1 character
                });

                eventHandler.On("left", () => {
                    currSelected -= 1;
                    if (currSelected < 0) {
                        currSelected = 2;
                    }
                    cursor3d.Highlight(selectableIcons[currSelected]);
                });

                eventHandler.On("right", () => {
                    currSelected += 1;
                    if (currSelected > 2) {
                        currSelected = 0;
                    }
                    cursor3d.Highlight(selectableIcons[currSelected]);
                });
            }

            public void FadeCursor() {
                cursor3d.Fade();
            }

            public Material GetPlayerPortrait() {
                return charInfos[currSelected].preview;
            }

            public string GetPlayerName() {
                return charInfos[currSelected].name;
            }

            public override Transform GetTransform() {
                return transform;
            }

            public override Vector3 Offset() {
                return Vector3.forward;
            }

            public override float Width() {
                return 2.0f;
            }
        }
    }
}