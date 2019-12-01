using System;
using System.Collections.Generic;
using UnityEngine;


namespace ResonantSpark {
    namespace Builder {
        public interface IAttackBuilder {
            IAttackBuilder Name(string name);
            IAttackBuilder Orientation(Character.Orientation orientation);
            IAttackBuilder Input(Input.InputNotation input);
            IAttackBuilder AnimationState(string animStateName);
            IAttackBuilder Frames(Action<IFrameBuilder> callback);
        }
    }
}