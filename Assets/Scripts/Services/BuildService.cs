using System;
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
            private ParticleService particleService;
            private CameraService cameraService;
            private TimeService timeService;

            private AllServices services;

            private FightingGameCharacter currFGChar;

            public void Start() {
                fgService = GetComponent<FightingGameService>();
                audioService = GetComponent<AudioService>();
                hitBoxService = GetComponent<HitBoxService>();
                projectileService = GetComponent<ProjectileService>();
                particleService = GetComponent<ParticleService>();
                cameraService = GetComponent<CameraService>();
                timeService = GetComponent<TimeService>();

                services = new AllServices()
                    .AddServiceAs<IBuildService>(this)
                    .AddServiceAs<IFightingGameService>(fgService)
                    .AddServiceAs<IAudioService>(audioService)
                    .AddServiceAs<IHitBoxService>(hitBoxService)
                    .AddServiceAs<IProjectileService>(projectileService)
                    .AddServiceAs<IParticleService>(particleService)
                    .AddServiceAs<ICameraService>(cameraService)
                    .AddServiceAs<ITimeService>(timeService);

                EventManager.TriggerEvent<Events.ServiceReady, Type>(typeof(BuildService));
            }

            public FightingGameCharacter Build(ICharacterBuilder charBuilder) {
                currFGChar = charBuilder.CreateCharacter(services);
                charBuilder.Build(services);
                return currFGChar;
            }

            public FightingGameCharacter GetBuildingFGChar() {
                return currFGChar;
            }
        }
    }
}