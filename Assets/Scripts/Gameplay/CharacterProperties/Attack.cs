using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Character;

namespace ResonantSpark {
    namespace CharacterProperties {
        public class Attack : ScriptableObject {
            private new string name;
            private FrameState[] frames;

            public Attack(Action<Builder.IAttackCallbackObj> builderCallback) {
                AttackBuilder atkBuilder = new AttackBuilder();
                builderCallback(atkBuilder);

                name = atkBuilder.name;
                frames = atkBuilder.GetFrames();
            }
        }
    }
}