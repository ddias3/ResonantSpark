using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Input;

namespace ResonantSpark {
    namespace Service {
        public class InputService : MonoBehaviour, IInputService {

            public GameObject inputs;
            public HumanInputController humanInputControllerPrefab;
            public DummyInputController dummyInputControllerPrefab;
            public AiInputController aiInputControllerPrefab;
            public NoneInputController noneInputControllerPrefab;

            private PersistenceService persistenceService;
            private FightingGameService fgService;

            private List<InputController> controllers;

            public void Start() {
                persistenceService = GetComponent<PersistenceService>();
                fgService = GetComponent<FightingGameService>();

                controllers = new List<InputController>();

                EventManager.TriggerEvent<Events.ServiceReady, Type>(typeof(InputService));
            }

            public int CreateController(InputControllerType type) {
                InputController controller = null;
                switch (type) {
                    case InputControllerType.Human:
                        controller = GameObject.Instantiate<HumanInputController>(humanInputControllerPrefab, inputs.transform);
                        break;
                    case InputControllerType.Dummy:
                        controller = GameObject.Instantiate<DummyInputController>(dummyInputControllerPrefab, inputs.transform);
                        break;
                    case InputControllerType.AI:
                        controller = GameObject.Instantiate<AiInputController>(aiInputControllerPrefab, inputs.transform);
                        break;
                    case InputControllerType.None:
                        controller = GameObject.Instantiate<NoneInputController>(noneInputControllerPrefab, inputs.transform);
                        break;
                }
                int index = controllers.Count;
                if (controller != null) {
                    controllers.Add(controller);
                }
                return index;
            }

            public InputController GetInputController(int controllerIndex) {
                return controllers[controllerIndex];
            }
        }
    }
}