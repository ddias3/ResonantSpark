using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.CharacterProperties;
using ResonantSpark.Service;

namespace ResonantSpark {
    namespace UI {
        namespace UIDebug {
            public class AttackInfo : GameUiElement {
                public UiService uiService;

                public Transform rows;
                public FrameState frameStatePrefab;
                private List<FrameState> frameDisplays;

                public new void Awake() {
                    base.Awake();

                    frameDisplays = new List<FrameState>();

                    for (int n = 0; n < 100; ++n) {
                        FrameState frameStateDisplay = Instantiate<FrameState>(frameStatePrefab, rows);
                        frameStateDisplay.name = "Frame[" + n + "]";
                        frameStateDisplay.gameObject.SetActive(false);
                        frameDisplays.Add(frameStateDisplay);
                    }

                    uiService.RegisterElement("attackInfo", this);
                }

                public void DisplayAttack(Attack attack) {
                    List<Character.FrameState> frames = attack.frames;

                    for (int n = 0; n < frames.Count && n < frameDisplays.Count; ++n) {
                        frameDisplays[n].gameObject.SetActive(true);

                        frameDisplays[n].hit = frames[n].hits.Count > 0;
                        frameDisplays[n].track = frames[n].trackCallback != null;
                        frameDisplays[n].sound = frames[n].soundCallback != null || frames[n].soundClip != null;
                        frameDisplays[n].projectile = frames[n].projectileCallback != null;
                        frameDisplays[n].chainCancellable = frames[n].chainCancellable;
                        frameDisplays[n].specialCancellable = frames[n].specialCancellable;
                        frameDisplays[n].counterHit = frames[n].counterHit;
                    }
                }

                public void ClearDisplay() {
                    for (int n = 0; n < frameDisplays.Count; ++n) {
                        frameDisplays[n].gameObject.SetActive(false);
                    }
                }

                public override void SetValue(string field) {
                    switch (field) {
                        case "clear":
                            ClearDisplay();
                            break;
                        default:
                            throw new InvalidOperationException("Attack info field invalid: " + field);
                    }
                }

                public override void SetValue(string field, object value0) {
                    switch (field) {
                        case "display":
                            DisplayAttack((Attack) value0);
                            break;
                        default:
                            throw new InvalidOperationException("Attack info field with 1 value invalid: " + field);
                    }
                }

                public override void SetValue(string field, object value0, object value1) {
                    switch (field) {
                        default:
                            throw new InvalidOperationException("Attack info field with 2 values invalid: " + field);
                    }
                }
            }
        }
    }
}