using System;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Utility {
        public class ResourceRecycler<T> where T : MonoBehaviour, IResource {

            [SerializeField]
            private List<T> assets;
            private List<bool> inUse;
            private int index;

            private Vector3 storeLocation;
            private T prefab;
            private Transform parent;
            private Action<T> callback;

                // This is supposed to be the constructor
            public ResourceRecycler(T prefab, Vector3 storeLocation, int instantiateNum = 4, Transform parent = null, Action<T> callback = null) {
                this.storeLocation = storeLocation;
                this.prefab = prefab;
                this.parent = parent;
                this.callback = callback;

                assets = new List<T>();
                inUse = new List<bool>();
                index = 0;

                for (int n = 0; n < instantiateNum; ++n) {
                    InitResource();
                }
            }

            public T UseResource() {
                T asset = null;
                int index = this.index;

                int stopIndex = index;
                do {
                    if (!assets[index].Active()) {
                        asset = assets[index];
                    }
                    index = (index + 1) % assets.Count;
                } while (index != stopIndex && asset == null);
                this.index = index;

                if (index == stopIndex && asset == null) {
                    asset = ExpandResourcePool();
                }

                asset.Activate();
                return asset;
            }

            private void InitResource() {
                T asset = GameObject.Instantiate<T>(prefab, storeLocation, Quaternion.identity, parent);

                assets.Add(asset);
                inUse.Add(false);

                if (callback != null) {
                    callback(asset);
                }
            }

            private T ExpandResourcePool() {
                int newSize = assets.Count * 2;
                for (int n = assets.Count; n < newSize; ++n) {
                    InitResource();
                }

                return UseResource();
            }
        }
    }
}