using System;
using System.Collections.Generic;

using ResonantSpark.Builder;
using ResonantSpark.Character;
using ResonantSpark.CharacterProperties;

namespace ResonantSpark {
    namespace Utility {
        public static class FrameUtil {

            public static class Default {
                public static bool chainCancellable = true;
                public static bool specialCancellable = true;
                public static int hitDamage = 800;
                public static int blockDamage = 0;
                public static float hitStun = 20.0f;
                public static float blockStun = 10.0f;
                public static int startFrame = 0;

                public static List<HitBox> hitBoxes = null;

                public static int counter = 0;
            }

            public static List<FrameState> CreateList(Action<IFrameListCallbackObj> callback) {
                FrameListBuilder builder = new FrameListBuilder();
                callback(builder);
                return builder.CreateFrameList();
            }
        }
    }
}