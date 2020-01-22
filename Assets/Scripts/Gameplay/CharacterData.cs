using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.CharacterProperties;
using ResonantSpark.Input;
using System.Collections;

namespace ResonantSpark {
    namespace Character {
        public class CharacterData {
            private Dictionary<string, Attack> attacks;
            private Dictionary<Attack, Func<CharacterStates.BaseState, Attack, bool>> attackSelectableCallbackMap;

            public CharacterData() {
                attacks = new Dictionary<string, Attack>();
                attackSelectableCallbackMap = new Dictionary<Attack, Func<CharacterStates.BaseState, Attack, bool>>();
            }

            public void AddAttack(Attack attack) {
                attacks.Add(attack.name, attack);
            }

            public void AddAttackSelectablilityCallback(Attack attack, Func<CharacterStates.BaseState, Attack, bool> callback) {
                attackSelectableCallbackMap.Add(attack, callback);
            }

            public Attack ChooseAttackFromSelectability(List<Attack> attacks, CharacterStates.BaseState currState, Attack currAttack) {

                List<Attack> validAttacks = attacks
                    .Where(atk => attackSelectableCallbackMap[atk].Invoke(currState, currAttack))
                    .OrderBy(atk => atk.priority)
                    .ToList();

                if (validAttacks.Count > 0) {
                    return validAttacks[0];
                }
                else {
                    return null;
                }
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
