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

            public StateMachine stateMachine;

            public float roundTime = 60.0f;
            private PlayerService playerService;
            private UiService uiService;
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

            public void SetUp(PlayerService playerService, FightingGameService fgService, UiService uiService) {
                this.playerService = playerService;
                this.fgService = fgService;
                this.uiService = uiService;

                char0 = playerService.GetFGChar(0);
                char1 = playerService.GetFGChar(1);

                char0.SetOpponentCharacter(char1);
                char1.SetOpponentCharacter(char0);

                char0.RegisterOnHealthChangeCallback(OnPlayerHealthChange(0));
                char1.RegisterOnHealthChangeCallback(OnPlayerHealthChange(1));

                char0.RegisterOnEmptyHealthCallback(OnPlayerEmptyHealth);
                char1.RegisterOnEmptyHealthCallback(OnPlayerEmptyHealth);
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
                currRoundTime = 20.0f; //roundTime;
                uiService.SetTime(currRoundTime);

                Vector3 spawnMid = fgService.GetSpawnPoint().position;
                Quaternion rotation = Quaternion.Euler(0, 180, 0); //Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0);

                char0.transform.position = spawnMid + rotation * new Vector3(0, 0, fgService.GetSpawnPointOffset());
                char1.transform.position = spawnMid + rotation * Quaternion.Euler(0, 180, 0) * new Vector3(0, 0, fgService.GetSpawnPointOffset());

                char0.transform.LookAt(char1.transform);
                char1.transform.LookAt(char0.transform);

                playerService.EachFGChar((id, fgChar) => {
                    fgChar.RoundReset();
                    uiService.SetMaxHealth(id, fgChar.maxHealth);
                    uiService.SetHealth(id, fgChar.maxHealth);

                    if (id == 0) testHP0 = fgChar.maxHealth;
                    else if (id == 1) testHP1 = fgChar.maxHealth;
                });

                fgService.ResetCamera();
            }

            private void OnRoundEnd() {
                uiService.SetTime(0.0f);

                if (char0.health > char1.health) {
                    char0RoundWins += 1;
                }
                else if (char1.health > char0.health) {
                    char1RoundWins += 1;
                }
                else {
                    // double K.O.
                    char0RoundWins += 1;
                    char1RoundWins += 1;
                }

                // TODO: Remove this and replace it with a state-machine.
                ResetRound();
            }

            private void EndRound() {
                //TODO: Stop inputs from both players, or don't not sure.
                OnRoundEnd();
            }

            private Action<int, int> OnPlayerHealthChange(int playerId) {
                return (hpChange, newHealth) => {
                    uiService.SetHealth(playerId, newHealth);
                };
            }

            private void OnPlayerEmptyHealth(FightingGameCharacter fgChar) {
                    // TODO: this might result in a race condition that needs to be resolved for double K.O.s
                EndRound();
            }

            private void FrameUpdate(int frameIndex) {
                if (Keyboard.current.digit0Key.wasPressedThisFrame) {
                    ResetRound();
                }

                playerService.EachFGChar((id, fgChar) => {
                    fgChar.CalculateScreenOrientation();
                });

                uiService.SetTime(currRoundTime);

                if (currRoundTime < 0) {
                    EndRound();
                }
                currRoundTime -= gameTimeManager.Layer("gameTime");

                // gameTime.Layer("gameTime") is Time.deltaTime
            }

            public int GetMaxPlayers() {
                return 2;
            }
        }
    }
}
