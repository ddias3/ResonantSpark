using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using ResonantSpark.MenuStates;

namespace ResonantSpark {
    namespace Menu {
        public class MenuInputManager : MonoBehaviour {
            private MenuBaseState activeState;

            public void Update() {
                if (Keyboard.current.wKey.wasPressedThisFrame) {
                    activeState.TriggerEvent("up");
                }
                if (Keyboard.current.sKey.wasPressedThisFrame) {
                    activeState.TriggerEvent("down");
                }
                if (Keyboard.current.aKey.wasPressedThisFrame) {
                    activeState.TriggerEvent("left");
                }
                if (Keyboard.current.dKey.wasPressedThisFrame) {
                    activeState.TriggerEvent("right");
                }

                if (Keyboard.current.jKey.wasPressedThisFrame) {
                    activeState.TriggerEvent("submit");
                }
                if (Keyboard.current.kKey.wasPressedThisFrame) {
                    activeState.TriggerEvent("cancel");
                }

                if (Keyboard.current.escapeKey.wasPressedThisFrame) {
                    activeState.TriggerEvent("pause");
                }

                if (Keyboard.current.uKey.wasPressedThisFrame) {

                }
                if (Keyboard.current.iKey.wasPressedThisFrame) {
                    // do nothing
                }
            }

            public void SetActiveState(MenuBaseState state) {
                activeState = state;
            }
        }
    }
}
