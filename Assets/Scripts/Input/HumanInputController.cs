using System.Collections;
using System.Collections.Generic;
using ResonantSpark.Gameplay;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ResonantSpark {
    namespace Input {
        [RequireComponent(typeof(InputBuffer), typeof(PlayerInput))]
        public class HumanInputController : MonoBehaviour, ICharacterControlSystem {
            private InputBuffer inputBuffer;
            private PlayerInput playerInput;

            private int controllerId = -1;

            public void Start() {
                inputBuffer = GetComponent<InputBuffer>();
                playerInput = GetComponent<PlayerInput>();
            }

            public void SetControllerId(int controllerId) {
                this.controllerId = controllerId;
            }

            public void ConnectToCharacter(FightingGameCharacter fgChar) {
                fgChar.SetInputBuffer(inputBuffer);
            }
        }
    }
}