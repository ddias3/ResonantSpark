using System;
using System.Collections.Generic;

using ResonantSpark.Builder;
using ResonantSpark.CharacterProperties;

namespace ResonantSpark {
    namespace Utility {
        public static class FrameUtil {

            public static class Default {
                public static bool specialCancellable = true;
                public static bool chainCancellable = true;
                public static bool cancellableOnWhiff = false;
                public static bool counterHit = true;
                public static int startFrame = 0;

                public static int counter = 0;
            }

            public static (List<FrameStateBuilder>, Dictionary<int, Action<IHitCallbackObject>>) CreateList(Action<IFrameListCallbackObj> callback) {
                FrameListBuilder builder = new FrameListBuilder();
                callback(builder);
                return builder.CreateFrameList();
            }
        }
    }
}