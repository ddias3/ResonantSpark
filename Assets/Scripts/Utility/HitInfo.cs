using UnityEngine;

using ResonantSpark.Gameplay;

namespace ResonantSpark {
    namespace Utility {
        public struct HitInfo {
            public HitBox hitBox { get; private set; }
            public InGameEntity hitEntity { get; private set; }
            public Vector3 position { get; private set; }
            public int damage { get; private set; }

            public HitInfo(HitBox hitBox, InGameEntity hitEntity, Vector3 position, int damage) {
                this.hitBox = hitBox;
                this.hitEntity = hitEntity;
                this.position = position;
                this.damage = damage;
            }
        }
    }
}