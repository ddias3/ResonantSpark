using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour {

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
        public int cancellableFrame;
        public int[] realignFrames;
    };

    public CharMoveStruct[] movementSet;
    public AnimationClip[] animations;

    private Animator animator;

    private bool inAction = false;
    private bool actionCancellable = true;

    private int currentState = 0;

    // Start is called before the first frame update
    void Start() {
        animator = gameObject.GetComponentInChildren<Animator>() as Animator;
        animations = animator.runtimeAnimatorController.animationClips;

            // TODO: Sort the animation clips to guarantee their order.
        //Debug.Log(animations[0].frameRate);
        //Debug.Log(animations[0].name);
        //Array.Sort<AnimationClip>(animations)
    }

    public float elapsedTime = 0f;
    public int elapsedFrame = 0;
    public Vector3 startPosition;
    public Quaternion startDirectionForward;
    public Vector3 endPosition;
    public GameObject target;

    void Update() {
        if (inAction) {
            elapsedFrame = (int) Mathf.Floor(60f * elapsedTime);

            transform.position = startPosition +
                movementSet[currentState].movementCurve.Evaluate(elapsedTime / animations[currentState].length) *
                    (startDirectionForward * movementSet[currentState].localMovement);

            //transform.position = Vector3.Lerp(startPosition, endPosition, movementSet[currentState].movementCurve.Evaluate(elapsedTime / animations[currentState].length));

            if (!actionCancellable && elapsedFrame >= movementSet[currentState].cancellableFrame) {
                Debug.Log("Action Cancellable");
                actionCancellable = true;
            }

            if (movementSet[currentState].realignFrames.Length > 0) {
                if (Array.Exists<int>(movementSet[currentState].realignFrames, elem => elem == elapsedFrame)) {
                    transform.LookAt(target.transform);
                }
            }
            else {
                //transform.LookAt(target.transform);
            }

            if (elapsedTime > animations[currentState].length) {
                inAction = false;
                currentState = (int) CharacterMovementEnum.Idle;
                animator.Play("idle");
            }

            elapsedTime += Time.deltaTime;
        }
    }

    public bool MoveAction(CharacterMovementEnum charMove, GameObject target) {
        if (actionCancellable) {
            Debug.Log("Move Action");

            actionCancellable = false;
            inAction = true;

            currentState = (int) charMove;

            elapsedTime = 0f;
            elapsedFrame = 0;

            startPosition = transform.position;
            startDirectionForward = transform.rotation;

            endPosition = startPosition + startDirectionForward * movementSet[currentState].localMovement;
            endPosition = target.transform.position + Vector3.Distance(startPosition, target.transform.position) * (endPosition - target.transform.position).normalized;

            this.target = target;

            switch (charMove) {
                case CharacterMovementEnum.StepBackward:
                    animator.Play("stepBackward", 0, 0f);
                    //anim.SetTrigger("stepBackward");
                    break;
                case CharacterMovementEnum.StepForward:
                    animator.Play("stepForward", 0, 0f);
                    //anim.SetTrigger("stepForward");
                    break;
                case CharacterMovementEnum.StepChest:
                    animator.Play("stepChest", 0, 0f);
                    //anim.SetTrigger("stepChest");
                    break;
                case CharacterMovementEnum.StepSpine:
                    animator.Play("stepSpine", 0, 0f);
                    break;
                case CharacterMovementEnum.DashBackward:
                    animator.Play("dashBackward", 0, 0f);
                    break;
                case CharacterMovementEnum.DashForward:
                    animator.Play("dashForward", 0, 0f);
                    break;
                case CharacterMovementEnum.SideStepChest:
                    animator.Play("sideStepChest", 0, 0f);
                    break;
                case CharacterMovementEnum.SideStepSpine:
                    animator.Play("sideStepSpine", 0, 0f);
                    break;
            }
            return true;
        }
        return false;
    }

    public void FaceCharacter(GameObject otherChar) {
        transform.LookAt(otherChar.transform.position);
    }
}
