using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Menu {
        public enum MenuState : int {
            Transition,
            Active,
            Inactive,
        }

        [RequireComponent(typeof(Animator))]
        public abstract class Menu : MonoBehaviour {
            protected Animator animator;
            protected MenuEventHandler eventHandler;

            public void Awake() {
                eventHandler = new MenuEventHandler();
            }

            public void Start() {
                animator = GetComponent<Animator>();
            }

            public void TriggerEvent(string eventName) {
                eventHandler.TriggerEvent(eventName);
            }
        }
    }
}