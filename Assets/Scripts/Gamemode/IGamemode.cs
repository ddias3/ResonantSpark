using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Gameplay;
using ResonantSpark.Service;

namespace ResonantSpark {
    namespace Gamemode {
        public interface IGamemode {
            void CreateDependencies(AllServices services);
            void SetUp();
            bool IsCurrentFGChar(InGameEntity entity);
            int GetMaxPlayers();

            void OnGameEntityNumHitsChange(InGameEntity entity, int numHits, int prevNumHits);
            void OnHitStunEnd(FightingGameCharacter fgChar);
            void OnHitStunStart(FightingGameCharacter fgChar);

            void SetGameTimeScaling(float scaling);
            float GetGameTimeScaling();

            void CameraEnable(bool enable);
            Vector2 ScreenOrientation(FightingGameCharacter fgChar);
        }
    }
}