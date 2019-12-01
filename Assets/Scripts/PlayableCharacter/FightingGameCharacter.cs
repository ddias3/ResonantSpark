using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ResonantSpark {
    namespace Gameplay {
        public class FightingGameCharacter : MonoBehaviour {

            public Animator animator;
            public StateMachine stateMachine;

            public Text charVelocity;

            private Input.InputBuffer inputBuffer;

            private GameObject opponentChar;
            private GameTimeManager gameTimeManager;
            public new Rigidbody rigidbody { get; private set; }

            private bool facingRight;

            public FightingGameCharacter SetOpponentCharacter(GameObject opponentChar) {
                this.opponentChar = opponentChar;
                return this;
            }

            public FightingGameCharacter SetGameTimeManager(GameTimeManager gameTimeManager) {
                this.gameTimeManager = gameTimeManager;
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

            public void Start() {
                rigidbody = gameObject.GetComponent<Rigidbody>();
                SetDirectionFacing(true);
            }

            public void FixedUpdate() {
                charVelocity.text = "Vel = " + (Quaternion.Inverse(rigidbody.rotation) * rigidbody.velocity).ToString("F3");
            }

            public bool Grounded() {
                //TODO: Create state for whether character is grounded or not
                return true;
            }

            public float LookToMoveAngle() {
                return Vector3.SignedAngle(rigidbody.transform.forward, opponentChar.transform.position - rigidbody.position, Vector3.up);
            }

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

            public void Play(string animationState) {
                animator.Play(animationState);
            }

            public void Play(string animationState, int layer, float normalizedTime) {
                animator.Play(animationState, layer, normalizedTime);
            }

            public List<Input.Combinations.Combination> GetFoundCombinations() {
                // TODO: look through them if necessary.
                return inputBuffer.GetFoundCombinations();
            }
        }
    }
}