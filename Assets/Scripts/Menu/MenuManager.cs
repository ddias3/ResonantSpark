using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Menu {
        public class MenuManager : MonoBehaviour {

            private List<Menu> menus;

            public void AddMenu(Menu menu) {
                if (menus == null) menus = new List<Menu>();
                menus.Add(menu);
            }

            public void InitMenus() {
                foreach (Menu menu in menus) {
                    menu.TriggerEvent("init");
                }
            }
        }
    }
}