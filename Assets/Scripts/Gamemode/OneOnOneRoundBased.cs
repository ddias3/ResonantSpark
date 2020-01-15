using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using ResonantSpark.Gameplay;

namespace ResonantSpark {
    namespace Gamemode {
        public class OneOnOneRoundBased : MonoBehaviour, IGamemode {

            private FightingGameCharacter char0;
            private FightingGameCharacter char1;

            public new GameObject camera;

            [Tooltip("the rate of in-game time")]
            public float gameTime = 1.0f;

            private FrameEnforcer frame;
            private GameTimeManager gameTimeManager;

            private float roundTime;

            public void Start() {
                frame = GameObject.FindGameObjectWithTag("rspTime").GetComponent<FrameEnforcer>();
                gameTimeManager = GameObject.FindGameObjectWithTag("rspTime").GetComponent<GameTimeManager>();

                gameTimeManager.AddLayer(new Func<float, float>(GameTime), "gameTime");

                EventManager.StartListening<Events.FightingGameCharsReady>(new UnityAction(ConnectFightingGameCharacters));
                EventManager.StartListening<Events.FrameEnforcerReady>(new UnityAction(EnablePlayers));
            }

            public void ConnectFightingGameCharacters() {
                Debug.Log("ConnectFightingGameCharacters");

                char0
                    .SetOpponentCharacter(char1.gameObject)
                    .SetGameTimeManager(gameTimeManager);
                char1
                    .SetOpponentCharacter(char0.gameObject)
                    .SetGameTimeManager(gameTimeManager);
            }

            private void EnablePlayers() {
                Debug.Log("Enable Players");

                char0.GetComponent<CharacterStates.Init>().StartStateMachine(frame);
                char1.GetComponent<CharacterStates.Init>().StartStateMachine(frame);

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

            public void SetFightingGameCharacter(FightingGameCharacter fgChar, int index) {
                switch (index) {
                    case 0:
                        char0 = fgChar;
                        break;
                    case 1:
                        char1 = fgChar;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("Index " + index + " out of range for setting a character for 1-v-1 gamemode");
                }
            }

            public int GetMaxPlayers() {
                return 2;
            }
        }
    }
}
