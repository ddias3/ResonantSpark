using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Gameplay;

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
        }
    }
}