using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Character;
using ResonantSpark.Service;
using ResonantSpark.CharacterProperties;

namespace ResonantSpark {
    namespace Gameplay {
        public class Hit : IEquatable<Hit> {
            private static int hitCounter = 0;
            public int id { get; private set; }

            public List<BlockType> validBlocks { get; private set; }
            public AttackPriority priority { get; private set; }
            public int hitDamage { get; private set; }
            public int blockDamage { get; private set; }
            public float hitStun { get; private set; }
            public float blockStun { get; private set; }
            public float comboScaling { get; private set; }
            public bool tracking { get; private set; }

            private List<InGameEntity> hitEntities;

            private List<HitBox> hitBoxes;
            private Action<Hit, Dictionary<HitBox, Vector3>, InGameEntity, List<HurtBox>, List<HitBox>> onHitCallback;

            private Dictionary<HitBox, List<HurtBox>> hitHurtBoxes;
            private Dictionary<HitBox, List<HitBox>> hitHitBoxes;
            private Dictionary<HitBox, Vector3> hitLocation;
            private List<(HitBox, Vector3, HurtBox, HitBox)> hitBoxQueue;

            private IHitService hitService;
            private IFightingGameService fgService;

            public Hit(AllServices services) {
                this.id = Hit.hitCounter++;

                this.hitService = services.GetService<IHitService>();
                this.fgService = services.GetService<IFightingGameService>();

                this.hitEntities = new List<InGameEntity>();
                this.hitBoxes = new List<HitBox>();

                hitHurtBoxes = new Dictionary<HitBox, List<HurtBox>>();
                hitHitBoxes = new Dictionary<HitBox, List<HitBox>>();
                hitLocation = new Dictionary<HitBox, Vector3>();
                hitBoxQueue = new List<(HitBox, Vector3, HurtBox, HitBox)>();

                hitService.RegisterHit(this);
            }

            public void Build(HitBuilder hitBuilder) {
                this.onHitCallback = hitBuilder.onHitCallback;

                this.priority = hitBuilder.priority;
                this.hitDamage = hitBuilder.hitDamage;
                this.blockDamage = hitBuilder.blockDamage;
                this.hitStun = hitBuilder.hitStun;
                this.blockStun = hitBuilder.blockStun;
                this.comboScaling = hitBuilder.comboScaling;
                this.tracking = hitBuilder.tracking;

                this.validBlocks = new List<BlockType>();
                foreach (BlockType block in Enum.GetValues(typeof(BlockType))) {
                    if (!hitBuilder.requiredBlocks.Contains(block) && block != BlockType.AIR) { // Allow air blocking in future version of the game.
                        this.validBlocks.Add(block);
                    }
                }
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
                    if (hitBoxQueue[n].Item3 != null) {
                        hitHurtBoxes[hitBoxQueue[n].Item1].Add(hitBoxQueue[n].Item3);
                        hitLocation[hitBoxQueue[n].Item1] = hitBoxQueue[n].Item2;
                    }
                    if (hitBoxQueue[n].Item4 != null) {
                        hitHitBoxes[hitBoxQueue[n].Item1].Add(hitBoxQueue[n].Item4);
                        hitLocation[hitBoxQueue[n].Item1] = hitBoxQueue[n].Item2;
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
                    if (!hitEntities.Contains(entity)) {
                        if (!entityMap.ContainsKey(entity)) {
                            entityMap.Add(entity, (new List<HurtBox>(), new List<HitBox>()));
                        }
                        entityMap[entity].hurt.Add(hb);
                    }
                }
                foreach (HitBox hb in hitBoxes) {
                    InGameEntity entity = hb.GetEntity();
                    if (!hitEntities.Contains(entity)) {
                        if (!entityMap.ContainsKey(entity)) {
                            entityMap.Add(entity, (new List<HurtBox>(), new List<HitBox>()));
                        }
                        entityMap[entity].hit.Add(hb);
                    }
                }

                    // Adding the entity has to be done after populating the entity map, or else only
                    //  1 hurtbox/hitbox will be added to the entity map
                foreach (HurtBox hb in hurtBoxes) {
                    InGameEntity entity = hb.GetEntity();
                    if (!hitEntities.Contains(entity)) {
                        hitEntities.Add(entity);
                    }
                }
                foreach (HitBox hb in hitBoxes) {
                    InGameEntity entity = hb.GetEntity();
                    if (!hitEntities.Contains(entity)) {
                        hitEntities.Add(entity);
                    }
                }

                return entityMap;
            }

            public void InvokeCallback(InGameEntity entity, List<HurtBox> hurt, List<HitBox> hit) {
                    //TODO: (Maybe) Have a reference to the entity that owns this hit.
                onHitCallback?.Invoke(this, hitLocation, entity, hurt, hit);
            }

            public void ClearHitQueue() {
                hitBoxQueue.Clear();
                hitLocation.Clear();
                foreach (KeyValuePair<HitBox, List<HurtBox>> kvp in hitHurtBoxes) {
                    kvp.Value.Clear();
                }
                foreach (KeyValuePair<HitBox, List<HitBox>> kvp in hitHitBoxes) {
                    kvp.Value.Clear();
                }
            }

            public void ClearHitEntities() {
                hitEntities.Clear();
            }

            public bool Equals(Hit other) {
                return id == other.id;
            }

            public override int GetHashCode() {
                return id;
            }
        }
    }
}