using System;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Gameplay {
        public abstract class InGameEntity : MonoBehaviour, IInGameEntity, IEquatable<InGameEntity> {
            public static int entityCounter = 0;

            public int id { get; private set; }

            public void Init() {
                this.id = entityCounter++;
            }

            public bool Equals(InGameEntity other) {
                return id == other.id;
            }

            public override int GetHashCode() {
                return id;
            }

            public abstract void GetHitBy(HitBox hitBox);
            public abstract void AddSelf();
            public abstract void RemoveSelf();
        }
    }
}