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
            private HitService hitService;
            private InputService inputService;
            private ParticleService particleService;
            private PersistenceService persistenceService;
            private PhysicsService physicsService;
            private PlayerService playerService;
            private ProjectileService projectileService;
            private CameraService cameraService;
            private TimeService timeService;
            private UiService uiService;

            private AllServices services;

            private FightingGameCharacter currFGChar;

            public void Start() {
                fgService = GetComponent<FightingGameService>();
                audioService = GetComponent<AudioService>();
                cameraService = GetComponent<CameraService>();
                hitBoxService = GetComponent<HitBoxService>();
                hitService = GetComponent<HitService>();
                inputService = GetComponent<InputService>();
                particleService = GetComponent<ParticleService>();
                persistenceService = GetComponent<PersistenceService>();
                physicsService = GetComponent<PhysicsService>();
                playerService = GetComponent<PlayerService>();
                projectileService = GetComponent<ProjectileService>();
                timeService = GetComponent<TimeService>();
                uiService = GetComponent<UiService>();

                services = new AllServices()
                    .AddServiceAs<IBuildService>(this)
                    .AddServiceAs<ICameraService>(cameraService)
                    .AddServiceAs<IFightingGameService>(fgService)
                    .AddServiceAs<IAudioService>(audioService)
                    .AddServiceAs<IHitBoxService>(hitBoxService)
                    .AddServiceAs<IHitService>(hitService)
                    .AddServiceAs<IInputService>(inputService)
                    .AddServiceAs<IParticleService>(particleService)
                    .AddServiceAs<IPersistenceService>(persistenceService)
                    .AddServiceAs<IPlayerService>(playerService)
                    .AddServiceAs<IProjectileService>(projectileService)
                    .AddServiceAs<IPhysicsService>(physicsService)
                    .AddServiceAs<ITimeService>(timeService)
                    .AddServiceAs<IUiService>(uiService);

                EventManager.TriggerEvent<Events.ServiceReady, Type>(typeof(BuildService));
            }

            public AllServices GetAllServices() {
                return services;
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