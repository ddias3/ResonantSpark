using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ResonantSpark {
    namespace Menu {
        public class MenuInputManager : MonoBehaviour {
            public Menu activeMenu;

            public void Update() {
                if (Keyboard.current.wKey.wasPressedThisFrame) {
                    activeMenu.TriggerEvent("up");
                }
                if (Keyboard.current.sKey.wasPressedThisFrame) {
                    activeMenu.TriggerEvent("down");
                }
                if (Keyboard.current.aKey.wasPressedThisFrame) {
                    activeMenu.TriggerEvent("left");
                }
                if (Keyboard.current.dKey.wasPressedThisFrame) {
                    activeMenu.TriggerEvent("right");
                }

                if (Keyboard.current.jKey.wasPressedThisFrame) {
                    activeMenu.TriggerEvent("submit");
                }
                if (Keyboard.current.kKey.wasPressedThisFrame) {
                    activeMenu.TriggerEvent("cancel");
                }

                if (Keyboard.current.uKey.wasPressedThisFrame) {

                }
                if (Keyboard.current.iKey.wasPressedThisFrame) {
                    // do nothing
                }
            }
        }
    }
}