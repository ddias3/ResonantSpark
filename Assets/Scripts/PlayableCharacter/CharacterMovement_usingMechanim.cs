using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterMovement_usingMechanim : MonoBehaviour {

    public enum CharacterMovementEnum : int {
        Idle = 0,
        StepBackward,
        StepForward,
        StepChest,
        StepSpine,
        DashBackward,
        DashForward,
        SideStepChest,
        SideStepSpine
    };

    [System.Serializable]
    public struct CharMoveStruct {
        public string moveName;
        public AnimationCurve movementCurve;
        public Vector3 localMovement;
    };

    private Animator animator;
    private StateCallback[] stateCallbacks;

    private bool inAction = false;
    private bool actionCancellable = true;
    private Vector3 initialPos;

    public CharMoveStruct[] movementSet;

    // Start is called before the first frame update
    void Start() {
        animator = GetComponent<Animator>();
        stateCallbacks = animator.GetBehaviours<StateCallback>();

        foreach (StateCallback stateCallback in stateCallbacks) {
            stateCallback.SetCallback(AnimationEnter, AnimationComplete);
        }

        initialPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
    }

    // Update is called once per frame
    void Update() {
        //if (anim.GetCurrentAnimatorStateInfo(0).IsName("idle")) {
        //    actionCancellable = true;
        //    inAction = false;
        //}

        //transform.position = Vector3.Lerp(initialPos, initialPos + new Vector3(0.3f, 0f, 0f), stepBackward.Evaluate(animator.GetCurrentAnimatorStateInfo(0).normalizedTime));

        int test0 = animator.GetCurrentAnimatorStateInfo(0).shortNameHash;

        //transform.localPosition = Vector3.Lerp(Vector3.zero, transform.localRotation * movementSet[actionIndex].localMovement, movementSet[actionIndex].movementCurve.Evaluate(animator.GetCurrentAnimatorStateInfo(0).normalizedTime));
    }

    public void CancellableAnimation() {
        Debug.Log("Action Cancellable");

        actionCancellable = true;
        animator.SetInteger("nextState", (int) CharacterMovementEnum.Idle);
    }

    public void AnimationEnter(AnimatorStateInfo stateInfo) {
        Debug.Log("Animation Enter");

        inAction = true;
        actionCancellable = false;
    }

    public void AnimationComplete(AnimatorStateInfo stateInfo) {
        Debug.Log("Animation Complete");
        inAction = false;
        actionCancellable = true;
    }

    public bool MoveAction(CharacterMovementEnum charMove) {
        if (actionCancellable) {
            Debug.Log("Move Action");

            actionCancellable = false;
            inAction = true;

            animator.SetInteger("nextState", (int) charMove);

            switch (charMove) {
                case CharacterMovementEnum.StepBackward:
                    animator.Play("stepBackward");
                    //anim.SetTrigger("stepBackward");
                    break;
                case CharacterMovementEnum.StepForward:
                    animator.Play("stepForward");
                    //anim.SetTrigger("stepForward");
                    break;
                case CharacterMovementEnum.StepChest:
                    animator.Play("stepChest");
                    //anim.SetTrigger("stepChest");
                    break;
                case CharacterMovementEnum.StepSpine:
                    animator.Play("stepSpine");
                    break;
                case CharacterMovementEnum.DashBackward:
                    animator.Play("dashBackward");
                    break;
                case CharacterMovementEnum.DashForward:
                    animator.Play("dashForward");
                    break;
                case CharacterMovementEnum.SideStepChest:
                    animator.Play("sideStepChest");
                    break;
                case CharacterMovementEnum.SideStepSpine:
                    animator.Play("sideStepSpine");
                    break;
            }

            return true;
        }
        return false;
    }
}
