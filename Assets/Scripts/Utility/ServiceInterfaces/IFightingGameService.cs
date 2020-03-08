using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Gameplay;
using ResonantSpark.Character;

namespace ResonantSpark {
    namespace Service {
        public interface IFightingGameService {
            void CreateGamemode();
            void SetUpGamemode();
            Transform GetSpawnPoint();
            float GetSpawnPointOffset();
            void ResetCamera();
            Transform GetCameraStart();
            List<Transform> GetLevelBoundaries();
            void Hit(InGameEntity hitEntity, HitBox hitBox, Action<AttackPriority> callback);
        }
    }
}