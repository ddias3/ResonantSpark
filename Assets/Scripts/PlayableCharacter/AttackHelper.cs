using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Input;
using ResonantSpark.Character;
using ResonantSpark.Utility;

namespace ResonantSpark {
    namespace Gameplay {
        public class AttackHelper {
            private FightingGameCharacter fgChar;

            private List<CharacterProperties.Attack> prevAttacks;

            public AttackHelper(FightingGameCharacter fgChar) {
                this.fgChar = fgChar;

                this.prevAttacks = new List<CharacterProperties.Attack>();
            }

            public CharacterProperties.Attack ChooseAttack(CharacterData charData, CharacterStates.CharacterBaseState currState, CharacterProperties.Attack currAttack, FightingGameInputCodeBut button, FightingGameInputCodeDir direction = FightingGameInputCodeDir.None) {
                InputNotation notation = GameInputUtil.SelectInputNotation(button, direction);

                List<CharacterProperties.Attack> attackCandidates = charData.SelectAttacks(fgChar.GetOrientation(), fgChar.GetGroundRelation(), notation);
                CharacterProperties.Attack attack = charData.ChooseAttackFromSelectability(attackCandidates, currState, currAttack, prevAttacks);

                return attack;
            }

            public void AddAttackToPrevs(CharacterProperties.Attack attack) {
                prevAttacks.Add(attack);
            }

            public void ClearPrevAttacks() {
                prevAttacks.Clear();
            }
        }
    }
}