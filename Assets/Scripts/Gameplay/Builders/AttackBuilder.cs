using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Builder;
using ResonantSpark.Service;

namespace ResonantSpark {
    namespace CharacterProperties {
        public partial class AttackBuilder : IAttackCallbackObj {
            private Dictionary<string, Action<IAttackCallbackGroupObj>> groupCallbacks;
            private Dictionary<string, AttackBuilderGroup> groupAttackBuilders;
            private AttackBuilderGroup defaultGroup;

            private AllServices services;

            public AttackBuilder(AllServices services) {
                this.services = services;

                startGroup = "default";

                groupCallbacks = new Dictionary<string, Action<IAttackCallbackGroupObj>>();
                groupAttackBuilders = new Dictionary<string, AttackBuilderGroup>();

                defaultGroup = new AttackBuilderGroup(services);
            }

            public void BuildAttack() {
                // There is no default group callback; that is part of the Attack Builder itself.
                defaultGroup.BuildAttack();
                if (!defaultGroup.ValueSet("frames")) {
                    defaultGroup.SetFrames(new List<Character.FrameState>());
                    defaultGroup.SetHits(new List<Gameplay.Hit>());
                }

                foreach (KeyValuePair<string, Action<IAttackCallbackGroupObj>> kvp in groupCallbacks) {
                    AttackBuilderGroup groupBuilder = new AttackBuilderGroup(services);
                    Action<IAttackCallbackGroupObj> groupCallback = kvp.Value;
                    groupCallback(groupBuilder);
                    groupBuilder.BuildAttack();

                    if (!groupBuilder.ValueSet("move")) groupBuilder.Movement(defaultGroup.moveX, defaultGroup.moveY, defaultGroup.moveZ);
                    if (!groupBuilder.ValueSet("framesContinuous")) groupBuilder.FramesContinuous(defaultGroup.framesContinuous);
                    if (!groupBuilder.ValueSet("cleanUpCallback")) groupBuilder.CleanUp(defaultGroup.cleanUpCallback);
                    if (!groupBuilder.ValueSet("animStateName")) groupBuilder.AnimationState(defaultGroup.animStateName);
                    if (!groupBuilder.ValueSet("initAttackState")) groupBuilder.InitCharState(defaultGroup.initAttackState);
                    if (!groupBuilder.ValueSet("frames")) {
                        groupBuilder.SetFrames(defaultGroup.GetFrames());
                        groupBuilder.SetHits(defaultGroup.GetHits());
                    }

                    groupAttackBuilders.Add(kvp.Key, groupBuilder);
                }
                groupAttackBuilders.Add("default", defaultGroup);
            }

            public Dictionary<string, AttackBuilderGroup> GetAttackBuilderGroups() {
                return groupAttackBuilders;
            }

            public AttackBuilderGroup GetDefaultGroup() {
                return defaultGroup;
            }
        }
    }
}