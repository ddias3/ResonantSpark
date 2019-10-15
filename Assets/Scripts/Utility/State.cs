using System;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    public interface State {
        void Enter(int frameIndex, State previousState);
        void Execute(int frameIndex, Action<State> changeState);
        void Exit(int frameIndex);
    }
}