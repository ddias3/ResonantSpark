using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    public class FightingGame : MonoBehaviour {

        public GameObject char0;
        public GameObject char1;

        public new GameObject camera;

        void Start() {
            char0.GetComponent<FightingGameCharacter>().opponentCharacter = char1;
            char1.GetComponent<FightingGameCharacter>().opponentCharacter = char0;
        }

        void Update() {

        }
    }
}
