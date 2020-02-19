using System;
using UnityEngine;

using ResonantSpark.Character;
using ResonantSpark.CharacterProperties;

namespace ResonantSpark {
    namespace Builder {
        public interface ICharacterPropertiesCallbackObj {
            ICharacterPropertiesCallbackObj Version(string version);
            ICharacterPropertiesCallbackObj MaxHealth(int maxHealth);
            ICharacterPropertiesCallbackObj WalkSpeed(float speed);
            ICharacterPropertiesCallbackObj WalkSpeed(Func<CharacterStates.CharacterBaseState, float> callback);
            ICharacterPropertiesCallbackObj RunSpeed(float speed);
            ICharacterPropertiesCallbackObj RunSpeed(Func<CharacterStates.CharacterBaseState, float> callback);
            ICharacterPropertiesCallbackObj MaxJumpHeight(float maxJumpHeight);
            ICharacterPropertiesCallbackObj MaxJumpHeight(Func<CharacterStates.CharacterBaseState, float> callback);
            ICharacterPropertiesCallbackObj Attack(Attack attack, Func<CharacterStates.CharacterBaseState, Attack, bool> callback);
        }
    }
}