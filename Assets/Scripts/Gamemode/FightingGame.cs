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

            EventManager.StartListening<Events.FrameEnforcerReady>(new UnityEngine.Events.UnityAction(EnablePlayers));
        }

        private void EnablePlayers() {
            char0.GetComponent<CharacterStates.Init>().StartStateMachine();
            char1.GetComponent<CharacterStates.Init>().StartStateMachine();
        }

        void Update() {

        }
    }
}
