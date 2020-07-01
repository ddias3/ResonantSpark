using ResonantSpark.CharacterProperties;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace UI {
        namespace UIDebug {
            public class AttackInfo : MonoBehaviour {
                public Transform rows;
                public FrameState frameStatePrefab;
                private List<FrameState> frameDisplays;

                public void Awake() {
                    frameDisplays = new List<FrameState>();

                    for (int n = 0; n < 100; ++n) {
                        FrameState frameStateDisplay = Instantiate<FrameState>(frameStatePrefab, rows);
                        frameStateDisplay.name = "Frame[" + n + "]";
                        frameStateDisplay.gameObject.SetActive(false);
                        frameDisplays.Add(frameStateDisplay);
                    }
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
            }
        }
    }
}