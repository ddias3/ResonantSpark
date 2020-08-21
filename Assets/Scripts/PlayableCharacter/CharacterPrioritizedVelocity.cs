using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Gameplay {
        public enum VelocityPriority : int {
            None = 0,
            Movement,
            Dash,
            Jump,
            MovementOverride,
            AttackMovement,
            Light,
            Medium,
            Heavy,
            Special,
            Projectile,
            Beam,
            Super,
            BoundOverride,
        };

        public class CharacterPrioritizedVelocity {
            private Dictionary<VelocityPriority, List<Vector3>> inputsAdditive;
            private Dictionary<VelocityPriority, Vector3> inputSetVelocity;
            private Dictionary<VelocityPriority, bool> setVelocityIsSet;

            public CharacterPrioritizedVelocity() {
                inputsAdditive = new Dictionary<VelocityPriority, List<Vector3>>();
                inputSetVelocity = new Dictionary<VelocityPriority, Vector3>();
                setVelocityIsSet = new Dictionary<VelocityPriority, bool>();

                foreach (VelocityPriority priority in Enum.GetValues(typeof(VelocityPriority))) {
                    inputsAdditive.Add(priority, new List<Vector3>());
                    inputSetVelocity.Add(priority, Vector3.zero);
                    setVelocityIsSet.Add(priority, false);
                }
            }

            public void AddVelocity(VelocityPriority priority, Vector3 velocity) {
                inputsAdditive[priority].Add(velocity);
            }

            public void ClearAdditiveVelocities(VelocityPriority priority) {
                inputsAdditive[priority].Clear();
            }

            public void SetVelocity(VelocityPriority priority, Vector3 velocity) {
                inputSetVelocity[priority] = velocity;
                setVelocityIsSet[priority] = true;
            }

            public void ClearVelocities() {
                foreach (VelocityPriority priority in Enum.GetValues(typeof(VelocityPriority))) {
                    inputsAdditive[priority].Clear();
                    setVelocityIsSet[priority] = false;
                }
            }

            public Vector3 CalculateVelocity() {
                return CalculateVelocity(Vector3.zero);
            }

            public Vector3 CalculateVelocity(Vector3 currVel) {
                VelocityPriority highestPriority = VelocityPriority.None;
                List<Vector3> currList = inputsAdditive[highestPriority];

                foreach (VelocityPriority priority in Enum.GetValues(typeof(VelocityPriority))) {
                    if (priority > highestPriority && inputsAdditive[priority].Count > 0 || setVelocityIsSet[priority]) {
                        highestPriority = priority;
                        currList = inputsAdditive[priority];
                    }
                }

                if (highestPriority == VelocityPriority.None) {
                    return currVel;
                }

                //Vector3 finalVelocity = currList.Aggregate(Vector3.zero, (aggregate, curr) => {
                //    return aggregate + curr;
                //});

                Vector3 finalVelocity;
                if (setVelocityIsSet[highestPriority]) {
                    finalVelocity = inputSetVelocity[highestPriority];
                }
                else {
                    finalVelocity = currVel;
                }
                foreach (Vector3 vel in currList) {
                    finalVelocity += vel;
                }

                return finalVelocity;
            }
        }
    }
}