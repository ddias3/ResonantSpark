using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Builder;
using ResonantSpark.Character;
using ResonantSpark.Gameplay;

namespace ResonantSpark {
    namespace Builder {
        public interface IHitCallbackObject {
            IHitCallbackObject Block(params BlockType[] requiredBlocks);
            IHitCallbackObject HitDamage(int damage);
            IHitCallbackObject BlockDamage(int damage);
            IHitCallbackObject HitStun(float frames);
            IHitCallbackObject BlockStun(float frames);
            IHitCallbackObject ComboScaling(float scaling);
            IHitCallbackObject Tracking(bool tracking);
            IHitCallbackObject Priority(AttackPriority priority);
            IHitCallbackObject OnHit(Action<Hit, Dictionary<HitBox, Vector3>, InGameEntity, List<HurtBox>, List<HitBox>> callback);
            IHitCallbackObject HitBox(HitBox hitBox);
        }
    }

    namespace CharacterProperties {
        public partial class HitBuilder : IHitCallbackObject {
            public List<BlockType> requiredBlocks { get; private set; }
            public AttackPriority priority { get; private set; }
            public int hitDamage { get; private set; }
            public int blockDamage { get; private set; }
            public float hitStun { get; private set; }
            public float blockStun { get; private set; }
            public float comboScaling { get; private set; }
            public bool tracking { get; private set; }
            public Action<Hit, Dictionary<HitBox, Vector3>, InGameEntity, List<HurtBox>, List<HitBox>> onHitCallback { get; private set; }

            public IHitCallbackObject Block(params BlockType[] requiredBlocks) {
                this.requiredBlocks = new List<BlockType>(requiredBlocks);
                return this;
            }
            public IHitCallbackObject BlockDamage(int blockDamage) {
                this.blockDamage = blockDamage;
                return this;
            }
            public IHitCallbackObject BlockStun(float blockStun) {
                this.blockStun = blockStun;
                return this;
            }
            public IHitCallbackObject ComboScaling(float comboScaling) {
                this.comboScaling = comboScaling;
                return this;
            }
            public IHitCallbackObject Tracking(bool tracking) {
                this.tracking = tracking;
                return this;
            }
            public IHitCallbackObject OnHit(Action<Hit, Dictionary<HitBox, Vector3>, InGameEntity, List<HurtBox>, List<HitBox>> callback) {
                this.onHitCallback = callback;
                return this;
            }
            public IHitCallbackObject HitBox(HitBox hitBox) {
                hitBoxes.Add(hitBox);
                return this;
            }
            public IHitCallbackObject HitDamage(int hitDamage) {
                this.hitDamage = hitDamage;
                return this;
            }
            public IHitCallbackObject HitStun(float hitStun) {
                this.hitStun = hitStun;
                return this;
            }
            public IHitCallbackObject Priority(AttackPriority priority) {
                this.priority = priority;
                return this;
            }
        }
    }
}