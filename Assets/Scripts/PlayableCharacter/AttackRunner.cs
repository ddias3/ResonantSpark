using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Input;
using ResonantSpark.Character;
using ResonantSpark.Utility;
using ResonantSpark.Service;

namespace ResonantSpark {
    namespace Gameplay {
        public class AttackRunner : MonoBehaviour {
            private FightingGameCharacter fgChar;

            private List<CharacterProperties.Attack> prevAttacks;

            private CharacterProperties.Attack queuedUpAttack;
            private CharacterProperties.Attack activeAttack;
            private bool startRequired = false;

            private Action onCompleteAttackCallback;

            public void Init(FightingGameCharacter fgChar) {
                this.fgChar = fgChar;
                this.prevAttacks = new List<CharacterProperties.Attack>();
                onCompleteAttackCallback = new Action(OnCompleteAttack);
            }

            public void ChooseAttack(CharacterData charData, CharacterStates.CharacterBaseState currState, CharacterProperties.Attack currAttack, FightingGameInputCodeBut button, FightingGameInputCodeDir direction = FightingGameInputCodeDir.None) {
                StringBuilder prevAttackStr = new StringBuilder();
                prevAttackStr.Append("[");
                prevAttacks.ForEach(atk => { prevAttackStr.Append(atk.ToString()).Append(","); });
                prevAttackStr.Append("]");
                Debug.LogFormat(prevAttackStr.ToString());

                InputNotation notation = GameInputUtil.SelectInputNotation(button, direction);

                List<CharacterProperties.Attack> attackCandidates = charData.SelectAttacks(fgChar.GetOrientation(), fgChar.GetGroundRelation(), notation);
                CharacterProperties.Attack attack = charData.ChooseAttackFromSelectability(attackCandidates, currState, currAttack, prevAttacks);

                if (attack != null) {
                    fgChar.SetState(attack.initCharState);

                    if (currAttack != null) {
                        prevAttacks.Add(currAttack);
                    }

                    attack.SetOnCompleteCallback(onCompleteAttackCallback);

                    this.queuedUpAttack = attack;
                    startRequired = true;
                }
            }

            public void RunFrame() {
                activeAttack?.RunFrame();
            }

            public void StartAttackIfRequired(int frameIndex) {
                if (startRequired) {
                    activeAttack = queuedUpAttack;
                    activeAttack.StartPerformable(frameIndex);
                }
                else {
                    Debug.LogWarning("Start Called without being required");
                }
            }

            public CharacterProperties.Attack GetCurrentAttack() {
                return activeAttack;
            }

            public List<Gameplay.Hit> GetNextHitInCurrentAttack() {
                if (activeAttack != null) {
                    return activeAttack.GetNextHit();
                }
                else {
                    return null;
                }
            }

            public CharacterVulnerability GetCharacterVulnerability() {
                // TODO: make sure this doesn't ever fuck up.
                return activeAttack.GetCharacterVulnerability();
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