using UnityEngine;
using System.Collections;
using System;
using ResonantSpark.Utility;
using ResonantSpark.Gamemode;

namespace ResonantSpark
{
    namespace GamemodeStates
    {
        public class RoundEndMode : GamemodeBaseState
        {
            private GameTimeManager gameTimeManager;
            float elapsedTime;

            // Use this for initialization
            void Awake()
            {
                base.Awake();
                gameTimeManager = GameObject.FindGameObjectWithTag("rspTime").GetComponent<GameTimeManager>();
                states.Register(this, "roundEndMode");
            }

            public override void Enter(int frameIndex, IState previousState)
            {
                elapsedTime = 0;
                Debug.Log("Entered Round End mode state");
            }

            public override void Execute(int frameIndex)
            {
                elapsedTime += gameTimeManager.Layer("gameTime");

                // logic here for ending a round and transitioning to new state
                int char0Wins = oneOnOne.GetChar0NumWins();
                int char1Wins = oneOnOne.GetChar1NumWins();
                if (char0Wins >= 2 || char1Wins >= 2)
                {
                    changeState(states.Get("gameEndMode"));
                }
                else
                {
                    changeState(states.Get("openingMode"));
                }
            }

            public override void Exit(int frameIndex)
            { }

            public void RoundEnd()
            {
                changeState(this);
            }
        }
    }
}
