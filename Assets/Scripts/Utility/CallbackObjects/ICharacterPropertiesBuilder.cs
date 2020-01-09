using System;
using UnityEngine;

using ResonantSpark.Character;
using ResonantSpark.CharacterProperties;

namespace ResonantSpark {
    namespace Builder {
        public interface ICharacterPropertiesCallbackObj {
            ICharacterPropertiesCallbackObj Version(string version);
            ICharacterPropertiesCallbackObj WalkSpeed(float speed);
            ICharacterPropertiesCallbackObj WalkSpeed(Func<CharacterState, float> callback);
            ICharacterPropertiesCallbackObj RunSpeed(float speed);
            ICharacterPropertiesCallbackObj RunSpeed(Func<CharacterState, float> callback);
            ICharacterPropertiesCallbackObj MaxJumpHeight(float maxJumpHeight);
            ICharacterPropertiesCallbackObj MaxJumpHeight(Func<CharacterState, float> callback);
            ICharacterPropertiesCallbackObj Attack(Attack attack, Func<CharacterState, bool> callback);
        }
    }
}