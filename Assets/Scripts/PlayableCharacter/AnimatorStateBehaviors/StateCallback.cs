using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateCallback : StateMachineBehaviour {

    private Action<AnimatorStateInfo> enterCb;
    private Action<AnimatorStateInfo> exitCb;

    public void SetCallback(Action<AnimatorStateInfo> enterCb, Action<AnimatorStateInfo> exitCb) {
        this.enterCb = enterCb;
        this.exitCb = exitCb;
    }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        enterCb?.Invoke(stateInfo);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        base.OnStateExit(animator, stateInfo, layerIndex);
        exitCb?.Invoke(stateInfo);
    }
}
