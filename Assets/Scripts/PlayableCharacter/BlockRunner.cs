using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Gameplay {
        public class BlockRunner : MonoBehaviour {
            private FightingGameCharacter fgChar;

            private float blockStun = 0.0f;

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