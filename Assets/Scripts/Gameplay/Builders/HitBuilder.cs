using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Builder;
using ResonantSpark.Service;
using ResonantSpark.Gameplay;
using ResonantSpark.Character;

namespace ResonantSpark {
    namespace CharacterProperties {
        public partial class HitBuilder : IHitCallbackObject {
            private AllServices services;

            private IHitBoxService hitBoxService;

            private List<HitBox> hitBoxes;

            public HitBuilder(AllServices allServices) {
                this.services = allServices;
                hitBoxService = allServices.GetService<IHitBoxService>();

                this.tracking = false;
                this.comboScaling = 1.0f;
                this.onHitCallback = null;
                this.requiredBlocks = new List<Block>();

                hitBoxes = new List<HitBox>();
            }

            public Hit CreateHit() {

                Hit hit = new Hit(services);
                hit.Build(this);

                for (int n = 0; n < hitBoxes.Count; ++n) {
                    HitBox hitBox = hitBoxes[n];
                    hitBox.hit = hit;

                    hitBox.Build(services, hit);
                    hit.AddHitBox(hitBox);
                }

                return hit;
            }
        }
    }
}