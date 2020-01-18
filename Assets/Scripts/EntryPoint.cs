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
        };

        public void Start() {
            EventManager.StartListening<Events.ServiceReady, Type>(new UnityAction<Type>(ServiceReady));
        }

        private void ServiceReady(Type serviceType) {
            remainingServices.Remove(serviceType);

            if (remainingServices.Count == 0) {
                StartSceneSetup();
            }
        }

        private void StartSceneSetup() {
            fgService.CreateGamemode();
            playerService.SetUpCharacters();

            playerService.SetCharacterSelected(0, char0Builder.GetComponent<ICharacterBuilder>());
            playerService.SetCharacterSelected(1, char1Builder.GetComponent<ICharacterBuilder>());

            playerService.SetNumberHumanPlayers(1);
            playerService.AssociateHumanInput(0, inputService.GetInputController(0));

            List<FightingGameCharacter> fgChars = new List<FightingGameCharacter>();

            playerService.StartCharacterBuild(fgChar => {
                fgChars.Add(fgChar);
            });

            GameObject.FindGameObjectWithTag("rspTime").GetComponent<FrameEnforcer>().StartFrameEnforcer();
        }
    }
}