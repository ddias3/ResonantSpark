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
            Light,
            Medium,
            Heavy,
            Special,
            Projectile,
            Beam,
            Super,
        };

        public class CharacterPrioritizedVelocity {
            private Dictionary<VelocityPriority, List<Vector3>> inputs;

            public CharacterPrioritizedVelocity() {
                inputs = new Dictionary<VelocityPriority, List<Vector3>>();

                foreach (VelocityPriority priority in Enum.GetValues(typeof(VelocityPriority))) {
                    inputs.Add(priority, new List<Vector3>());
                }
            }

            public void AddVelocity(VelocityPriority priority, Vector3 velocity) {
                inputs[priority].Add(velocity);
            }

            public void ClearVelocities() {
                foreach (VelocityPriority priority in Enum.GetValues(typeof(VelocityPriority))) {
                    inputs[priority].Clear();
                }
            }

            public Vector3 CalculateVelocity(Vector3 currVel) {
                VelocityPriority highestPriority = VelocityPriority.None;
                List<Vector3> currList = inputs[highestPriority];

                foreach (VelocityPriority priority in Enum.GetValues(typeof(VelocityPriority))) {
                    if (priority > highestPriority && inputs[priority].Count > 0) {
                        highestPriority = priority;
                        currList = inputs[priority];
                    }
                }

                if (highestPriority == VelocityPriority.None) {
                    return currVel;
                }

                Vector3 finalVelocity = currList.Aggregate(Vector3.zero, (aggregate, curr) => {
                    return aggregate + curr;
                });

                return finalVelocity;
            }
        }
    }
}