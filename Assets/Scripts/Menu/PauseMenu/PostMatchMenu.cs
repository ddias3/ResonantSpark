using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Menu {
        public class PostMatchMenu : Menu {
            public PostMatchSubmenu p1Menu;
            public PostMatchSubmenu p2Menu;

            public void Start() {
                GameObject.FindGameObjectWithTag("rspMenu")
                    .GetComponent<MenuManager>().AddMenu(this);

                eventHandler.On("init", () => {
                    // do nothing
                });
                eventHandler.On("activate", () => {
                    p1Menu.TriggerEvent("activate");
                    p2Menu.TriggerEvent("activate");
                });
                eventHandler.On("deactivate", () => {
                    p1Menu.TriggerEvent("deactivate");
                    p2Menu.TriggerEvent("deactivate");
                });

                eventHandler.On("pause", () => {
                    changeState("inactive");
                });
            }
        }
    }
}