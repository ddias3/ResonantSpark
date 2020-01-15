using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Gameplay;

namespace ResonantSpark {
    namespace Input {
        public interface ICharacterControlSystem {
            // TODO: Maybe make an interface for both human controls, dummy, and AI
            //   this interface would only be for setting up connections between code

            void ConnectToCharacter(FightingGameCharacter fgChar);
        }
    }
}
