﻿using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Input;
using ResonantSpark.Character;
using ResonantSpark.Utility;

namespace ResonantSpark {
    namespace Gameplay {
        public class AttackRunner : MonoBehaviour {
            public string testString;

            private FightingGameCharacter fgChar;

            private List<CharacterProperties.Attack> prevAttacks;

            private CharacterProperties.Attack activeAttack;
            private bool startRequired = false;

            public void Init(FightingGameCharacter fgChar) {
                this.fgChar = fgChar;
                this.prevAttacks = new List<CharacterProperties.Attack>();
            }

            public void ChooseAttack(CharacterData charData, CharacterStates.CharacterBaseState currState, CharacterProperties.Attack currAttack, FightingGameInputCodeBut button, FightingGameInputCodeDir direction = FightingGameInputCodeDir.None) {
                Debug.Log(prevAttacks);

                InputNotation notation = GameInputUtil.SelectInputNotation(button, direction);

                List<CharacterProperties.Attack> attackCandidates = charData.SelectAttacks(fgChar.GetOrientation(), fgChar.GetGroundRelation(), notation);
                CharacterProperties.Attack attack = charData.ChooseAttackFromSelectability(attackCandidates, currState, currAttack, prevAttacks);

                if (attack != null) {
                    fgChar.SetState(attack.initCharState);

                    if (currAttack != null) {
                        prevAttacks.Add(currAttack);
                    }

                    attack.SetOnCompleteCallback(new Action(OnCompleteAttack));

                    this.activeAttack = attack;
                    startRequired = true;
                }
            }

            public void RunFrame() {
                activeAttack?.RunFrame();
            }

            public void StartAttackIfRequired(int frameIndex) {
                if (startRequired) {
                    activeAttack.StartPerformable(frameIndex);
                }
            }

            public CharacterProperties.Attack GetCurrentAttack() {
                return activeAttack;
            }

            public void OnCompleteAttack() {
                activeAttack = null;
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