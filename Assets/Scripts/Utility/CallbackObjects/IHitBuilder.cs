using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Builder;
using ResonantSpark.Character;

namespace ResonantSpark {
    namespace Builder {
        public interface IHitCallbackObject {
            IHitCallbackObject Block(params Block[] requiredBlocks);
            IHitCallbackObject HitDamage(int damage);
            IHitCallbackObject BlockDamage(int damage);
            IHitCallbackObject HitStun(float frames);
            IHitCallbackObject BlockStun(float frames);
            IHitCallbackObject ComboScaling(float scaling);
            IHitCallbackObject Priority(AttackPriority priority);
            IHitCallbackObject HitBox(Action<IHitBoxCallbackObject> callback);
        }
    }

    namespace CharacterProperties {
        public partial class HitBuilder : IHitCallbackObject {
            public IHitCallbackObject Block(params Block[] requiredBlocks) {
                throw new NotImplementedException();
            }

            public IHitCallbackObject BlockDamage(int damage) {
                throw new NotImplementedException();
            }

            public IHitCallbackObject BlockStun(float frames) {
                throw new NotImplementedException();
            }

            public IHitCallbackObject ComboScaling(float scaling) {
                throw new NotImplementedException();
            }

            public IHitCallbackObject HitBox(Action<IHitBoxCallbackObject> callback) {
                throw new NotImplementedException();
            }

            public IHitCallbackObject HitDamage(int damage) {
                throw new NotImplementedException();
            }

            public IHitCallbackObject HitStun(float frames) {
                throw new NotImplementedException();
            }

            public IHitCallbackObject Priority(AttackPriority priority) {
                throw new NotImplementedException();
            }
        }
    }
}