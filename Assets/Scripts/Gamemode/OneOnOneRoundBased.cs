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

            public GameObject inGameUiPrefab;

            public MultipassStateMachine stateMachine;
            public MultipassStateDict states;

            public float roundTime = 60.0f;
            public float maxCharacterDistance = 8.0f;
            protected PlayerService playerService;
            protected UiService uiService;
            protected FightingGameService fgService;

            protected FightingGameCharacter player1;
            protected FightingGameCharacter player2;

            private int char0RoundWins = 0;
            private int char1RoundWins = 0;

            [Tooltip("the rate of in-game time")]
            public float gameTime = 1.0f;

            protected FrameEnforcer frame;
            protected GameTimeManager gameTimeManager;

            protected UI.InGameUi inGameUi;
            protected UnityEvent<UI.InGameUi> unityEventSetInGameUiWhenReady;
            protected UnityEvent<Menu.PauseMenu> unityEventSetPauseMenuWhenReady;

            private UnityAction enablePlayersCallback;
            private UnityAction resetGameCallback;

            public void Awake() {
                frame = GameObject.FindGameObjectWithTag("rspTime").GetComponent<FrameEnforcer>();

                gameTimeManager = GameObject.FindGameObjectWithTag("rspTime").GetComponent<GameTimeManager>();
                gameTimeManager.AddNode(new Func<float, float>(GameTime), new List<string> { "frameDelta", "game" });
                gameTimeManager.AddNode(new Func<float, float>(GameTime), new List<string> { "realDelta", "game" });

                unityEventSetInGameUiWhenReady = new InGameUiCreated();
                unityEventSetPauseMenuWhenReady = new PauseMenuCreated();

                EventManager.StartListening<Events.FrameEnforcerReady>(enablePlayersCallback = new UnityAction(EnablePlayers));
                EventManager.StartListening<Events.StartGame>(enablePlayersCallback = new UnityAction(ResetGame));
            }

            public void OnDestroy() {
                EventManager.StopListening<Events.FrameEnforcerReady>(enablePlayersCallback);
                EventManager.StopListening<Events.StartGame>(enablePlayersCallback);
            }

            public virtual void SetUp(PlayerService playerService, FightingGameService fgService, UiService uiService) {
                this.playerService = playerService;
                this.fgService = fgService;
                this.uiService = uiService;

                Debug.Log("Instantiating in game UI");
                inGameUi = GameObject.Instantiate(inGameUiPrefab).GetComponent<UI.InGameUi>();
                inGameUi.RegisterElements(uiService);

                unityEventSetInGameUiWhenReady.Invoke(inGameUi);
                unityEventSetInGameUiWhenReady.RemoveAllListeners(); // this line might not be necessary.

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

            public void PauseGame(bool pause) {
                frame.PauseExecution(pause);
            }

            public void ResetGame() {
                char0RoundWins = 0;
                char1RoundWins = 0;

                inGameUi.SetRoundWins(0, char0RoundWins);
                inGameUi.SetRoundWins(1, char1RoundWins);
                //uiService.SetValue(element: "roundCounterP1", field: "roundWins", value0: char0RoundWins);
                //uiService.SetValue(element: "roundCounterP2", field: "roundWins", value0: char1RoundWins);
            }

            public void ResetRound() {
                Transform spawnTransform = fgService.GetSpawnPoint();

                player1.position = spawnTransform.position + spawnTransform.rotation * new Vector3(0, 0, fgService.GetSpawnPointOffset());
                player2.position = spawnTransform.position + spawnTransform.rotation * Quaternion.Euler(0, 180, 0) * new Vector3(0, 0, fgService.GetSpawnPointOffset());

                player1.transform.LookAt(player2.position);
                player2.transform.LookAt(player1.position);

                playerService.EachFGChar((id, fgChar) => {
                    fgChar.RoundReset();
                    inGameUi.SetMaxHealth(id, fgChar.maxHealth);
                    inGameUi.SetHealth(id, fgChar.maxHealth);
                    //uiService.SetValue("healthBarP" + (id+1), field: "maxHealth", value0: fgChar.maxHealth);
                    //uiService.SetValue("healthBarP" + (id+1), field: "health", value0: fgChar.maxHealth);
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
                    inGameUi.SetMainScreenText("K.O.");
                    //uiService.SetValue(element: "mainScreenText", field: "text", value0: "K.O.");
                }
                else if (player2.health > player1.health) {
                    char1RoundWins += 1;
                    inGameUi.SetMainScreenText("K.O.");
                    //uiService.SetValue(element: "mainScreenText", field: "text", value0: "K.O.");
                }
                else {
                    // double K.O.
                    char0RoundWins += 1;
                    char1RoundWins += 1;

                    inGameUi.SetMainScreenText("Double K.O.");
                    //uiService.SetValue(element: "mainScreenText", field: "text", value0: "Double K.O.");
                }
            }

            public void TimeOutRound() {
                EndRound();
                inGameUi.SetMainScreenText("Time");
                //uiService.SetValue(element: "mainScreenText", field: "text", value0: "Time");
            }

            public void EndRound() {
                fgService.DisableControl();
                OnRoundEnd();
            }

            private Action<int, int> OnPlayerHealthChange(int playerId) {
                return (hpChange, newHealth) => {
                    inGameUi.SetHealth(playerId, newHealth);
                    //uiService.SetValue("healthBarP" + (playerId + 1), field: "health", value0: newHealth);
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
                inGameUi.SetTime(currRoundTime);
                //uiService.SetValue(element: "roundTimer", field: "time", value0: currRoundTime);
            }

            public void OnGameEntityNumHitsChange(InGameEntity entity, int numHits, int prevNumHits) {
                if (entity == player1) {
                    if (numHits > 0) {
                        inGameUi.SetComboCounter(1, numHits);
                        //uiService.SetValue(element: "comboCounterP1", field: "numHits", value0: numHits);
                    }
                    else if (prevNumHits >= 2) {
                        inGameUi.HideComboCounter(1);
                        //uiService.SetValue(element: "comboCounterP1", field: "hide");
                    }
                }
                else if (entity == player2) {
                    if (numHits > 0) {
                        inGameUi.SetComboCounter(0, numHits);
                        //uiService.SetValue(element: "comboCounterP2", field: "numHits", value0: numHits);
                    }
                    else if (prevNumHits >= 2) {
                        inGameUi.HideComboCounter(0);
                        //uiService.SetValue(element: "comboCounterP2", field: "hide");
                    }
                }
            }

            public void OnHitStunStart(FightingGameCharacter fgChar) {
                if (fgChar == player1) {
                    inGameUi.HealthBarSync(0);
                    inGameUi.HealthBarSyncPause(0, true);
                    //uiService.SetValue("healthBarP1", field: "healthSync");
                    //uiService.SetValue("healthBarP1", field: "healthSyncPause", value0: true);
                }
                else if (fgChar == player2) {
                    inGameUi.HealthBarSync(1);
                    inGameUi.HealthBarSyncPause(1, true);
                    //uiService.SetValue("healthBarP2", field: "healthSync");
                    //uiService.SetValue("healthBarP2", field: "healthSyncPause", value0: true);
                }
            }

            public void OnHitStunEnd(FightingGameCharacter fgChar) {
                if (fgChar == player1) {
                    inGameUi.HealthBarSyncPause(0, false);
                    //uiService.SetValue("healthBarP2", field: "healthSyncPause", value0: false);
                }
                else if (fgChar == player2) {
                    inGameUi.HealthBarSyncPause(1, false);
                    //uiService.SetValue("healthBarP2", field: "healthSyncPause", value0: false);
                }
            }

            public void SetGameTimeScaling(float scaling) {
                gameTime = scaling;
            }

            public float GetGameTimeScaling() {
                return gameTime;
            }

            public UI.InGameUi GetInGameUi() {
                return inGameUi;
            }

            public void GetInGameUiWhenReady(UnityAction<UI.InGameUi> callback) {
                unityEventSetInGameUiWhenReady.AddListener(callback);
            }

            public void GetPauseMenuWhenReady(UnityAction<Menu.PauseMenu> callback) {
                unityEventSetPauseMenuWhenReady.AddListener(callback);
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

        public class InGameUiCreated : UnityEvent<UI.InGameUi> { }
        public class PauseMenuCreated : UnityEvent<Menu.PauseMenu> { }
    }
}
