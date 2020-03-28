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

            public AnimatorRootMotion animatorRootMotion;

            public LayerMask groundRaycastMask;
            public float groundCheckDistance;
            public float groundCheckDistanceMinimum = 0.11f;

            public float landAnimationFrameTarget = 3f;

            public Transform standCollider;

            public TMPro.TextMeshPro __debugState;

            public int fgCharId { get; private set; }
            private int teamId;

            private FightingGameService fgService;

            private List<CharacterProperties.Attack> prevAttacks;

            private CharacterStates.CharacterBaseState prevState;

            private Vector3 target;

            private Input.InputBuffer inputBuffer;
            private List<Combination> inputDoNothingList;

            private FightingGameCharacter opponentChar;
            private GameTimeManager gameTimeManager;

            private List<Combination> inUseCombinations;

            private List<Action<int, int>> onHealthChangeCallbacks;
            private List<Action<FightingGameCharacter>> onEmptyHealthCallbacks;

            private new Rigidbody rigidbody;
            private CharacterPrioritizedVelocity charVelocity;
            private List<(Vector3, ForceMode)> forces;

            public Quaternion rotation {
                get { return rigidbody.rotation; }
            }

            public Vector3 position {
                get { return rigidbody.position; }
            }

            public int maxHealth {
                get { return charData.maxHealth; }
            }
            public int health { get; private set; }

            private Vector2 screenOrientation = Vector2.zero;

            private CharacterData charData;

            public float gameTime {
                get { return gameTimeManager.Layer("gameTime"); }
            }

            public float realTime {
                get { return gameTimeManager.Layer("realTime"); }
            }

            public void Init(CharacterData charData) {
                base.Init();
                this.fgCharId = fgCharCounter++;

                this.charData = charData;
                teamId = 0;

                health = charData.maxHealth;

                fgService = GameObject.FindGameObjectWithTag("rspService").GetComponent<FightingGameService>();

                prevAttacks = new List<CharacterProperties.Attack>();

                inputDoNothingList = new List<Combination> { ScriptableObject.CreateInstance<DirectionCurrent>().Init(0, Input.FightingGameAbsInputCodeDir.Neutral) };

                rigidbody = gameObject.GetComponent<Rigidbody>();
                charVelocity = new CharacterPrioritizedVelocity();
                forces = new List<(Vector3, ForceMode)>();

                inUseCombinations = new List<Combination>();

                onHealthChangeCallbacks = new List<Action<int, int>>();
                onEmptyHealthCallbacks = new List<Action<FightingGameCharacter>>();

                gameTimeManager = GameObject.FindGameObjectWithTag("rspTime").GetComponent<GameTimeManager>();
            }

            public FightingGameCharacter SetOpponentCharacter(FightingGameCharacter opponentChar) {
                this.opponentChar = opponentChar;
                target = opponentChar.position;
                return this;
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
                InputNotation notation = Utility.GameInputUtil.SelectInputNotation(button, direction);

                List<CharacterProperties.Attack> attackCandidates = charData.SelectAttacks(GetOrientation(), GetGroundRelation(), notation);
                CharacterProperties.Attack attack = charData.ChooseAttackFromSelectability(attackCandidates, currState, currAttack, prevAttacks);

                if (attack != null) {
                    CharacterStates.Attack attackState = (CharacterStates.Attack)states.Get("attack");

                    attackState.SetActiveAttack(attack);
                    SetState(attackState);

                    prevAttacks.Add(attack);
                }
            }

            public void ClearPrevAttacks() {
                prevAttacks.Clear();
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

            public Vector3 OpponentDirection() {
                return opponentChar.position - position;
            }

            public bool Grounded(out Vector3 groundPoint) {
                RaycastHit hitInfo;

                Debug.DrawLine(rigidbody.position + (Vector3.up * 0.1f), rigidbody.position + (Vector3.up * 0.1f) + (Vector3.down * groundCheckDistance), Color.yellow);

                if (Physics.Raycast(rigidbody.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, groundCheckDistance, groundRaycastMask, QueryTriggerInteraction.Ignore)) {
                    groundPoint = hitInfo.point;
                    return true;
                }
                else {
                    groundPoint = new Vector3(rigidbody.position.x, 0.0f, rigidbody.position.z);
                    return false;
                }
            }

            public bool CheckAboutToLand() {
                RaycastHit hitInfo;

                float checkDistance = Mathf.Max(landAnimationFrameTarget / 60.0f * -rigidbody.velocity.y, groundCheckDistanceMinimum);

                return checkDistance > 0.0f && Physics.Raycast(rigidbody.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, checkDistance + 0.1f, groundRaycastMask, QueryTriggerInteraction.Ignore);
            }

            public void AddRelativeVelocity(VelocityPriority priority, Vector3 velocity) {
                charVelocity.AddVelocity(priority, rotation * velocity);
            }

            public void AddForce(Vector3 force, ForceMode mode) {
                forces.Add((force, mode));
            }

            public void CalculateFinalVelocity() {
                rigidbody.velocity = charVelocity.CalculateVelocity(rigidbody.velocity);

                for (int n = 0; n < forces.Count; ++n) {
                    rigidbody.AddForce(forces[n].Item1, forces[n].Item2);
                }

                charVelocity.ClearVelocities();
                forces.Clear();
            }

            public void Rotate(Quaternion rotation) {
                rigidbody.MoveRotation(rotation * rigidbody.rotation);
            }
            
            public void SetRotation(Quaternion rotation) {
                rigidbody.MoveRotation(rotation);
            }

            public void SetRotation(Vector3 charDirection) {
                rigidbody.MoveRotation(Quaternion.Euler(0.0f, Vector3.SignedAngle(Vector3.right, charDirection, Vector3.up), 0.0f));
            }

            public void PushAway(float maxDistance, FightingGameCharacter otherChar) {
                float oneMinusDistance = maxDistance - Vector3.Distance(otherChar.transform.position, transform.position);
                Vector3 force = 1000.0f * oneMinusDistance * (otherChar.transform.position - transform.position).normalized;
                force.y = 0;

                //TODO: Decrease the force from the one actually doing the pushing.
                rigidbody.AddForce(-0.2f * force);
                otherChar.AddForce(0.8f * force);
            }

            public void AddForce(Vector3 force) {
                rigidbody.AddForce(force, ForceMode.Force);
            }

            public float LookToMoveAngle() {
                return Vector3.SignedAngle(rigidbody.transform.forward, opponentChar.transform.position - rigidbody.position, Vector3.up);
            }

            //TODO: Rename this function
            public void SetLocalMoveDirection(float x, float z) {
                animator.SetFloat("charX", x);
                animator.SetFloat("charZ", z);
            }

            public void SetState(CharacterStates.CharacterBaseState nextState) {
                prevState = (CharacterStates.CharacterBaseState) stateMachine.GetCurrentState();
                if (nextState.GetType() != typeof(CharacterStates.Attack) && nextState.GetType() != typeof(CharacterStates.AttackAirborne)) {
                    prevAttacks.Clear();
                }
                stateMachine.ChangeState(nextState);
            }

            public CharacterStates.CharacterBaseState State(string id) {
                return (CharacterStates.CharacterBaseState) states.Get(id);
            }

            public CharacterStates.CharacterBaseState GetPrevState() {
                return prevState;
            }

            public void AnimatorMoveCallback(Quaternion animatorRootRotation, Vector3 animatorVelocity) {
                CharacterStates.CharacterBaseState currState = (CharacterStates.CharacterBaseState) stateMachine.GetCurrentState();


            }

            public void UseCombination(Combination combo) {
                combo.inUse = true;
                inUseCombinations.Add(combo);
                inUseCombinations.Sort();
            }

            public List<Combination> GivenCombinations() {
                for (int n = 0; n < inUseCombinations.Count; ++n) {
                    inUseCombinations[n].inUse = false;
                }
                return inUseCombinations;
            }

            public Combination Given(params Type[] types) {
                Combination retVal = null;
                for (int n = 0; n < inUseCombinations.Count; ++n) {
                    if (Array.Exists(types, type => type == inUseCombinations[n].GetType())) {
                        retVal = inUseCombinations[n];
                    }
                    retVal.inUse = false;
                }
                inUseCombinations.Clear();
                return retVal;
            }

            public Vector3 GetLocalVelocity() {
                return Quaternion.Inverse(rigidbody.rotation) * rigidbody.velocity;
            }

            public Orientation GetOrientation() {
                //TODO: Determine actual orientation programmatically.
                return Orientation.SOUTHPAW;
            }

            public GroundRelation GetGroundRelation() {
                return ((CharacterStates.CharacterBaseState) stateMachine.GetCurrentState()).GetGroundRelation();
            }

            public void __debugSetStateText(string text, Color color) {
                if (screenOrientation.x > 0) {
                    __debugState.transform.rotation = rigidbody.rotation * Quaternion.Euler(0.0f, -90.0f, 0.0f);
                }
                else {
                    __debugState.transform.rotation = rigidbody.rotation * Quaternion.Euler(0.0f, 90.0f, 0.0f);
                }

                __debugState.text = text;
                __debugState.color = color == null ? Color.white : color;
            }

            public void Play(string animationState) {
                animator.Play(animationState, 0, this.gameTime);
            }

            public List<Input.Combinations.Combination> GetFoundCombinations() {
                    // TODO: Change the way the FGChar gets inputs over to the state machine.
                    //   If this is an NPC, it won't have an input buffer with it.
                //return inputBuffer?.GetFoundCombinations();

                if (inputBuffer != null) {
                    return inputBuffer.GetFoundCombinations();
                }
                else {
                    return inputDoNothingList;
                }
            }

            public override void GetHitBy(HitBox hitBox) {
                ((CharacterStates.CharacterBaseState) stateMachine.GetCurrentState()).GetHitBy(hitBox);
            }

            public override string HitBoxEventType(HitBox hitBox) {
                return "onHitFGChar";
            }

            public Vector3 GetTarget() {
                return target;
            }

            public void SetTarget(Vector3 newTargetPos) {
                target = newTargetPos;
            }

            public Transform GetOpponentTransform() {
                return opponentChar.transform;
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

            public void KnockBack(bool launch, Vector3 forceDirection, float forceMagnitude) {
                //AddRelativeVelocity
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