using System;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace CharacterStates {
        public class AttackServer : StateHelper {

            private List<CharacterProperties.Attack> attackQueue;

            public new void Start() {
                base.Start();

                attackQueue = new List<CharacterProperties.Attack>();
            }

            public void AddAttack(CharacterProperties.Attack attack) {
                attackQueue.Add(attack);
            }

            public CharacterProperties.Attack ServeAttack() {
                if (attackQueue.Count > 0) {
                    CharacterProperties.Attack attack = attackQueue[0];
                    attackQueue.RemoveAt(0);
                    return attack;
                }
                else {
                    return null;
                }
            }
        }
    }
}