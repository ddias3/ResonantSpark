using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Input.Combinations;
using ResonantSpark.Input;
using ResonantSpark.Character;
using ResonantSpark.Utility;
using ResonantSpark.Gameplay;

namespace ResonantSpark {
    namespace CharacterStates {
        public abstract class Attack : CharacterBaseState {

            protected Action onCompleteAttack;

            protected CharacterProperties.Attack activeAttack;
            protected CharacterProperties.Attack queuedUpAttack;

            public override void AnimatorMove(Quaternion animatorRootRotation, Vector3 animatorVelocity) {
                fgChar.SetRelativeVelocity(Gameplay.VelocityPriority.Dash, animatorVelocity);
            }

            public void OnCompleteAttack() {
                activeAttack = null;
            }

            public void SetActiveAttack(CharacterProperties.Attack atk) {
                queuedUpAttack = atk;
            }
        }
    }
}