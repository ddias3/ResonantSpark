using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

using ResonantSpark.Gameplay;
using ResonantSpark.Service;
using ResonantSpark.Utility;

namespace ResonantSpark {
    namespace Gamemode {
        public class OneOnOneRoundBased : MonoBehaviour, IGamemode {

            public MultipassStateMachine stateMachine;
            public MultipassStateDict states;

            public float roundTime = 60.0f;
            public float maxCharacterDistance = 8.0f;
            private PlayerService playerService;
            private UiService uiService;
            private FightingGameService fgService;

            private FightingGameCharacter player1;
            private FightingGameCharacter player2;

            private int char0RoundWins = 0;
            private int char1RoundWins = 0;

            [Tooltip("the rate of in-game time")]
            public float gameTime = 1.0f;

            private FrameEnforcer frame;
            private GameTimeManager gameTimeManager;

            private UnityAction enablePlayersCallback;
            private UnityAction resetGameCallback;

            public void Awake() {
                frame = GameObject.FindGameObjectWithTag("rspTime").GetComponent<FrameEnforcer>();

                gameTimeManager = GameObject.FindGameObjectWithTag("rspTime").GetComponent<GameTimeManager>();
                gameTimeManager.AddNode(new Func<float, float>(GameTime), new List<string> { "frameDelta", "game" });
                gameTimeManager.AddNode(new Func<float, float>(GameTime), new List<string> { "realDelta", "game" });

                EventManager.StartListening<Events.FrameEnforcerReady>(enablePlayersCallback = new UnityAction(EnablePlayers));
                EventManager.StartListening<Events.StartGame>(enablePlayersCallback = new UnityAction(ResetGame));
            }

            public void OnDestroy() {
                EventManager.StopListening<Events.FrameEnforcerReady>(enablePlayersCallback);
                EventManager.StopListening<Events.StartGame>(enablePlayersCallback);
            }

            public void SetUp(PlayerService playerService, FightingGameService fgService, UiService uiService) {
                this.playerService = playerService;
                this.fgService = fgService;
                this.uiService = uiService;

                player1 = playerService.GetFGChar(0);
                player2 = playerService.GetFGChar(1);
                player1.name += "P1";
                player2.name += "P2";

                player1.SetOpponentTarget(player2);
                player2.SetOpponentTarget(player1);

                player1.RegisterOnHealthChangeCallback(OnPlayerHealthChange(0));
                player2.RegisterOnHealthChangeCallback(OnPlayerHealthChange(1));

                player1.RegisterOnEmptyHealthCallback(OnPlayerEmptyHealth);
                player2.RegisterOnEmptyHealthCallback(OnPlayerEmptyHealth);
            }

            public bool IsCurrentFGChar(InGameEntity entity) {
                return entity == player1 || entity == player2;
            }

            private void EnablePlayers() {
                Debug.Log("Enable Players");

                player1.GetComponent<InitMultipassStateMachine>().StartStateMachine(frame);
                player2.GetComponent<InitMultipassStateMachine>().StartStateMachine(frame);
                GetComponent<InitMultipassStateMachine>().StartStateMachine(frame);
            }

            private float GameTime(float input) {
                return input * gameTime;
            }

            public void OpeningMode() {
                fgService.OpeningCamera();
            }

            public void FightingGameMode() {
                fgService.FightingGameCamera();
            }

            public void ResetGame() {
                char0RoundWins = 0;
                char1RoundWins = 0;

                uiService.SetRoundWins(0, char0RoundWins);
                uiService.SetRoundWins(1, char1RoundWins);
            }

            public void ResetRound() {
                Transform spawnTransform = fgService.GetSpawnPoint();

                player1.position = spawnTransform.position + spawnTransform.rotation * new Vector3(0, 0, fgService.GetSpawnPointOffset());
                player2.position = spawnTransform.position + spawnTransform.rotation * Quaternion.Euler(0, 180, 0) * new Vector3(0, 0, fgService.GetSpawnPointOffset());

                player1.transform.LookAt(player2.position);
                player2.transform.LookAt(player1.position);

                playerService.EachFGChar((id, fgChar) => {
                    fgChar.RoundReset();
                    uiService.SetMaxHealth(id, fgChar.maxHealth);
                    uiService.SetHealth(id, fgChar.maxHealth);
                });

                fgService.ResetCamera();
            }

            public float GetRoundLength() {
                return roundTime;
            }

            private void OnRoundEnd() {
                stateMachine.QueueStateChange(states.Get("roundEndMode"));

                if (player1.health > player2.health) {
                    char0RoundWins += 1;
                    uiService.SetMainScreenText("K.O.");
                }
                else if (player2.health > player1.health) {
                    char1RoundWins += 1;
                    uiService.SetMainScreenText("K.O.");
                }
                else {
                    // double K.O.
                    char0RoundWins += 1;
                    char1RoundWins += 1;

                    uiService.SetMainScreenText("Double K.O.");
                }
            }

            public void TimeOutRound() {
                EndRound();
                uiService.SetMainScreenText("Time");
            }

            public void EndRound() {
                fgService.DisableControl();
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

            public void RestrictDistance() {
                Vector3 charDirectLine = player2.position - player1.position;
                if (charDirectLine.magnitude > maxCharacterDistance) {
                    bool char0Moving = false;
                    bool char1Moving = false;
                    
                    charDirectLine.y = 0;

                    if (Vector3.Dot(player1.velocity, charDirectLine) < 0.0f) {
                        Vector3 newVelocity = Vector3.ProjectOnPlane(player1.velocity, charDirectLine);
                        player1.SetVelocity(VelocityPriority.BoundOverride, newVelocity);
                        player1.CalculateFinalVelocity();
                        char0Moving = true;
                    }
                    else if (!player1.Stationary()) {
                        char0Moving = true;
                    }

                    if (Vector3.Dot(player2.velocity, charDirectLine) > 0.0f) {
                        Vector3 newVelocity = Vector3.ProjectOnPlane(player2.velocity, -charDirectLine);
                        player2.SetVelocity(VelocityPriority.BoundOverride, newVelocity);
                        player2.CalculateFinalVelocity();
                        char1Moving = true;
                    }
                    else if (!player2.Stationary()) {
                        char1Moving = true;
                    }

                    float overDistanced = charDirectLine.magnitude - maxCharacterDistance;
                    if (char0Moving) {
                        if (char1Moving) {
                            player1.position += charDirectLine.normalized * overDistanced * 0.5f;
                            player2.position -= charDirectLine.normalized * overDistanced * 0.5f;
                        }
                        else {
                            player1.position += charDirectLine.normalized * overDistanced;
                        }
                    }
                    else if (char1Moving) {
                        player2.position -= charDirectLine.normalized * overDistanced;
                    }
                }
            }

            public void CalculateScreenOrientation() {
                playerService.EachFGChar((id, fgChar) => {
                    fgChar.CalculateScreenOrientation();
                });
            }

            public void SetDisplayTime(float currRoundTime) {
                uiService.SetTime(currRoundTime);
            }

            public void OnGameEntityNumHitsChange(InGameEntity entity, int numHits) {
                if (entity == player1) {
                    if (numHits > 0) {
                        uiService.SetComboCounter(1, numHits);
                    }
                    else {
                        uiService.HideComboCounter(1);
                    }
                }
                else if (entity == player2) {
                    if (numHits > 0) {
                        uiService.SetComboCounter(0, numHits);
                    }
                    else {
                        uiService.HideComboCounter(0);
                    }
                }
            }

            public int GetMaxPlayers() {
                return 2;
            }

            public int GetCharNumWins(int id) {
                if (id == 0) {
                    return char0RoundWins;
                }
                else if (id == 1) {
                    return char1RoundWins;
                }
                else {
                    throw new InvalidOperationException("One-on-one game mode only has 2 players allowed");
                }
            }
        }
    }
}
