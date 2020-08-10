using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Character;
using ResonantSpark.Utility;
using ResonantSpark.Service;
using ResonantSpark.Input;
using ResonantSpark.Input.Combinations;

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

            public void ChooseAttack(CharacterData charData, CharacterStates.CharacterBaseState currState, CharacterProperties.Attack currAttack, List<Combination> inputCombos, Func<FightingGameAbsInputCodeDir, FightingGameInputCodeDir> directionMapFunc) {
                StringBuilder prevAttackStr = new StringBuilder();
                prevAttackStr.Append("[");
                prevAttacks.ForEach(atk => { prevAttackStr.Append(atk.ToString()).Append(","); });
                prevAttackStr.Append("]");
                Debug.LogFormat(prevAttackStr.ToString());

                List<CharacterProperties.Attack> attackCandidates = charData.SelectAttacks(fgChar.GetOrientation(), fgChar.GetGroundRelation(), inputCombos, directionMapFunc);
                CharacterProperties.Attack attack = charData.ChooseAttackFromSelectability(attackCandidates, currState, currAttack, prevAttacks);

                if (attack != null) {
                    fgChar.SetState(attack.GetInitAttackState());

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