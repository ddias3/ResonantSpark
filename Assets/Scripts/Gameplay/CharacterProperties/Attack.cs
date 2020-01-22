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
            public int priority { get; private set; }
            public string animStateName { get; private set; }
            public List<FrameState> frames { get; private set; }
            public List<HitBox> hitBoxes { get; private set; }

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

                fgChar = services.GetService<IBuildService>().GetBuildingFGChar();

                AttackBuilder attackBuilder = new AttackBuilder(services);
                builderCallback(attackBuilder);

                attackBuilder.BuildAttack();

                name = attackBuilder.name;
                orientation = attackBuilder.orientation;
                groundRelation = attackBuilder.groundRelation;
                input = attackBuilder.input;
                priority = 1;
                animStateName = attackBuilder.animStateName;

                frames = attackBuilder.GetFrames();
                hitBoxes = attackBuilder.GetHitBoxes();

                tracker = new AttackTracker(frames.Count);
            }

            public void FrameCountSanityCheck(int frameIndex) {
                throw new NotImplementedException();
            }

            public bool IsCompleteRun() {
                throw new NotImplementedException();
            }

            public void SetOnCompleteCallback(Action onCompleteCallback) {
                this.onCompleteCallback = onCompleteCallback;
            }

            public void StartPerformable(int frameIndex) {
                tracker.Track(frameIndex);
                fgChar.Play(animStateName);
            }

            public void RunFrame() {
                int frameCount = tracker.frameCount;
                frames[frameCount].Perform();

                if (frameCount == frames.Count - 1) {
                    onCompleteCallback();
                }

                tracker.Increment();
            }

            public bool ChainCancellable() {
                return frames[tracker.frameCount].chainCancellable;
            }

            public bool SpecialCancellable() {
                return frames[tracker.frameCount].specialCancellable;
            }

            public bool Equals(Attack other) {
                return id == other.id;
            }

            public override int GetHashCode() {
                return id;
            }
        }
    }
}