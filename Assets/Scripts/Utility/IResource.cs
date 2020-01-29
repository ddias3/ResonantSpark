using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Utility {
        public interface IResource {
            bool IsActive();
            void Activate();
            void Deactivate();
        }
    }
}