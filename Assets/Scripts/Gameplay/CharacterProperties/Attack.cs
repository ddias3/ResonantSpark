using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Character;
using ResonantSpark.Gameplay;
using ResonantSpark.Service;
using ResonantSpark.Utility;

namespace ResonantSpark {
    namespace CharacterProperties {
        public class Attack : ScriptableObject, IInGamePerformable {
            private new string name;
            private List<FrameState> frames;

            private AttackTracker tracker;

            public Attack(Action<Builder.IAttackCallbackObj> builderCallback) {
                AttackBuilder atkBuilder = new AttackBuilder();
                builderCallback(atkBuilder);

                name = atkBuilder.name;
                frames = atkBuilder.GetFrames();

                tracker = new AttackTracker(frames.Count);
            }

            public void FrameCountSanityCheck(int frameIndex) {
                throw new NotImplementedException();
            }

            public bool IsCompleteRun() {
                throw new NotImplementedException();
            }

            public void StartPerformable(int frameIndex) {
                tracker.Track(frameIndex);
            }

            public void RunFrame(IHitBoxService hitBoxServ, IProjectileService projectServ, IAudioService audioServ) {
                int frameCount = tracker.GetFrameCount();
                // TODO: Perform the actions of the frame.
                frames[frameCount].Perform();
            }
        }
    }
}