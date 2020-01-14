using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Character {
        public class FrameState {
            public List<HitBox> hitBoxes { get; private set; }
            public bool activateHitBox { get; private set; }
            public bool chainCancellable { get; private set; }
            public bool specialCancellable { get; private set; }
            public int hitDamage { get; private set; }
            public int blockDamage { get; private set; }
            public float hitStun { get; private set; }
            public float blockStun { get; private set; }

            public FrameState(List<HitBox> hitBoxes, bool activateHitBox, bool chainCancellable, bool specialCancellable, int hitDamage, int blockDamage, float hitStun, float blockStun) {
                this.hitBoxes = hitBoxes;

                this.activateHitBox = activateHitBox;
                this.chainCancellable = chainCancellable;
                this.specialCancellable = specialCancellable;
                this.hitDamage = hitDamage;
                this.blockDamage = blockDamage;
                this.hitStun = hitStun;
                this.blockStun = blockStun;
            }

            public void Perform() {
                for (int n = 0; n < hitBoxes.Count; ++n) {
                    hitBoxes[n].Active();
                }

                // TODO: Add call to turn on the sound effect
            }
        }
    }
}