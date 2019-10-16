using System;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    public interface State {
        void OnStateMachineEnable(Action<State> changeState);
        void Enter(int frameIndex, State previousState);
        void Execute(int frameIndex);
        void Exit(int frameIndex);
    }
}