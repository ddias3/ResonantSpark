using System;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Builder {
        public interface ICharacterPropertiesBuilder {
            ICharacterPropertiesBuilder Version(string version);
            ICharacterPropertiesBuilder WalkSpeed(float speed);
            ICharacterPropertiesBuilder WalkSpeed(Func<Character.CharacterState, float> callback);
            ICharacterPropertiesBuilder RunSpeed(float speed);
            ICharacterPropertiesBuilder RunSpeed(Func<Character.CharacterState, float> callback);
            ICharacterPropertiesBuilder MaxJumpHeight(float maxJumpHeight);
            ICharacterPropertiesBuilder MaxJumpHeight(Func<Character.CharacterState, float> callback);
            ICharacterPropertiesBuilder Attack(CharacterProperties.Attack attack);
        }
    }
}