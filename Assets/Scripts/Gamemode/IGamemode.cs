using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Gameplay;

namespace ResonantSpark {
    namespace Gamemode {
        public interface IGamemode {
            void SetFightingGameCharacter(FightingGameCharacter fgChar, int index);
            int GetMaxPlayers();
        }
    }
}