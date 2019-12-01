using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Builder;

namespace ResonantSpark {
    namespace CharacterProperties {
        namespace Male0 {
            public class Movement : ScriptableObject {
                public void Init(ICharacterPropertiesBuilder charBuilder) {
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