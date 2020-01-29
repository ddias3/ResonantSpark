using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Gameplay;

namespace ResonantSpark {
    namespace Service {
        public class HitBoxService : MonoBehaviour, IHitBoxService {
            public Transform hitBoxEmpty;
            public HitBox hitBoxDefaultPrefab;

            private FrameEnforcer frame;

            private Dictionary<int, HitBox> hitBoxMap;
            private List<HitBox> previousActiveHitBoxes;
            private List<HitBox> activeHitBoxes;

            public void Start() {
                frame = GameObject.FindGameObjectWithTag("rspTime").GetComponent<FrameEnforcer>();
                frame.AddUpdate((int) FramePriority.Service, new System.Action<int>(FrameUpdate));
                frame.AddUpdate((int) FramePriority.ActivePollingReset, new System.Action<int>(ResetActivePolling));

                hitBoxMap = new Dictionary<int, HitBox>();
                activeHitBoxes = new List<HitBox>();
                previousActiveHitBoxes = new List<HitBox>();

                EventManager.TriggerEvent<Events.ServiceReady, Type>(typeof(HitBoxService));
            }

                // There is a better way to do this.
            //private void FrameUpdate(int frameIndex) {
            //    foreach (KeyValuePair<int, HitBox> kvp in hitBoxMap) {
            //        HitBox hitBox = kvp.Value;
            //        if (activeHitBoxes.Contains(hitBox)) {
            //            if (!hitBox.IsActive()) {
            //                hitBox.Activate();
            //            }
            //        }
            //        else {
            //            if (hitBox.IsActive()) {
            //                hitBox.Deactivate();
            //            }
            //        }
            //    }
            //}

            private void FrameUpdate(int frameIndex) {
                foreach (HitBox hitBox in activeHitBoxes) {
                    if (!previousActiveHitBoxes.Contains(hitBox)) {
                        if (!hitBox.IsActive()) {
                            hitBox.Activate();
                        }
                    }
                }

                foreach (HitBox hitBox in previousActiveHitBoxes) {
                    if (!activeHitBoxes.Contains(hitBox)) {
                        if (hitBox.IsActive()) {
                            hitBox.Deactivate();
                        }
                    }
                }
            }

            //private void ResetActivePolling(int frameIndex) {
            //    previousActiveHitBoxes = activeHitBoxes;
            //    activeHitBoxes = new List<HitBox>();
            //}

            private void ResetActivePolling(int frameIndex) {
                previousActiveHitBoxes.Clear();
                foreach (HitBox hitBox in activeHitBoxes) {
                    previousActiveHitBoxes.Add(hitBox);
                }
                activeHitBoxes.Clear();
            }

            public void RegisterHitBox(HitBox hitBox) {
                hitBoxMap.Add(hitBox.id, hitBox);
            }

            public void Active(HitBox hitBox) {
                activeHitBoxes.Add(hitBox);
            }

            public HitBox DefaultPrefab() {
                return hitBoxDefaultPrefab;
            }

            public Transform GetEmptyHoldTransform() {
                return hitBoxEmpty;
            }
        }
    }
}