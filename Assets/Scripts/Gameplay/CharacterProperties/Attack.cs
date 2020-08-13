using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Character;
using ResonantSpark.Gameplay;
using ResonantSpark.Service;
using ResonantSpark.Utility;
using ResonantSpark.Input;

namespace ResonantSpark {
    namespace CharacterProperties {
        public class Attack : IInGamePerformable, IEquatable<Attack> {
            private static int attackCounter = 0;

            private Action<Builder.IAttackCallbackObj> builderCallback;

            public int id { get; private set; }
            public string name { get; private set; }
            public Orientation orientation { get; private set; }
            public GroundRelation groundRelation { get; private set; }
            public InputNotation input { get; private set; }
            public string startGroup { get; private set; }

            public Dictionary<string, AttackInfoGroup> groups { get; private set; }
            
            private AttackInfoGroup currGroup;

            private Dictionary<string, InGameEntity> savedEntities;

            private IFightingGameService fgService;
            private IProjectileService projectServ;
            private IAudioService audioServ;

            private FightingGameCharacter fgChar;
            private Action onCompleteCallback;

            private AttackTracker tracker;

            public Attack(Action<Builder.IAttackCallbackObj> builderCallback) {
                this.builderCallback = builderCallback;

                this.id = Attack.attackCounter++;
            }

            public void BuildAttack(AllServices services) {
                fgService = services.GetService<IFightingGameService>();
                projectServ = services.GetService<IProjectileService>();
                audioServ = services.GetService<IAudioService>();

                    // TODO: Change this in the future to InGameEntity and allow attacks to be owned by InGameEntities
                fgChar = services.GetService<IBuildService>().GetBuildingFGChar();

                groups = new Dictionary<string, AttackInfoGroup>();
                savedEntities = new Dictionary<string, InGameEntity>();

                AttackBuilder attackBuilder = new AttackBuilder(services);
                builderCallback(attackBuilder);

                attackBuilder.BuildAttack();

                name = attackBuilder.name;
                orientation = attackBuilder.orientation;
                groundRelation = attackBuilder.groundRelation;
                input = attackBuilder.input;
                startGroup = attackBuilder.startGroup;
                foreach (KeyValuePair<string, AttackBuilderGroup> kvp in attackBuilder.GetAttackBuilderGroups()) {
                    AttackBuilderGroup builderGroup = kvp.Value;
                    AttackInfoGroup group = new AttackInfoGroup {
                        animStateName = builderGroup.animStateName,
                        initAttackState = builderGroup.initAttackState,
                        hits = builderGroup.GetHits(),
                        frames = builderGroup.GetFrames(),

                        xMoveCb = builderGroup.moveX,
                        yMoveCb = builderGroup.moveY,
                        zMoveCb = builderGroup.moveZ,

                        framesContinuous = builderGroup.framesContinuous,
                        cleanUpCallback = builderGroup.cleanUpCallback,
                    };
                    groups.Add(kvp.Key, group);
                }

                currGroup = groups[startGroup];
                tracker = new AttackTracker();
            }

            public void UseGroup(string groupId) {
                currGroup = groups[groupId];
            }

            public void ResetTracker() {
                tracker.Track();
            }

            public void SetAnimation() {
                fgChar.Play(currGroup.animStateName);
            }

            public void SaveEntity(string id, InGameEntity saveEntity) {
                savedEntities[id] = saveEntity;
            }

            public InGameEntity GetEntity(string id) {
                return savedEntities[id];
            }

            public void FrameCountSanityCheck(int frameIndex) {
                throw new NotImplementedException();
            }

            public bool IsCompleteRun() {
                return tracker.frameCount == currGroup.frames.Count - 1;
            }

            public void SetOnCompleteCallback(Action onCompleteCallback) {
                this.onCompleteCallback = onCompleteCallback;
            }

            public void StartPerformable(int frameIndex) {
                foreach (KeyValuePair<string, AttackInfoGroup> kvp in groups) {
                    AttackInfoGroup group = kvp.Value;
                    foreach (Hit hit in group.hits) {
                        hit.ClearHitEntities();
                    }
                }
                currGroup = groups[startGroup];
                tracker.Track(frameIndex);
                Debug.Log("Start Performable Attack Animation State: " + currGroup.animStateName + " from attack: " + ToString());
                fgChar.Play(currGroup.animStateName);
            }

            public void RunFrame() {
                //Debug.LogWarningFormat("    -- Atk: {0} | Frame#: = {1}/{2}", ToString(), frameCount, frames.Count);
                currGroup.framesContinuous?.Invoke((float)tracker.frameCount, fgChar.GetTarget().targetPos);
                currGroup.frames[tracker.frameCount].Perform(fgChar);

                fgChar.AddRelativeVelocity(VelocityPriority.AttackMovement,
                    new Vector3(
                        FunctionCalculus.Differentiate(currGroup.xMoveCb, tracker.frameCount) / fgChar.gameTime,
                        FunctionCalculus.Differentiate(currGroup.yMoveCb, tracker.frameCount) / fgChar.gameTime,
                        FunctionCalculus.Differentiate(currGroup.zMoveCb, tracker.frameCount) / fgChar.gameTime));

                if (IsCompleteRun()) {
                    currGroup.cleanUpCallback?.Invoke();
                    onCompleteCallback();
                    Reset();
                }

                tracker.Increment();
            }

            public void Reset() {
                currGroup = groups[startGroup];
                savedEntities.Clear();
            }

            public CharacterStates.Attack GetInitAttackState() {
                return currGroup.initAttackState;
            }

            public List<Hit> GetNextHit() {
                int n = tracker.frameCount;
                while (n < currGroup.frames.Count) {
                    if (currGroup.frames[n].hits.Count > 0) {
                        return currGroup.frames[n].hits;
                    }
                    else {
                        ++n;
                    }
                }
                return null;
            }

            public bool CounterHit() {
                return currGroup.frames[tracker.frameCount].counterHit;
            }

            public bool CancellableOnWhiff() {
                return currGroup.frames[tracker.frameCount].cancellableOnWhiff;
            }

            public bool ChainCancellable() {
                return currGroup.frames[tracker.frameCount].chainCancellable;
            }

            public bool SpecialCancellable() {
                return currGroup.frames[tracker.frameCount].specialCancellable;
            }

            public CharacterVulnerability GetCharacterVulnerability() {
                // TODO: return frames[tracker.frameCount].characterVulnerability;
                return new CharacterVulnerability {
                    strikable = true,
                    throwable = true,
                };
            }

            public void PredeterminedActions(string actionName, params object[] objs) {
            }

            public bool Equals(Attack other) {
                return id == other.id;
            }

            public override int GetHashCode() {
                return id;
            }

            public override string ToString() {
                return string.Format("ATK: {0}.{1}", orientation, name);
            }
        }

        public struct AttackInfoGroup {
            public int priority;
            public string animStateName;
            public CharacterStates.Attack initAttackState;
            public List<Hit> hits;
            public List<FrameState> frames;

            public Func<float, float> xMoveCb;
            public Func<float, float> yMoveCb;
            public Func<float, float> zMoveCb;

            public Action<float, Vector3> framesContinuous;
            public Action cleanUpCallback;
        }
    }
}