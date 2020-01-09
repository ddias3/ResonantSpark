using System;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Builder {
        public interface IAttackCallbackObj {
            IAttackCallbackObj Name(string name);
            IAttackCallbackObj Orientation(Character.Orientation orientation);
            IAttackCallbackObj GroundRelation(Character.GroundRelation groundRelation);
            IAttackCallbackObj Input(Input.InputNotation input);
            IAttackCallbackObj AnimationState(string animStateName);
            IAttackCallbackObj Frames(Action<IFrameCallbackObject> callback);
        }
    }
}