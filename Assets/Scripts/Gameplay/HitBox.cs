using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Service;
using ResonantSpark.Utility;
using ResonantSpark.CharacterProperties;
using ResonantSpark.Builder;

namespace ResonantSpark {
    namespace Gameplay {
        public class HitBox : IEquatable<HitBox> {
            private static int hitBoxCounter = 0;

            public int id { get; private set; }
            public Hit hit { get; set; }

            private HitBoxComponent hitBoxMono;

            private bool tracking;
            private List<InGameEntity> hitEntities;

            private Dictionary<string, Action<HitInfo>> eventCallbacks;

            private IHitBoxService hitBoxService;
            private IFightingGameService fgService;

            private Action<IHitBoxCallbackObject> buildCallback;

            public HitBox(Action<IHitBoxCallbackObject> buildCallback) {
                this.id = HitBox.hitBoxCounter++;
                this.buildCallback = buildCallback;
            }

            public HitBox Build(AllServices services, Hit hit) {
                this.id = HitBox.hitBoxCounter++;

                this.hitBoxService = services.GetService<IHitBoxService>();
                this.fgService = services.GetService<IFightingGameService>();

                this.hit = hit;

                hitEntities = new List<InGameEntity>();
                hitBoxService.RegisterHitBox(this);

                HitBoxBuilder builder = new HitBoxBuilder(services);
                buildCallback(builder);
                hitBoxMono = builder.CreateHitBox(this, hitBoxService.GetEmptyHoldTransform(), OnHitBoxEnter);

                tracking = builder.tracking;
                eventCallbacks = builder.eventCallbacks;

                return this;
            }

            public HitBox Init(Transform relativeTransform, bool tracking, Action<HitInfo> onHurtBoxEnter, Action<HitInfo> onHitBoxEnter) {
                Deactivate();

                return this;
            }

            public void OnHitBoxEnter(InGameEntity gameEntity, Collider other, Vector3 hitLocation) {
                if (gameEntity != null && !hitEntities.Contains(gameEntity)) {
                    hitEntities.Add(gameEntity);

                    HurtBox hurtBox = null;
                    if ((hurtBox = other.gameObject.GetComponent<HurtBox>()) != null) {

                    }
                    HitBox hitBox = null;
                    HitBoxComponent hitBoxComp = null;
                    if ((hitBoxComp = other.gameObject.GetComponent<HitBoxComponent>()) != null) {
                        hitBox = hitBoxComp.hitBox;
                    }

                    HitInfo hitInfo = new HitInfo(this, gameEntity, hitLocation, 1000);// -1);
                    hit.QueueUpEvent(gameEntity.HitBoxEventType(this), this, hitInfo);
                    //hit.InvokeEvent(gameEntity.HitBoxEventType(this), this, hitInfo);
                }
            }

            public void InvokeEvent(string eventName, HitInfo hitInfo) {
                Action<HitInfo> callback;
                if (eventCallbacks.TryGetValue(eventName, out callback)) {
                    callback(hitInfo);
                }
            }

            public void Active() {
                hitBoxService.Active(this);
            }

            public bool IsActive() {
                return hitBoxMono.IsActive();
            }

            public void Activate() {
                hitEntities.Clear();
                hitBoxMono.Activate();
            }

            public void Deactivate() {
                hitBoxMono.Deactivate();
            }

            public bool Equals(HitBox other) {
                return id == other.id;
            }

            public override int GetHashCode() {
                return id;
            }
        }
    }
}