using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.CharacterProperties;
using ResonantSpark.Input;
using System.Collections;

namespace ResonantSpark {
    namespace Character {
        public class CharacterData : MonoBehaviour {
            private Dictionary<string, Attack> attacks;
            private Dictionary<Attack, Func<CharacterState, bool>> attackSelectableCallbackMap;

            public void Start() {
                attacks = new Dictionary<string, Attack>();
            }

            public void AddAttack(Attack attack) {
                attacks.Add(attack.name, attack);
            }

            public void AddAttackSelectablilityCallback(Attack attack, Func<CharacterState, bool> callback) {
                attackSelectableCallbackMap.Add(attack, callback);
            }

            public List<Attack> SelectAttacks(Orientation orientation, GroundRelation groundRelation, InputNotation attackInput) {
                // TODO: Create a new IEnumerable class to pull out the filtered values in-place.

                List<Attack> filteredAttacks = attacks
                    .Select<KeyValuePair<string, Attack>, Attack>(kvp => kvp.Value)
                    .Where(atk => atk.orientation == orientation && atk.groundRelation == groundRelation && atk.input == attackInput)
                    .ToList();

                return filteredAttacks;
            }

            public Attack SelectAttack(string attackName) {
                return attacks[attackName];
            }
        }
    }
}
