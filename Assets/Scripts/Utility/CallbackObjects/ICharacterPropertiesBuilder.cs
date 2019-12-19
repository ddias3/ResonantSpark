using System;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Builder {
        public interface ICharacterPropertiesCallbackObj {
            ICharacterPropertiesCallbackObj Version(string version);
            ICharacterPropertiesCallbackObj WalkSpeed(float speed);
            ICharacterPropertiesCallbackObj WalkSpeed(Func<Character.CharacterState, float> callback);
            ICharacterPropertiesCallbackObj RunSpeed(float speed);
            ICharacterPropertiesCallbackObj RunSpeed(Func<Character.CharacterState, float> callback);
            ICharacterPropertiesCallbackObj MaxJumpHeight(float maxJumpHeight);
            ICharacterPropertiesCallbackObj MaxJumpHeight(Func<Character.CharacterState, float> callback);
            ICharacterPropertiesCallbackObj Attack(CharacterProperties.Attack attack);
        }
    }
}