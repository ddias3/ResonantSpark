using UnityEngine;

namespace ResonantSpark {
    namespace Service {
        public interface IProjectileService {
                // Returns an ID for the projectile
            int RegisterProjectile(GameObject prefab, int instantiateNum = 4);
            void FireProjectile(int id);
        }
    }
}