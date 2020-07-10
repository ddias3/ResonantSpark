using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Service;

namespace ResonantSpark {
    namespace Gameplay {
        [RequireComponent(typeof(RigidbodyFG))]
        public abstract class InGameEntity : MonoBehaviour, IEquatable<InGameEntity> {
            public static int entityCounter = 0;

            public InGameEntity parent { protected set; get; }

            public int id { get; private set; }
            public RigidbodyFG rigidFG { private set; get; }

            public Quaternion toLocal {
                get { return Quaternion.Inverse(rigidFG.rotation); }
            }

            public Vector3 position {
                get { return rigidFG.position; }
                set { rigidFG.position = value; }
            }

            public Vector3 velocity {
                get { return rigidFG.velocity; }
            }

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
            public abstract ComboState GetComboState();
        }
    }
}