using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using ResonantSpark.Gameplay;
using ResonantSpark.Service;

namespace ResonantSpark {
    namespace Gamemode {
        public class OneOnOneRoundBased : MonoBehaviour, IGamemode {

            private FightingGameCharacter char0;
            private FightingGameCharacter char1;

            [Tooltip("the rate of in-game time")]
            public float gameTime = 1.0f;

            private FrameEnforcer frame;
            private GameTimeManager gameTimeManager;

            private float roundTime;

            public void Awake() {
                frame = GameObject.FindGameObjectWithTag("rspTime").GetComponent<FrameEnforcer>();
                frame.AddUpdate((int) FramePriority.Gamemode, new Action<int>(FrameUpdate));
                gameTimeManager = GameObject.FindGameObjectWithTag("rspTime").GetComponent<GameTimeManager>();

                gameTimeManager.AddLayer(new Func<float, float>(GameTime), "gameTime");

                EventManager.StartListening<Events.FrameEnforcerReady>(new UnityAction(EnablePlayers));
                EventManager.StartListening<Events.StartGame>(new UnityAction(StartGame));
            }

            public void SetUp(PlayerService playerService, FightingGameService fgService) {
                char0 = playerService.GetFGChar(0);
                char1 = playerService.GetFGChar(1);

                char0.SetOpponentCharacter(char1);
                char1.SetOpponentCharacter(char0);

                Vector3 spawnMid = fgService.GetSpawnPoint().position;

                Quaternion rotation = Quaternion.Euler(0, 180, 0); //Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0);

                char0.transform.position = spawnMid + rotation * new Vector3(0, 0, fgService.GetSpawnPointOffset());
                char1.transform.position = spawnMid + rotation * Quaternion.Euler(0, 180, 0) * new Vector3(0, 0, fgService.GetSpawnPointOffset());

                char0.transform.LookAt(char1.transform);
                char1.transform.LookAt(char0.transform);
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
            }

            private void ResetRound() {
                roundTime = 60.0f;
            }

            void FrameUpdate(int frameIndex) {

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
