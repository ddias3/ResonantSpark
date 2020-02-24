using UnityEngine;
using System.Collections;
using System;
using ResonantSpark.Utility;
using ResonantSpark.Gamemode;

namespace ResonantSpark
{
    namespace GamemodeStates
    {
        public class RoundStartMode : GamemodeBaseState
        {
            private GameTimeManager gameTimeManager;
            float elapsedTime;

            // Use this for initialization
            void Awake()
            {
                base.Awake();
                gameTimeManager = GameObject.FindGameObjectWithTag("rspTime").GetComponent<GameTimeManager>();
                states.Register(this, "roundStartMode");
            }

            public override void Enter(int frameIndex, IState previousState)
            {
                elapsedTime = 0;
                Debug.Log("Entered Round Start mode state");
                oneOnOne.ResetRound();
                changeState(states.Get("fightingMode"));
            }

            public override void Execute(int frameIndex)
            {
                elapsedTime += gameTimeManager.Layer("gameTime");
            }

            public override void Exit(int frameIndex)
            { }
        }
    }
}
