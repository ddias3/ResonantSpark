
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using ResonantSpark.Input.Combinations;
using ResonantSpark.Character;
using ResonantSpark.Input;
using ResonantSpark.Service;
using ResonantSpark.Utility;

namespace ResonantSpark {
    namespace Gameplay {
        public class FightingGameCharacter : InGameEntity, IPredeterminedActions, IEquatable<FightingGameCharacter> {
            public static int fgCharCounter = 0;

            public AnimatorAdapter animator;
            public StateMachine stateMachine;
            public Utility.StateDict states;

            public AttackRunner attackRunner;

            public Orientation defaultForwardOrientation;

            public LayerMask groundRaycastMask;
            public float groundCheckDistance = 0.11f;
            public float groundCheckDistanceMinimum = 0.11f;

            public float landAnimationFrameTarget = 3f;

            public Transform standCollider;

            public TMPro.TextMeshPro __debugState;

            public int fgCharId { get; private set; }
            private int teamId;

            private FightingGameService fgService;

            private CharacterMovementAnimation charMovementAnimation;

            private CharacterStates.CharacterBaseState prevState;

            private Input.InputBuffer inputBuffer;
            private List<Combination> inputDoNothingList;

            private GameTimeManager gameTimeManager;

            private List<Combination> inUseCombinations;

            private List<Action<int, int>> onHealthChangeCallbacks;
            private List<Action<FightingGameCharacter>> onEmptyHealthCallbacks;

            private TargetFG targetFG;

            private bool controlEnable = false;

            public Quaternion toLocal {
                get { return Quaternion.Inverse(rigidFG.rotation); }
            }

            public Vector3 position {
                get { return rigidFG.position; }
                set { rigidFG.position = value; }
            }

            public int maxHealth {
                get { return charData.maxHealth; }
            }
            public int health { get; private set; }

            private Vector2 screenOrientation = Vector2.zero;

            private CharacterData charData;

            public float gameTime {
                get { return gameTimeManager.DeltaTime("frameDelta", "game"); }
            }

            public float realTime {
                get { return gameTimeManager.DeltaTime("realDelta"); }
            }

            public void Init(CharacterData charData) {
                base.Init();
                this.fgCharId = fgCharCounter++;

                this.charData = charData;
                teamId = 0;

                health = charData.maxHealth;

                fgService = GameObject.FindGameObjectWithTag("rspService").GetComponent<FightingGameService>();

                attackRunner.Init(this);

                inputDoNothingList = new List<Combination> { ScriptableObject.CreateInstance<DirectionCurrent>().Init(0, Input.FightingGameAbsInputCodeDir.Neutral) };

                targetFG = gameObject.GetComponentInChildren<TargetFG>();

                charMovementAnimation = new CharacterMovementAnimation(this);

                onHealthChangeCallbacks = new List<Action<int, int>>();
                onEmptyHealthCallbacks = new List<Action<FightingGameCharacter>>();

                gameTimeManager = GameObject.FindGameObjectWithTag("rspTime").GetComponent<GameTimeManager>();
            }

            public FightingGameCharacter SetInputBuffer(Input.InputBuffer inputBuffer) {
                this.inputBuffer = inputBuffer;
                return this;
            }

            public void RoundReset() {
                screenOrientation = fgService.ScreenOrientation(this);

                health = charData.maxHealth;

                stateMachine.Reset(); // TODO: Complete this functionality.
            }

            public void CalculateScreenOrientation() {
                Vector2 newScreenOrientation = fgService.ScreenOrientation(this);
                if (newScreenOrientation.x == 0) {
                    newScreenOrientation = screenOrientation;
                }
                screenOrientation = newScreenOrientation;
            }

            public void ChooseAttack(CharacterStates.CharacterBaseState currState, CharacterProperties.Attack currAttack, FightingGameInputCodeBut button, FightingGameInputCodeDir direction = FightingGameInputCodeDir.None) {
                attackRunner.ChooseAttack(charData, currState, currAttack, button, direction);
            }

            public void ClearPrevAttacks() {
                attackRunner.ClearPrevAttacks();
            }

            public void RunAttackFrame() {
                attackRunner.RunFrame();
            }

            public FightingGameInputCodeDir MapAbsoluteToRelative(FightingGameAbsInputCodeDir absInput) {
                //horizontal = ((((x - 1) mod 3) - 1) * orientation + 1);
                //vertical = floor((x - 1) / 3) * 3 + 1
                FightingGameInputCodeDir temp = (FightingGameInputCodeDir)(((int)absInput - 1) / 3 * 3 + 1 + (int)(((((int)absInput - 1) % 3) - 1) * screenOrientation.x + 1));
                //if (id == 0) Debug.Log((int) temp);
                return temp;
            }

            public FightingGameAbsInputCodeDir MapRelativeToAbsolute(FightingGameInputCodeDir relInput) {
                return (FightingGameAbsInputCodeDir)(int)MapAbsoluteToRelative((FightingGameAbsInputCodeDir)(int)relInput);
            }

            public Vector3 RelativeInputToLocal(FightingGameInputCodeDir relInput, bool upJump) {
                int forwardBackward = (((int)relInput) - 1) % 3 - 1;
                int upDown = (((int)relInput) - 1) / 3 - 1;

                Vector3 worldInput = new Vector3(forwardBackward, upDown, 0);
                if (!upJump) {
                    worldInput = Quaternion.Euler(90f, 0, 0) * worldInput;
                }

                // This sure is arbitrary... I'm not sure I have the vectors correctly set up so
                //   the part that mirrors across from side to side is the Z-axis???
                worldInput.z *= screenOrientation.x;

                //if (id == 0) Debug.Log((Quaternion.Euler(0.0f, -90.0f, 0.0f) * worldInput).ToString("F3"));

                return Quaternion.Euler(0.0f, -90.0f, 0.0f) * worldInput;
            }

            public bool Grounded(out Vector3 groundPoint) {
                RaycastHit hitInfo;

                Debug.DrawLine(rigidFG.position + (Vector3.up * 0.1f), rigidFG.position + (Vector3.up * 0.1f) + (Vector3.down * groundCheckDistance), Color.yellow);

                if (Physics.Raycast(rigidFG.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, groundCheckDistance, groundRaycastMask, QueryTriggerInteraction.Ignore)) {
                    groundPoint = hitInfo.point;
                    return true;
                }
                else {
                    groundPoint = new Vector3(rigidFG.position.x, 0.0f, rigidFG.position.z);
                    return false;
                }
            }

            public bool CheckAboutToLand() {
                RaycastHit hitInfo;

                float checkDistance = Mathf.Max(landAnimationFrameTarget / 60.0f * -rigidFG.velocity.y, groundCheckDistanceMinimum);

                return checkDistance > 0.0f && Physics.Raycast(rigidFG.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, checkDistance + 0.1f, groundRaycastMask, QueryTriggerInteraction.Ignore);
            }

            public void AddRelativeVelocity(VelocityPriority priority, Vector3 velocity) {
                rigidFG.AddRelativeVelocity(priority, velocity);
            }

            public void SetRelativeVelocity(VelocityPriority priority, Vector3 velocity) {
                rigidFG.SetRelativeVelocity(priority, velocity);
            }

            public void AddForce(Vector3 force, ForceMode mode) {
                rigidFG.AddForce(force, mode);
            }

            public void CalculateFinalVelocity() {
                rigidFG.CalculateFinalVelocity();
            }

            public void Rotate(Quaternion rotation) {
                rigidFG.Rotate(rotation);
            }
            
            public void SetRotation(Quaternion rotation) {
                rigidFG.SetRotation(rotation);
            }

            public void SetRotation(Vector3 charDirection) {
                rigidFG.SetRotation(charDirection);
            }

            public float LookToMoveAngle() {
                return Vector3.SignedAngle(rigidFG.transform.forward, targetFG.targetPos - rigidFG.position, Vector3.up);
            }

            public void SetLocalWalkParameters(float x, float z) {
                animator.SetFloat("charX", x);
                animator.SetFloat("charZ", z);
            }

            public void SetState(CharacterStates.CharacterBaseState nextState) {
                prevState = (CharacterStates.CharacterBaseState) stateMachine.GetCurrentState();
                if (nextState.GetType() != typeof(CharacterStates.AttackGrounded) && nextState.GetType() != typeof(CharacterStates.AttackAirborne)) {
                    attackRunner.ClearPrevAttacks();
                }
                stateMachine.ChangeState(nextState);
            }

            public CharacterStates.CharacterBaseState State(string id) {
                return (CharacterStates.CharacterBaseState) states.Get(id);
            }

            public CharacterStates.CharacterBaseState GetPrevState() {
                return prevState;
            }

            public Vector3 GetLocalVelocity() {
                return rigidFG.GetLocalVelocity();
            }

            public Orientation GetOrientation() {
                //TODO: Determine actual orientation programmatically.
                return defaultForwardOrientation;
            }

            public GroundRelation GetGroundRelation() {
                return ((CharacterStates.CharacterBaseState) stateMachine.GetCurrentState()).GetGroundRelation();
            }

            public void __debugSetStateText(string text, Color color) {
                if (screenOrientation.x > 0) {
                    __debugState.transform.rotation = rigidFG.rotation * Quaternion.Euler(0.0f, -90.0f, 0.0f);
                }
                else {
                    __debugState.transform.rotation = rigidFG.rotation * Quaternion.Euler(0.0f, 90.0f, 0.0f);
                }

                __debugState.text = text;
                __debugState.color = color == null ? Color.white : color;
            }

            public void Play(string animationState, float normalizedTime = 0.0f) {
                animator.Play(animationState, 0, normalizedTime);
            }

            public void PlayIfOther(string animationState, float normalizedTime = 0.0f) {
                AnimatorStateInfo currInfo = animator.GetCurrentAnimatorStateInfo(0, 0);

                if (!currInfo.IsName(animationState)) {
                    animator.Play(animationState, 0, normalizedTime);
                }
            }

            public List<Input.Combinations.Combination> GetFoundCombinations() {
                    // TODO: Change the way the FGChar gets inputs over to the state machine.
                    //   If this is an NPC, it won't have an input buffer with it.
                //return inputBuffer?.GetFoundCombinations();

                if (inputBuffer != null && controlEnable) {
                    return inputBuffer.GetFoundCombinations();
                }
                else {
                    return inputDoNothingList;
                }
            }

            public void Use(Combination combo) {
                inputBuffer?.Use(combo);
            }

            public void UseCombination<TCombo>(Action<Combination> callback) {
                inputBuffer?.ForEach((combo) => {
                    if (combo.GetType() == typeof(TCombo)) {
                        callback(combo);
                    }
                });
            }

            public void RemoveInUseCombinations(List<Combination> combos) {
                inputBuffer?.RemoveInUseCombinations(combos);
            }

            public List<Combination> GetInUseCombinations() {
                if (inputBuffer != null) {
                    return inputBuffer.GetInUseCombinations();
                }
                else {
                    return inputDoNothingList;
                }
            }

            public void GetHit(bool launch) {
                ((CharacterStates.CharacterBaseState) stateMachine.GetCurrentState()).GetHit(launch);
            }

            public override string HitBoxEventType(HitBox hitBox) {
                return "onHitFGChar";
            }

            public Vector3 GetTarget() {
                return targetFG.targetPos;
            }

            public void SetTarget(Vector3 newTargetPos) {
                targetFG.targetPos = newTargetPos;
            }

            public void SetOpponentTarget(InGameEntity opponentChar) {
                targetFG.SetNewTarget(opponentChar);
            }

            public Vector3 TargetDirection() {
                return targetFG.targetPos - rigidFG.position;
            }

            public void ChangeHealth(int amount) {
                health -= amount;

                for (int n = 0; n < onHealthChangeCallbacks.Count; ++n) {
                    onHealthChangeCallbacks[n].Invoke(amount, health);
                }

                if (health <= 0) {
                    for (int n = 0; n < onEmptyHealthCallbacks.Count; ++n) {
                        onEmptyHealthCallbacks[n].Invoke(this);
                    }
                }
            }

            public void KnockBack(AttackPriority attackPriority, bool launch, Vector3 knockbackDirection, float knockbackMagnitude) {

                //opponent.GetHit();

                VelocityPriority velPriority = VelocityPriority.Light;

                switch (attackPriority) {
                    case AttackPriority.LightAttack:
                        velPriority = VelocityPriority.Light;
                        break;
                    case AttackPriority.MediumAttack:
                        velPriority = VelocityPriority.Medium;
                        break;
                    case AttackPriority.HeavyAttack:
                        velPriority = VelocityPriority.Heavy;
                        break;
                }

                Vector3 finalVelocity = knockbackDirection.normalized * knockbackMagnitude;
                if (!launch) {
                    finalVelocity.y = 0;
                }

                AddRelativeVelocity(velPriority, finalVelocity);
                GetHit(launch);
            }

            public bool InHitStun() {
                return stateMachine.GetCurrentState().GetType() == typeof(CharacterStates.HitStunStand)
                    || stateMachine.GetCurrentState().GetType() == typeof(CharacterStates.HitStunCrouch)
                    || stateMachine.GetCurrentState().GetType() == typeof(CharacterStates.HitStunAirborne);
            }

            public void SetStandCollider(Vector3 standColliderOffset) {
                standCollider.localPosition = standColliderOffset;
            }

            public Vector3 GetSpeakPosition() {
                    // TODO: Return the position of the head;
                return Vector3.zero;
            }

            public void PredeterminedActions(string actionName) {
                CharacterStates.Clash clash = (CharacterStates.Clash) states.Get("clash");
                switch (actionName) {
                    case "verticalClash":
                        clash.SetClashAnimation("clash_vertical");
                        SetState(clash);
                        break;
                    case "horizontalClashSwingFromLeft":
                        clash.SetClashAnimation("clash_horizontalFromLeft");
                        SetState(clash);
                        break;
                    case "horizontalClashSwingFromRight":
                        clash.SetClashAnimation("clash_horizontalFromRight");
                        SetState(clash);
                        break;
                }
            }

            public void UpdateCharacterMovement() {
                charMovementAnimation.Increment();
            }

            public void UpdateTarget() {
                targetFG.UpdateTarget();
            }

            public void AnimationCrouch() {
                if (prevState == states.Get("stand")) {
                    charMovementAnimation.FromStand();
                }
                else {
                    charMovementAnimation.Crouch();
                }
            }

            public void AnimationStand() {
                if (prevState == states.Get("crouch")) {
                    charMovementAnimation.FromCrouch();
                }
                else {
                    charMovementAnimation.Stand();
                }
            }

            public void AnimationWalkVelocity() {
                charMovementAnimation.WalkVelocity(GetLocalVelocity());
            }

            public void SetControlEnable(bool controlEnable) {
                this.controlEnable = controlEnable;
            }

            public bool GetControlEnable() {
                return this.controlEnable;
            }

            public override void AddSelf() {
                throw new NotImplementedException();
            }

            public override void RemoveSelf() {
                throw new NotImplementedException();
            }

            public void RegisterOnHealthChangeCallback(Action<int, int> callback) {
                onHealthChangeCallbacks.Add(callback);
            }

            public void RegisterOnEmptyHealthCallback(Action<FightingGameCharacter> callback) {
                onEmptyHealthCallbacks.Add(callback);
            }

            public bool Equals(FightingGameCharacter other) {
                return fgCharId == other.fgCharId;
            }

            public override int GetHashCode() {
                return fgCharId;
            }
        }
    }
}