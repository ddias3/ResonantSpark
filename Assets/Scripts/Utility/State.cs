using System;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    public interface IState {
        void OnStateMachineEnable(Action<IState> changeState);
        void Enter(int frameIndex, IState previousState);
        void ExecutePass0(int frameIndex);
        void ExecutePass1(int frameIndex);
        void Exit(int frameIndex);
    }
}