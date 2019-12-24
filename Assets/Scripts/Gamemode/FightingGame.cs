using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ResonantSpark {
    namespace Gameplay {
        public class FightingGame : MonoBehaviour {

            public GameObject char0;
            public GameObject char1;

            public new GameObject camera;

            [Tooltip("the rate of in-game time")]
            public float gameTime = 1.0f;

            private FrameEnforcer frame;
            private GameTimeManager gameTimeManager;

            private float roundTime;

            public void Start() {
                frame = gameObject.GetComponent<FrameEnforcer>();
                gameTimeManager = gameObject.GetComponent<GameTimeManager>();

                gameTimeManager.AddLayer(new Func<float, float>(GameTime), "gameTime");

                EventManager.StartListening<Events.FightingGameCharsReady>(new UnityAction(ConnectFightingGameCharacters));
                EventManager.StartListening<Events.FrameEnforcerReady>(new UnityAction(EnablePlayers));
            }

            public void ConnectFightingGameCharacters() {
                Debug.Log("ConnectFightingGameCharacters");

                char0.GetComponent<FightingGameCharacter>()
                    .SetOpponentCharacter(char1)
                    .SetGameTimeManager(gameTimeManager);
                //char1.GetComponent<FightingGameCharacter>()
                //    .SetOpponentCharacter(char0)
                //    .SetGameTimeManager(gameTimeManager);
            }

            private void EnablePlayers() {
                Debug.Log("Enable Players");

                char0.GetComponent<CharacterStates.Init>().StartStateMachine(frame);
                //char1.GetComponent<CharacterStates.Init>().StartStateMachine(frame);

                //ResetRound();
            }

            private float GameTime(float input) {
                return input * gameTime;
            }

            private void ResetRound() {
                roundTime = 60.0f;
            }

            void Update() {

            }
        }
    }
}
