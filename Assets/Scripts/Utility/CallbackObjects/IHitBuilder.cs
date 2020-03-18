using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Builder;
using ResonantSpark.Character;
using ResonantSpark.Utility;
using ResonantSpark.Gameplay;

namespace ResonantSpark {
    namespace Builder {
        public interface IHitCallbackObject {
            IHitCallbackObject Block(params Block[] requiredBlocks);
            IHitCallbackObject HitDamage(int damage);
            IHitCallbackObject BlockDamage(int damage);
            IHitCallbackObject HitStun(float frames);
            IHitCallbackObject BlockStun(float frames);
            IHitCallbackObject ComboScaling(float scaling);
            IHitCallbackObject Tracking(bool tracking);
            IHitCallbackObject Priority(AttackPriority priority);
            IHitCallbackObject Event(string eventName, Action<HitBox, HitInfo> callback);
            IHitCallbackObject HitBox(HitBox hitBox);
        }
    }

    namespace CharacterProperties {
        public partial class HitBuilder : IHitCallbackObject {
            public List<Block> requiredBlocks { get; private set; }
            public AttackPriority priority { get; private set; }
            public int hitDamage { get; private set; }
            public int blockDamage { get; private set; }
            public float hitStun { get; private set; }
            public float blockStun { get; private set; }
            public float comboScaling { get; private set; }
            public bool tracking { get; private set; }
            public Dictionary<string, Action<HitBox, HitInfo>> eventCallbacks { get; private set; }

            public IHitCallbackObject Block(params Block[] requiredBlocks) {
                this.requiredBlocks = new List<Block>(requiredBlocks);
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
            public IHitCallbackObject Event(string eventName, Action<HitBox, HitInfo> callback) {
                this.eventCallbacks.Add(eventName, callback);
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