using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.SimplifiedPhysics;

namespace ResonantSpark {
    namespace Gameplay {
        [RequireComponent(typeof(Rigidbody))]
        public class RigidbodyFG : MonoBehaviour {
            public Vector3 gravity = new Vector3(0.0f, -9.81f, 0.0f);
            public Collider collider;
            public List<Constraint> constraints;

            private CharacterPrioritizedVelocity charVelocity;
            private List<(Vector3, ForceMode)> forces;

            private new Rigidbody rigidbody;
            private GameTimeManager gameTime;

            private SimplifiedPhysics.State currState;

            private Func<Rigidbody, SimplifiedPhysics.State> directControlCallback;

            public void Awake() {
                charVelocity = new CharacterPrioritizedVelocity();
                forces = new List<(Vector3, ForceMode)>();

                rigidbody = GetComponent<Rigidbody>();
                //rigidbody.isKinematic = true;
                rigidbody.constraints = RigidbodyConstraints.FreezeRotation;

                currState = new SimplifiedPhysics.State() {
                    x = rigidbody.position,
                    v = rigidbody.velocity,
                };

                SimplifiedPhysics.PhysicsTracker.Get().Add(this);

                gameTime = GameObject.FindGameObjectWithTag("rspTime").GetComponent<GameTimeManager>();
            }

            public Quaternion rotation {
                get { return rigidbody.rotation; }
            }

            public Vector3 velocity {
                get {
                    return rigidbody.velocity;
                    //return currState.v;   // Use this line if you're going to use custom physics
                }
                set {
                    currState.v = value;

                    rigidbody.velocity = currState.v;
                }
            }

            public Quaternion toLocal {
                get { return Quaternion.Inverse(rigidbody.rotation); }
            }

            public Vector3 position {
                get {
                    return rigidbody.position;
                    //return currState.x;   // Use this line if you're going to use custom physics
                }
                set {
                    currState.x = value;

                    rigidbody.position = currState.x;
                    transform.position = currState.x;
                }
            }

            public void CalculateFinalVelocity() {
                Debug.Log(rigidbody.velocity.ToString("F3"));
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

            public Vector3 GetLocalVelocity() {
                return Quaternion.Inverse(rigidbody.rotation) * rigidbody.velocity;
            }

            public void AddRelativeVelocity(VelocityPriority priority, Vector3 velocity) {
                charVelocity.AddVelocity(priority, rigidbody.rotation * velocity);
            }

            public void AddVelocity(VelocityPriority priority, Vector3 velocity) {
                charVelocity.AddVelocity(priority, velocity);
            }

            public void SetRelativeVelocity(VelocityPriority priority, Vector3 velocity) {
                charVelocity.SetVelocity(priority, rigidbody.rotation * velocity);
            }

            public void SetVelocity(VelocityPriority priority, Vector3 velocity) {
                charVelocity.SetVelocity(priority, velocity);
            }

            public void AddForce(Vector3 force, ForceMode mode) {
                forces.Add((force, mode));
            }

            public void SetDirectControlCallback(Func<Rigidbody, SimplifiedPhysics.State> callback) {
                this.directControlCallback = callback;
            }

            public void FrameUpdateMovement(int frameIndex) {
                if (directControlCallback != null) {
                    currState = directControlCallback(rigidbody);
                }
                else {
                    currState = SimplifiedPhysics.NumericalIntegration.RK4(Acceleration, currState, 0.0f, gameTime.DeltaTime("frameDelta", "game"));
                }

                rigidbody.MovePosition(currState.x);
            }

            public void FrameUpdateCollisions(int frameIndex) {
                
            }

            public void FrameUpdateResolve(int frameIndex) {
            }

            private Vector3 Acceleration(SimplifiedPhysics.State state) {
                return Vector3.up;// return gravity;
            }
        }
    }
}