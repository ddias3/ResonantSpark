using System;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Builder {
        public interface IFrameListBuilder {
            IFrameListBuilder SpecialCancellable(bool specialCancellable);
            IFrameListBuilder From(int startFrame);
            IFrameListBuilder ChainCancellable(bool chainCancellable);
            IFrameListBuilder To(int endFrame);
            IFrameListBuilder HitBox(Action<IHitBoxBuilder> callback);
            IFrameListBuilder HitBox(Character.HitBox hitBox);
            IFrameListBuilder HitBox();
            IFrameListBuilder HitDamage(int damage);
            IFrameListBuilder BlockDamage(int damage);
            IFrameListBuilder HitStun(float frames);
            IFrameListBuilder BlockStun(float frame);
        }
    }
}
