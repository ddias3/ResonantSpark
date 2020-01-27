using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

using ResonantSpark.Gameplay;
using ResonantSpark.Service;

namespace ResonantSpark {
    namespace Gamemode {
        public class OneOnOneRoundBased : MonoBehaviour, IGamemode {

            public float roundTime = 60.0f;
            private PlayerService playerService;
            private UIService uiService;
            private FightingGameService fgService;

            private FightingGameCharacter char0;
            private FightingGameCharacter char1;

            private int char0RoundWins = 0;
            private int char1RoundWins = 0;

            [Tooltip("the rate of in-game time")]
            public float gameTime = 1.0f;

            private FrameEnforcer frame;
            private GameTimeManager gameTimeManager;

            private float currRoundTime;

            private int testHP0;
            private int testHP1;

            public void Awake() {
                frame = GameObject.FindGameObjectWithTag("rspTime").GetComponent<FrameEnforcer>();
                frame.AddUpdate((int) FramePriority.Gamemode, new Action<int>(FrameUpdate));
                gameTimeManager = GameObject.FindGameObjectWithTag("rspTime").GetComponent<GameTimeManager>();

                gameTimeManager.AddLayer(new Func<float, float>(GameTime), "gameTime");

                EventManager.StartListening<Events.FrameEnforcerReady>(new UnityAction(EnablePlayers));
                EventManager.StartListening<Events.StartGame>(new UnityAction(StartGame));
            }

            public void SetUp(PlayerService playerService, FightingGameService fgService, UIService uiService) {
                this.playerService = playerService;
                this.fgService = fgService;
                this.uiService = uiService;

                char0 = playerService.GetFGChar(0);
                char1 = playerService.GetFGChar(1);

                char0.SetOpponentCharacter(char1);
                char1.SetOpponentCharacter(char0);
            }

            private void EnablePlayers() {
                Debug.Log("Enable Players");

                char0.GetComponent<CharacterStates.Init>().StartStateMachine(frame);
                char1.GetComponent<CharacterStates.Init>().StartStateMachine(frame);
            }

            private float GameTime(float input) {
                return input * gameTime;
            }

            private void StartGame() {
                ResetRound();

                char0RoundWins = 0;
                char1RoundWins = 1;
            }

            private void ResetRound() {
                currRoundTime = 10.0f; //roundTime;
                uiService.SetTime(currRoundTime);

                Vector3 spawnMid = fgService.GetSpawnPoint().position;
                Quaternion rotation = Quaternion.Euler(0, 180, 0); //Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0);

                char0.transform.position = spawnMid + rotation * new Vector3(0, 0, fgService.GetSpawnPointOffset());
                char1.transform.position = spawnMid + rotation * Quaternion.Euler(0, 180, 0) * new Vector3(0, 0, fgService.GetSpawnPointOffset());

                char0.transform.LookAt(char1.transform);
                char1.transform.LookAt(char0.transform);

                playerService.EachFGChar((id, fgChar) => {
                    fgChar.ResetHealth();
                    uiService.SetMaxHealth(id, fgChar.maxHealth);
                    uiService.SetHealth(id, fgChar.maxHealth);

                    if (id == 0) testHP0 = fgChar.maxHealth;
                    else if (id == 1) testHP1 = fgChar.maxHealth;
                });
            }

            private void OnRoundEnd() {
                if (testHP0 > testHP1) {
                    char0RoundWins += 1;
                }
                else if (testHP1 > testHP0) {
                    char1RoundWins += 1;
                }
                else {
                    // double K.O.
                    char0RoundWins += 1;
                    char1RoundWins += 1;
                }
            }

            private void FrameUpdate(int frameIndex) {
                if (Keyboard.current.digit7Key.wasPressedThisFrame) {
                    testHP0 -= 500;
                    uiService.SetHealth(0, testHP0);
                }

                if (Keyboard.current.digit8Key.wasPressedThisFrame) {
                    testHP1 -= 500;
                    uiService.SetHealth(1, testHP1);
                }

                uiService.SetTime(currRoundTime);

                if (currRoundTime < 0) {
                    ResetRound();
                }
                currRoundTime -= gameTimeManager.Layer("gameTime");
            }

            public int GetMaxPlayers() {
                return 2;
            }
        }
    }
}
