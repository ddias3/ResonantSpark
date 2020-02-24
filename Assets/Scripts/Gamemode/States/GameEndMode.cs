using UnityEngine;
using System.Collections;
using System;
using ResonantSpark.Utility;
using ResonantSpark.Gamemode;

namespace ResonantSpark
{
    namespace GamemodeStates
    {
        public class GameEndMode : GamemodeBaseState
        {
            private GameTimeManager gameTimeManager;
            float elapsedTime;

            // Use this for initialization
            void Awake()
            {
                base.Awake();
                gameTimeManager = GameObject.FindGameObjectWithTag("rspTime").GetComponent<GameTimeManager>();
                states.Register(this, "gameEndMode");
            }

            public override void Enter(int frameIndex, IState previousState)
            {
                elapsedTime = 0;
                Debug.Log("Entered Game End mode state");
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
