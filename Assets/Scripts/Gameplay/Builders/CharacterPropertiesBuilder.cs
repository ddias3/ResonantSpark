using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Builder;
using ResonantSpark.Character;

namespace ResonantSpark {
    namespace CharacterProperties {
        public class CharacterPropertiesBuilder : ICharacterPropertiesCallbackObj {
            protected int tempNumber;
            protected List<Attack> attacks { get; private set; }

            public CharacterPropertiesBuilder() {
                attacks = new List<Attack>();
            }

            public ICharacterPropertiesCallbackObj Attack(Attack attack) {
                attacks.Add(attack);
                return this;
            }

            public ICharacterPropertiesCallbackObj MaxJumpHeight(float maxJumpHeight) {
                return this;
            }

            public ICharacterPropertiesCallbackObj MaxJumpHeight(Func<CharacterState, float> callback) {
                return this;
            }

            public ICharacterPropertiesCallbackObj RunSpeed(float speed) {
                return this;
            }

            public ICharacterPropertiesCallbackObj RunSpeed(Func<CharacterState, float> callback) {
                return this;
            }

            public ICharacterPropertiesCallbackObj Version(string version) {
                return this;
            }

            public ICharacterPropertiesCallbackObj WalkSpeed(float speed) {
                return this;
            }

            public ICharacterPropertiesCallbackObj WalkSpeed(Func<CharacterState, float> callback) {
                return this;
            }
        }
    }
}