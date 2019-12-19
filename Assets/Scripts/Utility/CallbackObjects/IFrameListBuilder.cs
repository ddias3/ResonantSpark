using System;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Builder {
        public interface IFrameListCallbackObj {
            IFrameListCallbackObj SpecialCancellable(bool specialCancellable);
            IFrameListCallbackObj From(int startFrame);
            IFrameListCallbackObj ChainCancellable(bool chainCancellable);
            IFrameListCallbackObj To(int endFrame);
            IFrameListCallbackObj HitBox(Action<IHitBoxCallbackObject> callback);
            IFrameListCallbackObj HitBox(Character.HitBox hitBox);
            IFrameListCallbackObj HitBox();
            IFrameListCallbackObj HitDamage(int damage);
            IFrameListCallbackObj BlockDamage(int damage);
            IFrameListCallbackObj HitStun(float frames);
            IFrameListCallbackObj BlockStun(float frame);
        }
    }
}
