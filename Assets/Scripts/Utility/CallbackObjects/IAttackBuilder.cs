using System;
using System.Collections.Generic;

using ResonantSpark.Character;
using ResonantSpark.CharacterProperties;

namespace ResonantSpark {
    namespace Builder {
        public interface IAttackCallbackObj {
            IAttackCallbackObj Name(string name);
            IAttackCallbackObj Orientation(Orientation orientation);
            IAttackCallbackObj GroundRelation(GroundRelation groundRelation);
            IAttackCallbackObj Input(Input.InputNotation input);
            IAttackCallbackObj AnimationState(string animStateName);
            IAttackCallbackObj Frames((List<FrameStateBuilder> frameList, Dictionary<int, Action<IHitBoxCallbackObject>> hitBoxCallbackMap) framesInfo);
        }
    }
}