using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Gameplay {
        public class ComboRunner : MonoBehaviour {
            [Tooltip("Max number of grabs on opponent that is in hit stun.")]
            public int maxGrabs = 1;
            [Tooltip("Max number of wall bounces on opponent that is in hit stun.")]
            public int maxWallBounces = 2;

            [Tooltip("Number of hits for hit stun to decrease by a fixed amount")]
            public int numHitsStunDecreaseLevel1 = 6;
            [Tooltip("Amount of \"frames\" to remove from any future hits.")]
            public float stunDecreaseLevel1 = 4.0f;

            [Tooltip("Number of hits for hit stun to decrease by a fixed amount")]
            public int numHitsStunDecreaseLevel2 = 12;
            [Tooltip("Amount of \"frames\" to remove from any future hits.")]
            public float stunDecreaseLevel2 = 7.0f;

            public int numHits { private set; get; }
            public int numGrabs { private set; get; }
            public int numWallBounces { private set; get; }

            private FightingGameCharacter fgChar;

            public void Init(FightingGameCharacter fgChar) {
                this.fgChar = fgChar;
                ResetCounts();
            }

            public void ResetCounts() {
                numHits = 0;
                numGrabs = 0;
                numWallBounces = 0;
            }

            public float GetFilteredHitStun(float hitStun) {
                if (numHits < numHitsStunDecreaseLevel1) {
                    return hitStun;
                }
                else if (numHits < numHitsStunDecreaseLevel2) {
                    return hitStun - stunDecreaseLevel1;
                }
                else {
                    return hitStun - stunDecreaseLevel2;
                }
            }

            public void AddNumHits(int hits) {
                numHits += hits;
            }

            public void AddNumGrabs(int grabs) {
                numGrabs += grabs;
            }

            public void AddWallBounces(int wallBounces) {
                numWallBounces += wallBounces;
            }
        }
    }
}