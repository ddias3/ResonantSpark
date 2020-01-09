using System;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace CharacterStates {
        public class AttackTracker : StateHelper {

            public Action onCompleteAttackCallback;

            private int startFrame;
            private int frameCount;

            private CharacterProperties.Attack currentAttack;

            public new void Start() {
                base.Start();
            }

            public void TrackAttack(int frameIndex, CharacterProperties.Attack attack) {
                startFrame = frameIndex;
                frameCount = 0;

                currentAttack = attack;
            }

            public void ClearAttack() {
                TrackAttack(-1, null);
            }

            public void IncrementTracker() {
                currentAttack.RunFrame(frameCount);
                frameCount++;
            }
        }
    }
}