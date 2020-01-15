using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Character;

namespace ResonantSpark {
    namespace Service {
        public class HitBoxService : MonoBehaviour, IHitBoxService {
            public Transform hitBoxEmpty;

            private FrameEnforcer frame;

            private Dictionary<int, HitBox> hitBoxMap;
            private List<HitBox> activeHitBoxes;

            public void Start() {
                frame = GameObject.FindGameObjectWithTag("rspTime").GetComponent<FrameEnforcer>();
                frame.AddUpdate((int)FramePriority.Service, new System.Action<int>(FrameUpdate));

                hitBoxMap = new Dictionary<int, HitBox>();
                activeHitBoxes = new List<HitBox>();
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
                throw new System.NotImplementedException();
            }

            public Transform GetEmptyHoldTransform() {
                throw new System.NotImplementedException();
            }
        }
    }
}