using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Gameplay;

namespace ResonantSpark {
    namespace Input {
        public enum InputControllerType : int {
            None = 0,
            Human,
            Dummy,
            AI
        }

        [RequireComponent(typeof(InputBuffer))]
        public abstract class InputController : MonoBehaviour, ICharacterControlSystem {
            protected InputBuffer inputBuffer;

            public void Awake() {
                inputBuffer = GetComponent<InputBuffer>();
            }

            public void ConnectToCharacter(FightingGameCharacter fgChar) {
                fgChar.SetInputBuffer(inputBuffer);
            }
        }
    }
}