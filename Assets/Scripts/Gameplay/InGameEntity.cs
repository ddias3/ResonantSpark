using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using ResonantSpark.Service;

namespace ResonantSpark {
    namespace Gameplay {
        [RequireComponent(typeof(RigidbodyFG))]
        public abstract class InGameEntity : MonoBehaviour, IPredeterminedActions, IEquatable<InGameEntity> {
            public static int entityCounter = 0;

            public InGameEntity parent { protected set; get; }
            public bool isAttached { protected set; get; }
            public List<InGameEntity> attachedEntities { protected set; get; }

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

            protected Dictionary<Type, UnityEvent> onAttachCallbacks;
            protected Dictionary<Type, UnityEvent> onDetachCallbacks;

            public void Init() {
                this.id = entityCounter++;
                attachedEntities = new List<InGameEntity>();
                onAttachCallbacks = new Dictionary<Type, UnityEvent>();
                onDetachCallbacks = new Dictionary<Type, UnityEvent>();
                rigidFG = GetComponent<RigidbodyFG>();
            }

            public bool Equals(InGameEntity other) {
                return id == other.id;
            }

            public override int GetHashCode() {
                return id;
            }

            public void RunAttachedEntities() {
                foreach (InGameEntity entity in attachedEntities) {
                    entity.AttachedFollow(this);
                }
            }

            public virtual void AttachedFollow(InGameEntity attachedTo) {
                // do nothing
            }

            public void Attach(InGameEntity entity) {
                attachedEntities.Add(entity);
                if (onAttachCallbacks.TryGetValue(entity.GetType(), out UnityEvent callbacks)) {
                    callbacks.Invoke();
                }
            }

            public void Detach(InGameEntity entity) {
                attachedEntities.Remove(entity);
                if (onDetachCallbacks.TryGetValue(entity.GetType(), out UnityEvent callbacks)) {
                    callbacks.Invoke();
                }
            }

            public abstract void AddSelf();
            public abstract void RemoveSelf();
            public abstract ComboState GetComboState();

            public abstract void PredeterminedActions(string actionName);
            public abstract void PredeterminedActions(string actionName, params object[] objs);
        }
    }
}