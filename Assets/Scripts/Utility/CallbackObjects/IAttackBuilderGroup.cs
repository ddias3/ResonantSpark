using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Builder;
using ResonantSpark.Character;
using ResonantSpark.CharacterProperties;
using ResonantSpark.Input;

namespace ResonantSpark {
    namespace Builder {
        public interface IAttackCallbackGroupObj {
            //IAttackCallbackGroupObj Input(params string[] inputStrings);
            IAttackCallbackGroupObj Movement(Func<float, float> xMoveCb = null, Func<float, float> yMoveCb = null, Func<float, float> zMoveCb = null);
            IAttackCallbackGroupObj AnimationState(string animStateName);
            IAttackCallbackGroupObj InitCharState(CharacterStates.Attack attackState);
            IAttackCallbackGroupObj FramesContinuous(Action<float, Vector3> callback);
            IAttackCallbackGroupObj Frames((List<FrameStateBuilder> frameList, Dictionary<int, Action<IHitCallbackObject>> hitCallbackMap) framesInfo);
            IAttackCallbackGroupObj CleanUp(Action callback);
        }
    }

    namespace CharacterProperties {
        public partial class AttackBuilderGroup : IAttackCallbackGroupObj {
            //public InputNotation input { get; private set; }
            public Func<float, float> moveX { get; private set; }
            public Func<float, float> moveY { get; private set; }
            public Func<float, float> moveZ { get; private set; }
            public Action<float, Vector3> framesContinuous { get; private set; }
            public Action cleanUpCallback { get; private set; }
            public string animStateName { get; private set; }
            public CharacterStates.Attack initAttackState { get; private set; }

            private Func<float, float> return0Callback = frame => 0.0f;

            //public IAttackCallbackGroupObj Input(params string[] inputStrings) {
            //    this.input = new InputNotation(new List<string>(inputStrings));
            //    return this;
            //}
            public IAttackCallbackGroupObj Movement(Func<float, float> xMoveCb = null, Func<float, float> yMoveCb = null, Func<float, float> zMoveCb = null) {
                if (xMoveCb != null) moveX = xMoveCb;
                else moveX = return0Callback;

                if (yMoveCb != null) moveY = yMoveCb;
                else moveY = return0Callback;

                if (zMoveCb != null) moveZ = zMoveCb;
                else moveZ = return0Callback;

                valuesSet["move"] = true;
                return this;
            }
            public IAttackCallbackGroupObj FramesContinuous(Action<float, Vector3> callback) {
                this.framesContinuous = callback;
                valuesSet["framesContinuous"] = true;
                return this;
            }
            public IAttackCallbackGroupObj AnimationState(string animStateName) {
                this.animStateName = animStateName;
                valuesSet["animStateName"] = true;
                return this;
            }
            public IAttackCallbackGroupObj InitCharState(CharacterStates.Attack initAttackState) {
                this.initAttackState = initAttackState;
                valuesSet["initAttackState"] = true;
                return this;
            }
            public IAttackCallbackGroupObj Frames((List<FrameStateBuilder> frameList, Dictionary<int, Action<IHitCallbackObject>> hitCallbackMap) framesInfo) {
                this.frames.AddRange(framesInfo.frameList);
                this.hitCallbackMap = framesInfo.hitCallbackMap;
                valuesSet["frames"] = true;
                return this;
            }
            public IAttackCallbackGroupObj CleanUp(Action callback) {
                this.cleanUpCallback = callback;
                valuesSet["cleanUpCallback"] = true;
                return this;
            }
        }
    }
}
