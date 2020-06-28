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

                char0 = playerService.GetFGChar(0);
                char1 = playerService.GetFGChar(1);

                char0.SetOpponentTarget(char1);
                char1.SetOpponentTarget(char0);

                char0.RegisterOnHealthChangeCallback(OnPlayerHealthChange(0));
                char1.RegisterOnHealthChangeCallback(OnPlayerHealthChange(1));

                char0.RegisterOnEmptyHealthCallback(OnPlayerEmptyHealth);
                char1.RegisterOnEmptyHealthCallback(OnPlayerEmptyHealth);
            }

            private void EnablePlayers() {
                Debug.Log("Enable Players");

                char0.GetComponent<InitMultipassStateMachine>().StartStateMachine(frame);
                char1.GetComponent<InitMultipassStateMachine>().StartStateMachine(frame);
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

                char0.position = spawnTransform.position + spawnTransform.rotation * new Vector3(0, 0, fgService.GetSpawnPointOffset());
                char1.position = spawnTransform.position + spawnTransform.rotation * Quaternion.Euler(0, 180, 0) * new Vector3(0, 0, fgService.GetSpawnPointOffset());

                char0.transform.LookAt(char1.position);
                char1.transform.LookAt(char0.position);

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

                if (char0.health > char1.health) {
                    char0RoundWins += 1;
                    uiService.SetMainScreenText("K.O.");
                }
                else if (char1.health > char0.health) {
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
                
            }

            public void CalculateScreenOrientation() {
                playerService.EachFGChar((id, fgChar) => {
                    fgChar.CalculateScreenOrientation();
                });
            }

            public void SetDisplayTime(float currRoundTime) {
                uiService.SetTime(currRoundTime);
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
