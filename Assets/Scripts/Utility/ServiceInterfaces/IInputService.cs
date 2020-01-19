using UnityEngine;

using ResonantSpark.Character;
using ResonantSpark.Input;

namespace ResonantSpark {
    namespace Service {
        public interface IInputService {
            void SetUpControllers();
            HumanInputController GetInputController(int controllerIndex);
        }
    }
}
