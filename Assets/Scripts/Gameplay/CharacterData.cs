using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.CharacterProperties;
using ResonantSpark.Input;
using ResonantSpark.Input.Combinations;
using ResonantSpark.Utility;

namespace ResonantSpark {
    namespace Character {
        public class CharacterData : ScriptableObject {
            public int maxHealth { get; private set; }

            private Dictionary<string, Attack> attacks;
            private Dictionary<Attack, Func<CharacterStates.CharacterBaseState, Attack, List<Attack>, bool>> attackSelectableCallbackMap;

            public void Init(int maxHealth) {
                attacks = new Dictionary<string, Attack>();
                attackSelectableCallbackMap = new Dictionary<Attack, Func<CharacterStates.CharacterBaseState, Attack, List<Attack>, bool>>();

                this.maxHealth = maxHealth;
            }

            public void AddAttack(Attack attack) {
                attacks.Add(attack.name, attack);
            }

            public void AddAttackSelectablilityCallback(Attack attack, Func<CharacterStates.CharacterBaseState, Attack, List<Attack>, bool> callback) {
                attackSelectableCallbackMap.Add(attack, callback);
            }

            public Attack ChooseAttackFromSelectability(List<Attack> attacks, CharacterStates.CharacterBaseState currState, Attack currAttack, List<Attack> prevAttacks) {

                List<Attack> validAttacks = attacks
                    .Where(atk => attackSelectableCallbackMap[atk].Invoke(currState, currAttack, prevAttacks))
                    .ToList();

                if (validAttacks.Count > 0) {
                    return validAttacks[0];
                }
                else {
                    return null;
                }
            }

            public List<Attack> SelectAttacks(Orientation orientation, GroundRelation groundRelation, List<Combination> inputCombos, Func<FightingGameAbsInputCodeDir, FightingGameInputCodeDir> directionMapFunc) {
                // TODO: Create a new IEnumerable class to pull out the filtered values in-place.

                List<string> inputNotation = GameInputUtil.CreateInputComboNotation(inputCombos, directionMapFunc);

                List<Attack> filteredAttacks = attacks
                    .Select<KeyValuePair<string, Attack>, Attack>(kvp => kvp.Value)
                    .Where(atk => atk.orientation == orientation && atk.groundRelation == groundRelation && atk.input.ValidInputCombos(inputNotation))
                    .ToList();

                return filteredAttacks;
            }

            public Attack SelectAttack(string attackName) {
                return attacks[attackName];
            }
        }
    }
}
