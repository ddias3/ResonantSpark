using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Gameplay;
using ResonantSpark.Utility;

namespace ResonantSpark {
    namespace Service {
        public class ProjectileService : MonoBehaviour, IProjectileService {

            private static int projectileCounter = 0;

            public Vector3 underLevel = new Vector3(0, -100, 0);
            public Transform projectileEmpty;

            private FrameEnforcer frame;

            private Dictionary<int, GameObject> projectilePrefabs;
            private Dictionary<int, ResourceRecycler<Projectile>> projectileMap;

            private List<Projectile> activeProjectiles;

            public void Start() {
                frame = GameObject.FindGameObjectWithTag("rspTime").GetComponent<FrameEnforcer>();
                frame.AddUpdate((int) FramePriority.Service, new System.Action<int>(FrameUpdate));
                frame.AddUpdate((int)FramePriority.ActivePollingReset, new System.Action<int>(ResetActivePolling));

                projectilePrefabs = new Dictionary<int, GameObject>();
                projectileMap = new Dictionary<int, ResourceRecycler<Projectile>>();

                activeProjectiles = new List<Projectile>();

                EventManager.TriggerEvent<Events.ServiceReady, Type>(typeof(ProjectileService));
            }

            private void FrameUpdate(int frameIndex) {

            }

            private void ResetActivePolling(int frameIndex) {

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

            public void FireProjectile(int id) {
                Projectile projectile = projectileMap[id].UseResource();
                projectile.FireProjectile();
            }
        }
    }
}