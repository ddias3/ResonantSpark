using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Builder;
using ResonantSpark.Gameplay;
using ResonantSpark.Service;

namespace ResonantSpark {
    namespace CharacterProperties {
        namespace Male0 {
            public class Movement {

                public Movement(AllServices services) {

                }

                public void Init(ICharacterPropertiesCallbackObj charBuilder, FightingGameCharacter fgChar) {
                    charBuilder
                        .WalkSpeed(1.0f)
                        .RunSpeed(charState => {
                            return 2.0f;
                        })
                        .MaxJumpHeight(1.0f);
                }
            }
        }
    }
}