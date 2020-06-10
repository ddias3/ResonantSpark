using System;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Menu {
        public class MenuStack : MonoBehaviour {
            List<Menu> menuStack;

            public void Awake() {
                menuStack = new List<Menu>();
            }

            public void AddMenu(Menu menu) {
                if (!menuStack.Contains(menu)) {
                    menuStack.Add(menu);
                    menu.TriggerEvent("activate");
                }
            }

            public Menu Pop() {
                Menu menu = Peek();
                menuStack.RemoveAt(menuStack.Count - 1);
                menu.TriggerEvent("deactivate");
                return menu;
            }

            public Menu Pop(Menu menu) {
                int index = menuStack.LastIndexOf(menu);
                if (index > -1) {
                    menuStack.RemoveAt(index);
                }
                menu.TriggerEvent("deactivate");
                return menu;
            }

            public Menu Peek() {
                return menuStack[menuStack.Count - 1];
            }
        }
    }
}