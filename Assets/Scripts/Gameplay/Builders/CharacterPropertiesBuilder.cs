using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Builder;
using ResonantSpark.Character;
using ResonantSpark.Service;

namespace ResonantSpark {
    namespace CharacterProperties {
        public partial class CharacterPropertiesBuilder : ICharacterPropertiesCallbackObj {
            protected AllServices services;

            public CharacterPropertiesBuilder(AllServices services) {
                attacks = new List<(Attack, Func<CharacterStates.CharacterBaseState, Attack, bool>)>();

                this.services = services;
            }

            public CharacterData BuildAttacks(CharacterData charData) {
                foreach ((Attack, Func<CharacterStates.CharacterBaseState, Attack, bool>) atk in attacks) {
                    Attack attack = atk.Item1;
                    Func<CharacterStates.CharacterBaseState, Attack, bool> callback = atk.Item2;

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
        }
    }
}