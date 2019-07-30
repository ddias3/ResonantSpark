using System;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    public interface State {
        void Enter(State previousState);
        void Execute(Action<State> changeState);
        void Exit();
    }
}