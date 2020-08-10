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
            IAttackCallbackObj Group(string groupId, Action<IAttackCallbackGroupObj> attackCallback);
            IAttackCallbackObj Name(string name);
            IAttackCallbackObj Requires(Orientation orientation, GroundRelation groundRelation);
            IAttackCallbackObj Input(params string[] inputStrings);
            IAttackCallbackObj StartGroup(string startGroup);
            IAttackCallbackObj Movement(Func<float, float> xMoveCb = null, Func<float, float> yMoveCb = null, Func<float, float> zMoveCb = null);
            IAttackCallbackObj AnimationState(string animStateName);
            IAttackCallbackObj InitCharState(CharacterStates.Attack attackState);
            IAttackCallbackObj FramesContinuous(Action<float, Vector3> callback);
            IAttackCallbackObj Frames((List<FrameStateBuilder> frameList, Dictionary<int, Action<IHitCallbackObject>> hitCallbackMap) framesInfo);
            IAttackCallbackObj CleanUp(Action callback);
        }
    }

    namespace CharacterProperties {
        public partial class AttackBuilder : IAttackCallbackObj {

            public string name { private set; get; }
            public Orientation orientation { get; private set; }
            public GroundRelation groundRelation { get; private set; }
            public InputNotation input { get; private set; }
            public string startGroup { private set; get; }

            public IAttackCallbackObj Group(string groupId, Action<IAttackCallbackGroupObj> attackCallback) {
                this.groupCallbacks.Add(groupId, attackCallback);
                return this;
            }
            public IAttackCallbackObj Name(string name) {
                this.name = name;
                return this;
            }
            public IAttackCallbackObj Input(params string[] inputStrings) {
                //this.defaultGroup.Input(inputStrings);
                this.input = new InputNotation(new List<string>(inputStrings));
                return this;
            }
            public IAttackCallbackObj StartGroup(string startGroup) {
                this.startGroup = startGroup;
                return this;
            }
            public IAttackCallbackObj Requires(Orientation orientation, GroundRelation groundRelation) {
                this.orientation = orientation;
                this.groundRelation = groundRelation;
                return this;
            }
            public IAttackCallbackObj Movement(Func<float, float> xMoveCb = null, Func<float, float> yMoveCb = null, Func<float, float> zMoveCb = null) {
                this.defaultGroup.Movement(xMoveCb, yMoveCb, zMoveCb);
                return this;
            }
            public IAttackCallbackObj FramesContinuous(Action<float, Vector3> callback) {
                this.defaultGroup.FramesContinuous(callback);
                return this;
            }
            public IAttackCallbackObj AnimationState(string animStateName) {
                this.defaultGroup.AnimationState(animStateName);
                return this;
            }
            public IAttackCallbackObj InitCharState(CharacterStates.Attack initAttackState) {
                this.defaultGroup.InitCharState(initAttackState);
                return this;
            }
            public IAttackCallbackObj Frames((List<FrameStateBuilder> frameList, Dictionary<int, Action<IHitCallbackObject>> hitCallbackMap) framesInfo) {
                this.defaultGroup.Frames(framesInfo);
                return this;
            }
            public IAttackCallbackObj CleanUp(Action callback) {
                this.defaultGroup.CleanUp(callback);
                return this;
            }
        }
    }
}