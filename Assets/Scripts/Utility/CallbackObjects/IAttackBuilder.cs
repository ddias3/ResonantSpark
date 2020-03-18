using UnityEngine;
using System;
using System.Collections.Generic;

using ResonantSpark.Builder;
using ResonantSpark.Character;
using ResonantSpark.CharacterProperties;
using ResonantSpark.Input;

namespace ResonantSpark {
    namespace Builder {
        public interface IAttackCallbackObj {
            IAttackCallbackObj Name(string name);
            IAttackCallbackObj Orientation(Orientation orientation);
            IAttackCallbackObj GroundRelation(GroundRelation groundRelation);
            IAttackCallbackObj Input(params InputNotation[] input);
            IAttackCallbackObj Movement(Func<float, float> xMoveCb = null, Func<float, float> yMoveCb = null, Func<float, float> zMoveCb = null);
            IAttackCallbackObj AnimationState(string animStateName);
            IAttackCallbackObj FramesContinuous(Action<float, Vector3> callback);
            IAttackCallbackObj Frames((List<FrameStateBuilder> frameList, Dictionary<int, Action<IHitCallbackObject>> hitCallbackMap) framesInfo);
            IAttackCallbackObj CleanUp(Action callback);
        }
    }

    namespace CharacterProperties {
        public partial class AttackBuilder : IAttackCallbackObj {
            public string name { get; private set; }
            public Orientation orientation { get; private set; }
            public GroundRelation groundRelation { get; private set; }
            public List<InputNotation> input { get; private set; }
            public Func<float, float> moveX { get; private set; }
            public Func<float, float> moveY { get; private set; }
            public Func<float, float> moveZ { get; private set; }
            public Action<float, Vector3> framesCountinuous { get; private set; }
            public Action cleanUpCallback { get; private set; }
            public string animStateName { get; private set; }

            private Func<float, float> return0Callback = frame => 0.0f;

            public IAttackCallbackObj Name(string name) {
                this.name = name;
                return this;
            }
            public IAttackCallbackObj Orientation(Orientation orientation) {
                this.orientation = orientation;
                return this;
            }
            public IAttackCallbackObj GroundRelation(GroundRelation groundRelation) {
                this.groundRelation = groundRelation;
                return this;
            }
            public IAttackCallbackObj Input(params InputNotation[] input) {
                this.input = new List<InputNotation>(input);
                return this;
            }
            public IAttackCallbackObj Movement(Func<float, float> xMoveCb = null, Func<float, float> yMoveCb = null, Func<float, float> zMoveCb = null) {
                if (xMoveCb != null) moveX = xMoveCb;
                else moveX = return0Callback;

                if (yMoveCb != null) moveY = yMoveCb;
                else moveY = return0Callback;

                if (zMoveCb != null) moveZ = zMoveCb;
                else moveZ = return0Callback;

                return this;
            }
            public IAttackCallbackObj FramesContinuous(Action<float, Vector3> callback) {
                this.framesCountinuous = callback;
                return this;
            }
            public IAttackCallbackObj AnimationState(string animStateName) {
                this.animStateName = animStateName;
                return this;
            }
            public IAttackCallbackObj Frames((List<FrameStateBuilder> frameList, Dictionary<int, Action<IHitCallbackObject>> hitCallbackMap) framesInfo) {
                this.frames.AddRange(framesInfo.frameList);
                this.hitCallbackMap = framesInfo.hitCallbackMap;
                return this;
            }
            public IAttackCallbackObj CleanUp(Action callback) {
                this.cleanUpCallback = callback;
                return this;
            }
        }
    }
}