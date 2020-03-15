using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Builder;
using ResonantSpark.Service;
using ResonantSpark.Gameplay;

namespace ResonantSpark {
    namespace CharacterProperties {
        public partial class HitBuilder : IHitCallbackObject {

            private AllServices services;

            private IHitBoxService hitBoxService;

            private List<Action<IHitBoxCallbackObject>> hitBoxBuilderCallbacks;

            public HitBuilder(AllServices allServices) {
                this.services = allServices;
                hitBoxService = allServices.GetService<IHitBoxService>();

                this.tracking = false;
                this.comboScaling = 1.0f;

                hitBoxBuilderCallbacks = new List<Action<IHitBoxCallbackObject>>();
            }

            public Hit CreateHit() {

                List<HitBox> hitBoxes = new List<HitBox>();

                for (int n = 0; n < hitBoxBuilderCallbacks.Count; ++n) {
                    Action<IHitBoxCallbackObject> callback = hitBoxBuilderCallbacks[n];

                    HitBoxBuilder builder = new HitBoxBuilder(services);
                    callback(builder);
                    HitBox hitBox = builder.CreateHitBox(hitBoxService.GetEmptyHoldTransform());

                    hitBoxes.Add(hitBox);
                }

                return null;
            }
        }
    }
}