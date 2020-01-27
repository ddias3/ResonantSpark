using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using ResonantSpark.Service;
using ResonantSpark.Gamemode;
using ResonantSpark.Input;
using ResonantSpark.Builder;
using ResonantSpark.Gameplay;

namespace ResonantSpark {
    public class EntryPoint : MonoBehaviour {

        public GameObject char0Builder;
        public GameObject char1Builder;

        public FightingGameService fgService;
        public PlayerService playerService;
        public InputService inputService;

        private List<Type> remainingServices = new List<Type> {
            typeof(AudioService),
            typeof(BuildService),
            typeof(FightingGameService),
            typeof(HitBoxService),
            typeof(InputService),
            typeof(PersistenceService),
            typeof(PlayerService),
            typeof(ProjectileService),
            typeof(UIService),
        };

        public void Start() {
            EventManager.StartListening<Events.ServiceReady, Type>(new UnityAction<Type>(ServiceReady));
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