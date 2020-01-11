using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Gameplay;
using ResonantSpark.Utility;

namespace ResonantSpark {
    namespace Service {
        public class FightingGameService : MonoBehaviour, IHitBoxService, IProjectileService {

            public Vector3 underLevel = new Vector3(0, -100, 0);
            public Transform projectileEmpty;
            public Transform hitBoxEmpty;

            private static int projectileCounter = 0;

            private FrameEnforcer frame;

            private Dictionary<int, GameObject> projectilePrefabs;
            private Dictionary<int, ResourceRecycler<Projectile>> projectileMap;

            private List<Projectile> activeProjectiles;

            public void Start() {
                frame = GetComponent<FrameEnforcer>();

                projectilePrefabs = new Dictionary<int, GameObject>();
                projectileMap = new Dictionary<int, ResourceRecycler<Projectile>>();
            }

            public int RegisterProjectile(GameObject prefab, int instantiateNum = 4) {
                int id = projectileCounter++;
                projectilePrefabs.Add(id, prefab);

                projectileMap.Add(id, new ResourceRecycler<Projectile>(prefab.GetComponent<Projectile>(), underLevel, instantiateNum, projectileEmpty, projectile => {
                    //projectile.Deactivate();
                    Debug.Log("Created projectile id: " + id);
                }));

                return id;
            }

            public void SetHitBox() {

            }

            public void FireProjectile(int id) {
                Projectile projectile = projectileMap[id].UseResource();
                projectile.FireProjectile();
            }

            public void HitBox(Vector3 position, int durationFrames) {
                throw new System.NotImplementedException();
            }
        }
    }
}