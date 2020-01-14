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

            public Attack SelectAttacks(Orientation orientation, GroundRelation groundRelation, InputNotation attackInput) {
                    // TODO: Create a new IEnumerable class to pull out the filtered values in-place.
                List<KeyValuePair<string, Attack>> filteredAttacks = attacks
                    .Where(kvp => kvp.Value.orientation == orientation && kvp.Value.groundRelation == groundRelation && kvp.Value.input == attackInput)
                    .ToList();

                if (filteredAttacks.Count > 1) {
                    Debug.LogErrorFormat("Found multiple attacks for the following data: {0}, {1}, {2}", orientation, groundRelation, attackInput);
                }

                return filteredAttacks[0].Value;
            }

            public Attack SelectAttack(string attackName) {
                return attacks[attackName];
            }
        }
    }
}
