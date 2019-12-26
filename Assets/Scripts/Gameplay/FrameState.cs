using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Character {
        public class FrameState {
            public HitBox[] hitBoxes { get; private set; }
            public AnimationClip animationClip { get; private set; }

            public FrameState Init(HitBox[] hitBoxes, AnimationClip anim) {
                this.hitBoxes = hitBoxes;
                this.animationClip = anim;
                return this;
            }
        }
    }
}