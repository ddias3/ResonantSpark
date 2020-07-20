using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Gameplay;
using ResonantSpark.SimplifiedPhysics;

namespace ResonantSpark {
    namespace Service {
        public class PhysicsService : MonoBehaviour {
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

                //frame.AddUpdate((int) FramePriority.PhysicsMovement, FrameUpdateMovement);
                //frame.AddUpdate((int) FramePriority.PhysicsCollisions, FrameUpdateCollisions);
                frame.AddUpdate((int) FramePriority.PhysicsResolve, FrameUpdateResolve);
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
                    physics.rigidFGs[n].FrameUpdateMovement(frameIndex);
                }
            }

            public void FrameUpdateCollisions(int frameIndex) {
                for (int i = 0; i < physics.rigidFGs.Count; ++i) {
                    for (int j = i + 1; j < physics.rigidFGs.Count; ++j) {
                        var rigidFG0 = physics.rigidFGs[i];
                        var rigidFG1 = physics.rigidFGs[j];

                        Bounds bounds0 = rigidFG0.collider.bounds;
                        Bounds bounds1 = rigidFG1.collider.bounds;

                        if (bounds0.Intersects(bounds1)) {
                            // TODO: Find intersection;                            
                        }
                    }
                }

                for (int i = 0; i < physics.rigidFGs.Count; ++i) {
                    for (int j = 0; j < physics.stationaryColliders.Count; ++j) {
                        var rigidFG0 = physics.rigidFGs[i];
                        var collider1 = physics.stationaryColliders[j];

                        Bounds bounds0 = rigidFG0.collider.bounds;
                        Bounds bounds1 = collider1.collider.bounds;
                    }
                }
            }

            public void FrameUpdateResolve(int frameIndex) {
                
            }
        }
    }
}