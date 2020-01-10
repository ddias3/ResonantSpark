using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Gameplay {
        public class FightingGameService : MonoBehaviour {

            public Vector3 underLevel = new Vector3(0, -100, 0);
            public Transform projectileEmpty;
            public Transform hitBoxEmpty;

            private static int projectileCounter = 0;

            private FrameEnforcer frame;

            private Dictionary<int, GameObject> projectilePrefabs;
            private Dictionary<int, List<Projectile>> preInstantiatedProjectiles;
            private Dictionary<int, int> preInstantiatedProjectilesIndex;

            private List<Projectile> activeProjectiles;

            public void Start() {
                frame = GetComponent<FrameEnforcer>();

                projectilePrefabs = new Dictionary<int, GameObject>();
                preInstantiatedProjectiles = new Dictionary<int, List<Projectile>>();
                preInstantiatedProjectilesIndex = new Dictionary<int, int>();

                activeProjectiles = new List<Projectile>();
            }

            public int RegisterProjectile(GameObject prefab, int instantiateNum = 4) {
                int id = projectileCounter++;
                projectilePrefabs.Add(id, prefab);

                List<Projectile> instantiatedProjectilesList = new List<Projectile>();
                for (int n = 0; n < instantiateNum; ++n) {
                    Projectile projectile = GameObject.Instantiate<GameObject>(prefab, underLevel, Quaternion.identity, projectileEmpty).GetComponent<Projectile>();
                    projectile.Store(underLevel);

                    instantiatedProjectilesList.Add(projectile);
                }

                return id;
            }

            public void SetHitBox() {

            }

            public void FireProjectile(int id) {
                Projectile projectile = null;
                int index = preInstantiatedProjectilesIndex[id];
                List<Projectile> projectilePool = preInstantiatedProjectiles[id];

                int stopIndex = index;
                do {
                    if (!projectilePool[index].active) {
                        projectile = projectilePool[index];
                    }
                    index = (index + 1) % projectilePool.Count;
                } while (index != stopIndex && projectile == null);
                preInstantiatedProjectilesIndex[id] = index;

                projectile.FireProjectile();
            }
        }
    }
}