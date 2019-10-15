using System;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    public class FightingGame : MonoBehaviour {

        public GameObject char0;
        public GameObject char1;

        public new GameObject camera;

        private FrameEnforcer frame;

        public void Start() {
            frame = gameObject.GetComponent<FrameEnforcer>();

            char0.GetComponent<FightingGameCharacter>().opponentCharacter = char1;
            //char1.GetComponent<FightingGameCharacter>().opponentCharacter = char0;

            EventManager.StartListening<Events.FrameEnforcerReady>(new UnityEngine.Events.UnityAction(EnablePlayers));
        }

        private void EnablePlayers() {
            Debug.Log("Enable Players");

            char0.GetComponent<CharacterStates.Init>().StartStateMachine(frame);
            //char1.GetComponent<CharacterStates.Init>().StartStateMachine(frame);
        }

        void Update() {

        }
    }
}
