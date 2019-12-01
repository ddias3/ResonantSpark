using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Builder;
using ResonantSpark.Character;
using ResonantSpark.Gameplay;

namespace ResonantSpark {
    namespace CharacterProperties {
        public class CharacterPropertiesBuilder : ICharacterPropertiesBuilder {
            protected int tempNumber;
            protected List<Attack> attacks;

            public CharacterPropertiesBuilder() {
                attacks = new List<Attack>();
            }

            public FightingGameCharacter CreateCharacter() {
                return null;
            }

            public ICharacterPropertiesBuilder Attack(Attack attack) {
                attacks.Add(attack);
                return this;
            }

            public ICharacterPropertiesBuilder MaxJumpHeight(float maxJumpHeight) {
                return this;
            }

            public ICharacterPropertiesBuilder MaxJumpHeight(Func<CharacterState, float> callback) {
                return this;
            }

            public ICharacterPropertiesBuilder RunSpeed(float speed) {
                return this;
            }

            public ICharacterPropertiesBuilder RunSpeed(Func<CharacterState, float> callback) {
                return this;
            }

            public ICharacterPropertiesBuilder Version(string version) {
                return this;
            }

            public ICharacterPropertiesBuilder WalkSpeed(float speed) {
                return this;
            }

            public ICharacterPropertiesBuilder WalkSpeed(Func<CharacterState, float> callback) {
                return this;
            }
        }
    }
}