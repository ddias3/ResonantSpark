using UnityEngine;

using ResonantSpark.Gameplay;
using ResonantSpark.Builder;

namespace ResonantSpark {
    namespace Service {
        public class BuildService : MonoBehaviour, IBuildService {

            private FightingGameService fgService;
            private AudioService audioService;
            private HitBoxService hitBoxService;
            private ProjectileService projectileService;

            private AllServices services;

            public void Start() {
                fgService = GetComponent<FightingGameService>();
                audioService = GetComponent<AudioService>();
                hitBoxService = GetComponent<HitBoxService>();
                projectileService = GetComponent<ProjectileService>();

                new AllServices()
                    .AddServiceAs<IBuildService>(this)
                    .AddServiceAs<IFightingGameService>(fgService)
                    .AddServiceAs<IAudioService>(audioService)
                    .AddServiceAs<IHitBoxService>(hitBoxService)
                    .AddServiceAs<IProjectileService>(projectileService);
            }

            public FightingGameCharacter Build(ICharacterBuilder charBuilder) {
                GameObject char0 = charBuilder.CreateCharacter(services);
                return char0.GetComponent<FightingGameCharacter>();
            }

            public FightingGameCharacter GetBuildingFGChar() {
                throw new System.NotImplementedException();
            }
        }
    }
}