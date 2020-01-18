using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using ResonantSpark.Input.Combinations;
using ResonantSpark.Character;

namespace ResonantSpark {
    namespace Gameplay {
        public class FightingGameCharacter : MonoBehaviour, IFightingGameCharacter, IEquatable<FightingGameCharacter> {
            public static int fgCharCounter = 0;

            public Animator animator;
            public StateMachine stateMachine;

            public Text charVelocity;

            public LayerMask groundRaycastMask;
            public float groundCheckDistance;
            public float groundCheckDistanceMinimum = 0.11f;

            public float landAnimationFrameTarget = 3f;

            public int id { get; private set; }
            private int teamId;

            private Input.InputBuffer inputBuffer;

            private GameObject opponentChar;
            private GameTimeManager gameTimeManager;

            private List<Combination> inUseCombinations;

            public new Rigidbody rigidbody { get; private set; }

            private bool facingRight = true;

            private CharacterData charData;

            public void Init() {
                this.id = fgCharCounter++;

                teamId = 0;

                rigidbody = gameObject.GetComponent<Rigidbody>();
                inUseCombinations = new List<Combination>();

                gameTimeManager = GameObject.FindGameObjectWithTag("rspTime").GetComponent<GameTimeManager>();
            }

            public FightingGameCharacter SetOpponentCharacter(GameObject opponentChar) {
                this.opponentChar = opponentChar;
                return this;
            }

            public FightingGameCharacter SetInputBuffer(Input.InputBuffer inputBuffer) {
                this.inputBuffer = inputBuffer;
                return this;
            }

            public float gameTime {
                get { return gameTimeManager.Layer("gameTime"); }
            }

            public float realTime {
                get { return gameTimeManager.Layer("realTime"); }
            }

            public void SetDirectionFacing(bool right) {
                this.facingRight = right;
            }

            public void FixedUpdate() {
                //try {
                //    charVelocity.text = "Vel = " + (Quaternion.Inverse(rigidbody.rotation) * rigidbody.velocity).ToString("F3");
                //}
                //catch (System.NullReferenceException) {
                //    // do nothing
                //}
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

            //protected void StickToGroundHelper(float downwardDistance) {
            //    RaycastHit hitInfo;
            //    if (Physics.SphereCast(rigidbody.position + player.legsCollider.center + (Vector3.up * (0.02f - 0.5f * player.legsCollider.height + player.legsCollider.radius)),
            //            player.legsCollider.radius,
            //            Vector3.down,
            //            out hitInfo,
            //            downwardDistance,
            //            player.raycastMask,
            //            QueryTriggerInteraction.Ignore)) {
            //        if (Vector3.Angle(hitInfo.normal, Vector3.up) < maxGroundedMoveAngle) {
            //            if (rigidbody.velocity.y < 0f) {
            //                rigidbody.velocity = Vector3.ProjectOnPlane(rigidbody.velocity, hitInfo.normal) + Vector3.up * rigidbody.velocity.y;
            //            }
            //            else {
            //                rigidbody.velocity = Vector3.ProjectOnPlane(rigidbody.velocity, hitInfo.normal);
            //            }
            //        }
            //        // TODO: Fix the code for slopes that are too steep.
            //        //else {
            //        //    Debug.Log("Push Away on slope angle = " + Vector3.Angle(hitInfo.normal, Vector3.up));
            //        //    //Vector3 slidingDownForce = Vector3.ProjectOnPlane(hitInfo.normal, Vector3.up) - Vector3.Project(hitInfo.normal, Vector3.up);
            //        //    Vector3 slidingDownForce = Quaternion.AngleAxis(90f, Vector3.Cross(hitInfo.normal, Vector3.ProjectOnPlane(hitInfo.normal, Vector3.up))) * hitInfo.normal;
            //        //    Debug.DrawLine(rigidbody.position, rigidbody.position + hitInfo.normal, Color.white);
            //        //    Debug.DrawLine(rigidbody.position, rigidbody.position + slidingDownForce, Color.black);
            //        //    rigidbody.AddForce(slidingDownForce * 10f);
            //        //}
            //    }
            //}

            public float LookToMoveAngle() {
                return Vector3.SignedAngle(rigidbody.transform.forward, opponentChar.transform.position - rigidbody.position, Vector3.up);
            }

            //TODO: Rename this function
            public void SetLocalMoveDirection(float x, float z) {
                animator.SetFloat("charX", x);
                animator.SetFloat("charZ", z);
            }

            public Vector3 CameraToChar(Vector3 input) {
                if (facingRight) {
                    return Quaternion.Euler(0.0f, -90.0f, 0.0f) * input;
                }
                else {
                    return Quaternion.Euler(0.0f, 90.0f, 0.0f) * input;
                }
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
                return Orientation.REGULAR;
            }

            public GroundRelation GetGroundRelation() {
                // TODO: Figure out how to get this value into FGChar
                return GroundRelation.STAND;
            }

            public void Play(string animationState) {
                animator.Play(animationState);
            }

            public void Play(string animationState, int layer, float normalizedTime) {
                animator.Play(animationState, layer, normalizedTime);
            }

            public List<Input.Combinations.Combination> GetFoundCombinations() {
                    // TODO: Change the way the FGChar gets inputs over to the state machine.
                    //   If this is an NPC, it won't have an input buffer with it.
                return inputBuffer?.GetFoundCombinations();
            }

            public void Hit() {
                // TODO: hit the opponent character
            }

            public bool Equals(FightingGameCharacter other) {
                return id == other.id;
            }

            public override int GetHashCode() {
                return id;
            }
        }
    }
}