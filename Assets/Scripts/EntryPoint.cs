using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using ResonantSpark.Service;

namespace ResonantSpark {
    public class EntryPoint : MonoBehaviour {

        public FightingGameService fgService;
        public PlayerService playerService;
        public InputService inputService;

        private UnityAction<Type> serviceReadyCallback;

        private List<Type> remainingServices = new List<Type> {
            typeof(AudioService),
            typeof(BuildService),
            typeof(CameraService),
            typeof(FightingGameService),
            typeof(HitBoxService),
            typeof(InputService),
            typeof(ParticleService),
            typeof(PersistenceService),
            typeof(PlayerService),
            typeof(ProjectileService),
            typeof(TimeService),
            typeof(UiService),
        };

        public void Start() {
            EventManager.StartListening<Events.ServiceReady, Type>(serviceReadyCallback = new UnityAction<Type>(ServiceReady));
        }

        public void OnDestroy() {
            EventManager.StopListening<Events.ServiceReady, Type>(serviceReadyCallback);
        }

        private void ServiceReady(Type serviceType) {
            remainingServices.Remove(serviceType);

            if (remainingServices.Count == 0) {
                StartCoroutine(StartSceneSetup());
            }
        }

        private IEnumerator StartSceneSetup() {
            yield return new WaitForEndOfFrame();

            fgService.CreateGamemode();
            inputService.SetUpControllers();
            playerService.SetUpCharacters();
            playerService.StartCharacterBuild();
            fgService.SetUpGamemode();

            EventManager.TriggerEvent<Events.StartGame>();

            GameObject.FindGameObjectWithTag("rspTime").GetComponent<FrameEnforcer>().StartFrameEnforcer();
        }
    }
}