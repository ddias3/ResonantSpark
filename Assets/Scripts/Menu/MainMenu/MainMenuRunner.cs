using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Menu {
        public class MainMenuRunner : MenuRunner {

            public Menu mainMenu;

            public new void Start() {
                base.Start();
            }
            
            public void DelayedStart() {
                Persistence persistence = Persistence.GetPersistence();
                if (persistence.firstTimeLoad) {
                    persistence.MenuLoaded();

                    stateMachine.ChangeState(states.Get("intro"));

                    //animatorDict["camera"].Play("intro");
                }
                else {
                    //animatorDict["camera"].Play("introSkip");
                    //animatorDict["mainMenu"].Play("activate");

                    //mainMenu.TriggerEvent("activate");

                    stateMachine.ChangeState(states.Get("introSkip"));
                }
            }
        }
    }
}
