using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Character;
using ResonantSpark.Service;

namespace ResonantSpark {
    namespace Gameplay {
        public class Hit : IEquatable<Hit> {
            private static int hitCounter = 0;
            public int id { get; private set; }

            public AttackPriority priority { get; private set; }

            private List<InGameEntity> hitEntities;

            private List<HitBox> hitBoxes;
            private Dictionary<string, Action<List<HitBox>, HitInfo>> hitEventCallbacks;

            private Dictionary<HitBox, List<HurtBox>> hitHurtBoxes;
            private Dictionary<HitBox, List<HitBox>> hitHitBoxes;
            private Dictionary<HitBox, Vector3> hitBoxLocation;
            private List<(HitBox, Vector3, HurtBox, HitBox)> hitBoxQueue;

            private IHitService hitService;
            private IFightingGameService fgService;

            public Hit(AllServices services, Dictionary<string, Action<List<HitBox>, HitInfo>> hitEventCallbacks) {
                this.id = Hit.hitCounter++;
                this.hitService = services.GetService<IHitService>();
                this.fgService = services.GetService<IFightingGameService>();

                this.hitEntities = new List<InGameEntity>();

                this.hitBoxes = new List<HitBox>();
                this.hitEventCallbacks = hitEventCallbacks;

                hitHurtBoxes = new Dictionary<HitBox, List<HurtBox>>();
                hitHitBoxes = new Dictionary<HitBox, List<HitBox>>();
                hitBoxLocation = new Dictionary<HitBox, Vector3>();
                hitBoxQueue = new List<(HitBox, Vector3, HurtBox, HitBox)>();

                hitService.RegisterHit(this);

                PopulateEventCallbacks();
            }

            public void AddHitBox(HitBox hitBox) {
                hitBoxes.Add(hitBox);
                hitHurtBoxes.Add(hitBox, new List<HurtBox>());
            }

            public void Active() {
                foreach (HitBox hitBox in hitBoxes) {
                    hitBox.Active();
                }
            }

            public void RegisterHitInfo(HitBox hitBox, Vector3 hitLocation, HurtBox hurtBox) {
                if (!hitBoxes.Contains(hitBox)) {
                    throw new InvalidOperationException("Attempting to invoke a hitbox event to a hit it doesn't belongs to");
                }

                hitBoxQueue.Add((hitBox, hitLocation, hurtBox, null));
            }

            public void RegisterHitInfo(HitBox hitBox, Vector3 hitLocation, HitBox otherHitBox) {
                if (!hitBoxes.Contains(hitBox)) {
                    throw new InvalidOperationException("Attempting to invoke a hitbox event to a hit it doesn't belongs to");
                }

                hitBoxQueue.Add((hitBox, hitLocation, null, otherHitBox));
            }

            public Dictionary<InGameEntity, (List<HurtBox> hurt, List<HitBox> hit)> ParseHitInfo() {
                if (hitBoxQueue.Count == 0) {
                    return null;
                }

                for (int n = 0; n < hitBoxQueue.Count; ++n) {
                    if (hitBoxQueue[n].Item2 != null) {
                        hitHurtBoxes[hitBoxQueue[n].Item1].Add(hitBoxQueue[n].Item3);
                        hitBoxLocation[hitBoxQueue[n].Item1] = hitBoxQueue[n].Item2;
                    }
                    if (hitBoxQueue[n].Item3 != null) {
                        hitHitBoxes[hitBoxQueue[n].Item1].Add(hitBoxQueue[n].Item4);
                        hitBoxLocation[hitBoxQueue[n].Item1] = hitBoxQueue[n].Item2;
                    }
                }

                HashSet<HurtBox> hurtBoxes = new HashSet<HurtBox>();
                HashSet<HitBox> hitBoxes = new HashSet<HitBox>();

                foreach (KeyValuePair<HitBox, List<HurtBox>> kvp in hitHurtBoxes) {
                    HitBox hitBox = kvp.Key;
                    for (int n = 0; n < kvp.Value.Count; ++n) {
                        hurtBoxes.Add(kvp.Value[n]);
                    }
                }
                foreach (KeyValuePair<HitBox, List<HitBox>> kvp in hitHitBoxes) {
                    HitBox hitBox = kvp.Key;
                    for (int n = 0; n < kvp.Value.Count; ++n) {
                        hitBoxes.Add(kvp.Value[n]);
                    }
                }

                Dictionary<InGameEntity, (List<HurtBox> hurt, List<HitBox> hit)> entityMap = new Dictionary<InGameEntity, (List<HurtBox>, List<HitBox>)>();

                foreach (HurtBox hb in hurtBoxes) {
                    InGameEntity entity = hb.GetEntity();
                    if (!entityMap.ContainsKey(entity)) {
                        entityMap.Add(entity, (new List<HurtBox>(), new List<HitBox>()));
                    }
                    entityMap[entity].hurt.Add(hb);
                }
                foreach (HitBox hb in hitBoxes) {
                    InGameEntity entity = hb.GetEntity();
                    if (!entityMap.ContainsKey(entity)) {
                        entityMap.Add(entity, (new List<HurtBox>(), new List<HitBox>()));
                    }
                    entityMap[entity].hit.Add(hb);
                }

                Clear();

                return entityMap;
            }

            public void InvokeEvent(string eventName, HitBox hitBox, HitInfo hitInfo) {
                if (!hitBoxes.Contains(hitBox)) {
                    throw new InvalidOperationException("Attempting to invoke a hitbox event to a hit it doesn't belongs to");
                }

                Action<List<HitBox>, HitInfo> callback = this.hitEventCallbacks[eventName];
                callback(new List<HitBox> { hitBox }, hitInfo);
            }

            public void Clear() {
                hitBoxQueue.Clear();
                hitBoxLocation.Clear();
                foreach (KeyValuePair<HitBox, List<HurtBox>> kvp in hitHurtBoxes) {
                    kvp.Value.Clear();
                }
                foreach (KeyValuePair<HitBox, List<HitBox>> kvp in hitHitBoxes) {
                    kvp.Value.Clear();
                }
            }

            public bool Equals(Hit other) {
                return id == other.id;
            }

            public override int GetHashCode() {
                return id;
            }

            private void PopulateEventCallbacks() {
                List<string> eventNames = new List<string> {
                    "onHitFGChar",
                    "onEqualPriorityHitBox",
                    "onHitProjectile",
                };

                foreach (string eventName in eventNames) {
                    if (!hitEventCallbacks.ContainsKey(eventName)) {
                        hitEventCallbacks.Add(eventName, DefaultEventHandler(eventName));
                    }
                }
            }

            private Action<List<HitBox>, HitInfo> DefaultEventHandler(string eventName) {
                return (List<HitBox> hitBoxes, HitInfo hitInfo) => {
                    hitBoxes[0].InvokeEvent(eventName, hitInfo);
                };
            }
        }
    }
}