using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using ResonantSpark.Utility;

namespace ResonantSpark {
    namespace Menu {
        public class TrainingHooks : MonoBehaviour, IHookExpose {

            public ScrollSelect dummyBlock;
            public PauseMenuRunner pauseMenuRunner;

            private Dictionary<string, UnityEventBase> hooks;

            public void Awake() {
                dummyBlock.options = new List<ScrollSelectOption<string>> {
                    new ScrollSelectOption<string> { name = "None", callbackData = "none" },
                    new ScrollSelectOption<string> { name = "All", callbackData = "all" },
                    new ScrollSelectOption<string> { name = "Low", callbackData = "low" },
                    new ScrollSelectOption<string> { name = "High", callbackData = "high" },
                };
                dummyBlock.AddListener(new UnityAction<string>(DummyBlock));

                hooks = new Dictionary<string, UnityEventBase> {
                    { "dummyBlock", dummyBlock.scrollEvent },
                    { "pause", pauseMenuRunner.pauseEvent },
                    { "test", new UnityEvent() },
                };
            }

            public void DummyBlock(string data) {
                Debug.Log("Dummy Block: " + data);
            }

            public Dictionary<string, UnityEventBase> GetHooks() {
                return hooks;
            }
        }
    }
}