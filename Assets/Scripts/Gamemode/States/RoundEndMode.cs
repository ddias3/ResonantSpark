using UnityEngine;
using System.Collections;
using System;
using ResonantSpark.Utility;
using ResonantSpark.Gamemode;

namespace ResonantSpark
{
    namespace GamemodeStates
    {
        public class RoundEndMode : BaseState
        {
            private OneOnOneRoundBased oneOnOne;
            private GameTimeManager gameTimeManager;
            float elapsedTime;

            // Use this for initialization
            void Awake()
            {
                base.Awake();
                oneOnOne = gameObject.GetComponentInParent<OneOnOneRoundBased>();
                gameTimeManager = GameObject.FindGameObjectWithTag("rspTime").GetComponent<GameTimeManager>();
            }

            public override void Enter(int frameIndex, IState previousState)
            {
                elapsedTime = 0;
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
