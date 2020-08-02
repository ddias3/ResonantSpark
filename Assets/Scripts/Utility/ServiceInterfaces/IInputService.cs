using UnityEngine;

using ResonantSpark.Character;
using ResonantSpark.Input;

namespace ResonantSpark {
    namespace Service {
        public interface IInputService {
            InputController GetInputController(int controllerIndex);
        }
    }
}
