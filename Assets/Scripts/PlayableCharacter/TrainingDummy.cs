using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Gameplay {
        public enum DummyBlock : int {
            None,
            Low,
            High,
            All
        };

        public class TrainingDummy : MonoBehaviour {
            public FightingGameCharacter fgChar;

            private DummyBlock blockMode;

            public void SetBlockMode(string mode) {
                switch (mode) {
                    case "none":
                        blockMode = DummyBlock.None;
                        break;
                    case "all":
                        blockMode = DummyBlock.All;
                        break;
                    case "low":
                        blockMode = DummyBlock.Low;
                        break;
                    case "high":
                        blockMode = DummyBlock.High;
                        break;
                }
            }
        }
    }
}