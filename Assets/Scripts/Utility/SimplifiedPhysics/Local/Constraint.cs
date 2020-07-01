using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace SimplifiedPhysics {
        public abstract class Constraint : MonoBehaviour {
            public bool active { get; set; }

            public abstract void Restrict(Rigidbody rigidbody);
        }
    }
}