using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Gameplay;
using ResonantSpark.Service;

namespace ResonantSpark {
    namespace Gamemode {
        public interface IGamemode {
            void SetFightingGameCharacter(FightingGameCharacter fgChar, int index);
            void SetUp(PlayerService playerService, FightingGameService fgService);
            int GetMaxPlayers();
        }
    }
}