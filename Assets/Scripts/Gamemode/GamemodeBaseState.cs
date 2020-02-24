using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Input.Combinations;
using ResonantSpark.Gameplay;
using ResonantSpark.Character;
using ResonantSpark.Utility;
using ResonantSpark.Gamemode;

namespace ResonantSpark
{
    namespace GamemodeStates
    {
        public abstract class GamemodeBaseState : BaseState
        {
            protected OneOnOneRoundBased oneOnOne;
            public new void Awake()
            {
                base.Awake();

                oneOnOne = gameObject.GetComponentInParent<OneOnOneRoundBased>();
            }
        }
    }
}
