using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Gameplay;
using ResonantSpark.Character;

namespace ResonantSpark {
    namespace Service {
        public interface IFightingGameService {
            void RegisterInGameEntity(InGameEntity entity);
            void RemoveInGameEntity(InGameEntity entity);
            void CreateGamemode();
            void SetUpGamemode();
            Transform GetSpawnPoint();
            float GetSpawnPointOffset();
            void ResetCamera();
            Transform GetCameraStart();
            List<Transform> GetLevelBoundaries();
            void Hit(InGameEntity hitEntity, InGameEntity byEntity, Hit hit, Action<AttackPriority, int> onHit, Action<AttackPriority, int> onBlock);
            void HitStunStart(FightingGameCharacter fgChar);
            void HitStunEnd(FightingGameCharacter fgChar);
            bool IsCurrentFGChar(InGameEntity entity);
            void DisableControl();
        }
    }
}