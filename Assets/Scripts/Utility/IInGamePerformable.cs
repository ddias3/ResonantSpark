using UnityEngine;

using ResonantSpark.Service;

namespace ResonantSpark {
    namespace Gameplay {
        public interface IInGamePerformable {
            void StartPerformable(int frameIndex);
            void RunFrame(IHitBoxService hitBoxServ, IProjectileService projectServ, IAudioService audioServ);

                // This expects the Performable to track which frame it's in.
            bool IsCompleteRun();

                // For testing purposes right now, I'm going to double check that the internal frameCount and the (frameIndex - frameStart) match
            void FrameCountSanityCheck(int frameIndex);
        }
    }
}