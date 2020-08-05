using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Gameplay {
        public class BlockRunner : MonoBehaviour {
            public float blockStun { private set; get; }

            private FightingGameCharacter fgChar;

            public void Init(FightingGameCharacter fgChar) {
                this.fgChar = fgChar;
            }

            public void SetBlockStun(float blockStun) {
                this.blockStun = blockStun;
            }

            public void IncrementBlockStun() {
                blockStun -= 1.0f;
            }
        }
    }
}