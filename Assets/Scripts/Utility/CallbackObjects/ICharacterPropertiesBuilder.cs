using System;
using UnityEngine;

using ResonantSpark.Character;
using ResonantSpark.CharacterProperties;

namespace ResonantSpark {
    namespace Builder {
        public interface ICharacterPropertiesCallbackObj {
            ICharacterPropertiesCallbackObj Version(string version);
            ICharacterPropertiesCallbackObj WalkSpeed(float speed);
            ICharacterPropertiesCallbackObj WalkSpeed(Func<CharacterStates.BaseState, float> callback);
            ICharacterPropertiesCallbackObj RunSpeed(float speed);
            ICharacterPropertiesCallbackObj RunSpeed(Func<CharacterStates.BaseState, float> callback);
            ICharacterPropertiesCallbackObj MaxJumpHeight(float maxJumpHeight);
            ICharacterPropertiesCallbackObj MaxJumpHeight(Func<CharacterStates.BaseState, float> callback);
            ICharacterPropertiesCallbackObj Attack(Attack attack, Func<CharacterStates.BaseState, Attack, bool> callback);
        }
    }
}