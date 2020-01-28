using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Gameplay;
using ResonantSpark.Service;

namespace ResonantSpark {
    namespace Gamemode {
        public interface IGamemode {
            void SetUp(PlayerService playerService, FightingGameService fgService, UiService uiService);
            int GetMaxPlayers();
        }
    }
}