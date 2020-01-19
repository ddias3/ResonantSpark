using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Input;

namespace ResonantSpark {
    namespace Service {
        public class InputService : MonoBehaviour, IInputService {

            public GameObject inputs;
            public HumanInputController prefab;

            private PersistenceService persistenceService;
            private PlayerService playerService;

            private int numHumanPlayers = 0;
            private Dictionary<int, HumanInputController> controllers;

            public void Start() {
                persistenceService = GetComponent<PersistenceService>();
                playerService = GetComponent<PlayerService>();

                controllers = new Dictionary<int, HumanInputController>();

                EventManager.TriggerEvent<Events.ServiceReady, Type>(typeof(InputService));
            }

            public void SetUpControllers() {
                for (int n = 0; n < playerService.GetMaxPlayers(); ++n) {
                    if (persistenceService.GetControllerIndex(n) >= 0) {
                        HumanInputController humanInput = GameObject.Instantiate<HumanInputController>(prefab, inputs.transform);
                        humanInput.SetControllerId(n);

                        controllers.Add(n, humanInput);
                    }
                }
            }

            public HumanInputController GetInputController(int controllerIndex) {
                if (controllers.TryGetValue(controllerIndex, out HumanInputController inputController)) {
                    return inputController;
                }
                return null;
            }
        }
    }
}