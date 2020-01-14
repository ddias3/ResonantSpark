using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Builder;
using ResonantSpark.Character;
using ResonantSpark.Service;
using ResonantSpark.Gameplay;

namespace ResonantSpark {
    namespace CharacterProperties {
        public class CharacterPropertiesBuilder : ICharacterPropertiesCallbackObj {
            protected string version;
            protected List<(Attack, Func<CharacterState, bool>)> attacks { get; private set; }

            protected AllServices services;

            public CharacterPropertiesBuilder(AllServices services) {
                attacks = new List<(Attack, Func<CharacterState, bool>)>();

                this.services = services;
            }

            public CharacterData BuildAttacks(CharacterData charData) {
                foreach ((Attack, Func<CharacterState, bool>) atk in attacks) {
                    Attack attack = atk.Item1;
                    Func<CharacterState, bool> callback = atk.Item2;

                    attack.BuildAttack(services);

                    charData.AddAttackSelectablilityCallback(attack, callback);
                    charData.AddAttack(attack);
                }

                return charData;
            }

            //TODO:
            //public void BuildMovement() {
            //    
            //}

            public ICharacterPropertiesCallbackObj MaxJumpHeight(float maxJumpHeight) {
                //TODO: Figure out how to connect this
                return this;
            }

            public ICharacterPropertiesCallbackObj MaxJumpHeight(Func<CharacterState, float> callback) {
                //TODO: Figure out how to connect this
                return this;
            }

            public ICharacterPropertiesCallbackObj RunSpeed(float speed) {
                //TODO: Figure out how to connect this
                return this;
            }

            public ICharacterPropertiesCallbackObj RunSpeed(Func<CharacterState, float> callback) {
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

            public ICharacterPropertiesCallbackObj WalkSpeed(Func<CharacterState, float> callback) {
                //TODO: Figure out how to connect this
                return this;
            }

            public ICharacterPropertiesCallbackObj Attack(Attack attack, Func<CharacterState, bool> callback) {
                attacks.Add((attack, callback));
                return this;
            }
        }
    }
}