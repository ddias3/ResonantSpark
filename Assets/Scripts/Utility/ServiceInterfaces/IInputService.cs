using UnityEngine;

using ResonantSpark.Character;
using ResonantSpark.Input;

namespace ResonantSpark {
    namespace Service {
        public interface IInputService {
            int CreateController(InputControllerType type);
            InputController GetInputController(int controllerIndex);
        }
    }
}
