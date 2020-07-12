using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Builder;
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
            ICharacterPropertiesCallbackObj Attack(Attack attack, Func<CharacterStates.CharacterBaseState, Attack, List<Attack>, bool> callback);
        }
    }

    namespace CharacterProperties {
        public partial class CharacterPropertiesBuilder : ICharacterPropertiesCallbackObj {
            protected List<(Attack, Func<CharacterStates.CharacterBaseState, Attack, List<Attack>, bool>)> attacks { get; private set; }
            protected string version;
            protected int maxHealth = 100000;//10000;

            public ICharacterPropertiesCallbackObj MaxHealth(int maxHealth) {
                this.maxHealth = maxHealth;
                return this;
            }

            public ICharacterPropertiesCallbackObj MaxJumpHeight(float maxJumpHeight) {
                //TODO: Figure out how to connect this
                return this;
            }

            public ICharacterPropertiesCallbackObj MaxJumpHeight(Func<CharacterStates.CharacterBaseState, float> callback) {
                //TODO: Figure out how to connect this
                return this;
            }

            public ICharacterPropertiesCallbackObj RunSpeed(float speed) {
                //TODO: Figure out how to connect this
                return this;
            }

            public ICharacterPropertiesCallbackObj RunSpeed(Func<CharacterStates.CharacterBaseState, float> callback) {
                //TODO: Figure out how to connect this
                return this;
            }

            public ICharacterPropertiesCallbackObj Version(string version) {
                this.version = version;
                return this;
            }

            public ICharacterPropertiesCallbackObj WalkSpeed(float speed) {
                //TODO: Figure out how to connect this
                return this;
            }

            public ICharacterPropertiesCallbackObj WalkSpeed(Func<CharacterStates.CharacterBaseState, float> callback) {
                //TODO: Figure out how to connect this
                return this;
            }

            public ICharacterPropertiesCallbackObj Attack(Attack attack, Func<CharacterStates.CharacterBaseState, Attack, List<Attack>, bool> callback) {
                attacks.Add((attack, callback));
                return this;
            }
        }
    }
}