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
            private List<HitBox> activeHitBoxes;

            public void Start() {
                frame = GameObject.FindGameObjectWithTag("rspTime").GetComponent<FrameEnforcer>();
                frame.AddUpdate((int)FramePriority.Service, new System.Action<int>(FrameUpdate));

                hitBoxMap = new Dictionary<int, HitBox>();
                activeHitBoxes = new List<HitBox>();

                EventManager.TriggerEvent<Events.ServiceReady, Type>(typeof(HitBoxService));
            }

            private void FrameUpdate(int frameIndex) {
                //activeHitBoxes.Clear();
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