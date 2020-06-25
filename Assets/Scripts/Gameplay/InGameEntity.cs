using System;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Gameplay {
        [RequireComponent(typeof(RigidbodyFG))]
        public abstract class InGameEntity : MonoBehaviour, IInGameEntity, IEquatable<InGameEntity> {
            public static int entityCounter = 0;

            public int id { get; private set; }
            public RigidbodyFG rigidFG { private set; get; }

            public void Init() {
                this.id = entityCounter++;
                rigidFG = GetComponent<RigidbodyFG>();
            }

            public bool Equals(InGameEntity other) {
                return id == other.id;
            }

            public override int GetHashCode() {
                return id;
            }

            public abstract string HitBoxEventType(HitBox hitBox);
            public abstract void AddSelf();
            public abstract void RemoveSelf();
        }
    }
}