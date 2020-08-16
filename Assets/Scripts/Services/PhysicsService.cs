using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Gameplay;
using ResonantSpark.SimplifiedPhysics;

namespace ResonantSpark {
    namespace Service {
        public class PhysicsService : MonoBehaviour, IPhysicsService {
            public List<GlobalConstraint> constraints;

            public bool enableStep { set; get; }

            private PhysicsTracker physics;

            private GameTimeManager gameTime;

            public void Start() {
                Physics.autoSimulation = false;
                enableStep = true;

                physics = PhysicsTracker.Get();

                FrameEnforcer frame = GameObject.FindGameObjectWithTag("rspTime").GetComponent<FrameEnforcer>();
                frame.AddUpdate((int)FramePriority.PhysicsMovement, FramePhysicsSimulate);

                    // I'm going to use Unity Physics for this version. I'll create my own physics for the game in a future version.
                //frame.AddUpdate((int) FramePriority.PhysicsMovement, FrameUpdateMovement);
                //frame.AddUpdate((int) FramePriority.PhysicsCollisions, FrameUpdateCollisions);
                //frame.AddUpdate((int) FramePriority.PhysicsResolve, FrameUpdateResolve);

                    // Except for the 2 player characters, they're going to have special resolution code... oh well.
                frame.AddUpdate((int) FramePriority.PhysicsResolve, SpecialCharacterColliderCode);
                gameTime = GameObject.FindGameObjectWithTag("rspTime").GetComponent<GameTimeManager>();

                EventManager.TriggerEvent<Events.ServiceReady, Type>(typeof(PhysicsService));
            }

            public void OnDestroy() {
                Physics.autoSimulation = true;
                enableStep = false;
                physics.Clear();
            }

            public void FramePhysicsSimulate(int frameIndex) {
                if (enableStep) {
                    Physics.Simulate(gameTime.DeltaTime("frameDelta", "game"));
                }
            }

            public void FrameUpdateMovement(int frameIndex) {
                for (int n = 0; n < physics.rigidFGs.Count; ++n) {
                    physics.rigidFGs[n].FrameUpdateMovement(frameIndex, gameTime.DeltaTime("frameDelta", "game"));
                }
            }

            private void FrameUpdateCollisions(int frameIndex) {
                foreach (GlobalConstraint constraint in constraints) {
                    if (constraint.active) {
                        constraint.Preprocess(gameTime.DeltaTime("frameDelta", "game"));
                    }
                }

                for (int i = 0; i < physics.rigidFGs.Count; ++i) {
                    for (int j = i + 1; j < physics.rigidFGs.Count; ++j) {
                        var rigidFG0 = physics.rigidFGs[i];
                        var rigidFG1 = physics.rigidFGs[j];

                        Bounds bounds0 = rigidFG0.collider.bounds;
                        Bounds bounds1 = rigidFG1.collider.bounds;

                        if (bounds0.Intersects(bounds1)) {
                            // TODO: Find intersection;
                            SimplifiedPhysics.Collision.CapsuleCapsuleIntersection((CapsuleCollider)rigidFG0.collider, (CapsuleCollider)rigidFG1.collider);
                        }
                    }
                }

                for (int i = 0; i < physics.rigidFGs.Count; ++i) {
                    for (int j = 0; j < physics.stationaryColliders.Count; ++j) {
                        var rigidFG0 = physics.rigidFGs[i];
                        var collider1 = physics.stationaryColliders[j];

                        Bounds bounds0 = rigidFG0.collider.bounds;
                        Bounds bounds1 = collider1.collider.bounds;

                        if (bounds0.Intersects(bounds1)) {
                            // TODO: Find intersection
                        }
                    }
                }
            }

            private void FrameUpdateResolve(int frameIndex) {

                // Start Constrait Resolution
                // Create a graph of what objects affect other objects
                //      Start at the stationary objects and push through the graph less and less.

                // while (in graph looping) {
                    foreach (RigidbodyFG rigidFG in physics.rigidFGs) {
                        rigidFG.FrameUpdateCollisions(0); // Also passing in the Vector3 offset each time it moves.
                    }
                // }

                foreach (GlobalConstraint constraint in constraints) {
                    if (constraint.active) {
                        constraint.Postprocess(gameTime.DeltaTime("frameDelta", "game"));
                    }
                }
            }

            private void SpecialCharacterColliderCode(int frameIndex) {
                foreach (GlobalConstraint constraint in constraints) {
                    if (constraint.active) {
                        constraint.Preprocess(gameTime.DeltaTime("frameDelta", "game"));
                    }
                }

                foreach (GlobalConstraint constraint in constraints) {
                    if (constraint.active) {
                        constraint.Postprocess(gameTime.DeltaTime("frameDelta", "game"));
                    }
                }
            }

            public void Configure<T>(Action<int, T> callbackAsConstrait) {
                int n = 0;
                foreach (T constraintOfType in constraints.OfType<T>()) {
                    callbackAsConstrait(n, constraintOfType);
                    ++n;
                }
            }
        }
    }
}