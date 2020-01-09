using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.CharacterProperties;
using ResonantSpark.Input;

namespace ResonantSpark {
    namespace Character {
        public class CharacterData : MonoBehaviour {
            private Dictionary<string, Attack> attacks;

            private Dictionary<Orientation, Dictionary<GroundRelation, Dictionary<InputNotation, Attack>>> orientations;
            private Dictionary<GroundRelation, Dictionary<InputNotation, Attack>> groundRelations;
            private Dictionary<InputNotation, Attack> notations;

            public void Start() {
                attacks = new Dictionary<string, Attack>();
            }

            public void CacheAttacks() {

            }

                // This function is a fucking disaster
            public Attack SelectAttack(Orientation orientation, GroundRelation groundRelation, InputNotation attackInput) {
                foreach (KeyValuePair<Orientation, Dictionary<GroundRelation, Dictionary<InputNotation, Attack>>> dic0 in orientations) {
                    if (dic0.Key == orientation) foreach (KeyValuePair<GroundRelation, Dictionary<InputNotation, Attack>> dic1 in dic0.Value) {
                        if (dic1.Key == groundRelation) foreach (KeyValuePair<InputNotation, Attack> entry in dic1.Value) {
                            if (entry.Key == attackInput) {
                                return entry.Value;
                            }
                        }
                    }
                }
                return null;
            }

            public Attack SelectAttack(string attackName) {
                return attacks[attackName];
            }
        }
    }
}
