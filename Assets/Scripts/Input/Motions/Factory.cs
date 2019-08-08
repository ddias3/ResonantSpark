using System;
using System.Collections.Generic;
using ResonantSpark.Input.Combinations;

namespace ResonantSpark {
    namespace Input {
        public class Factory {
            private const int INIT_POOL_SIZE = 16;

            //private enum MemoryPool : int {
            //    None = 0,
            //    DoubleTap,
            //    DirectionPress,
            //}

            private Empty empty;
            private Dictionary<System.Type, List<Combination>> memPools;
            private Dictionary<System.Type, int> ind;

            public Factory() {
                memPools = new Dictionary<System.Type, List<Combination>> {
                    { typeof(DoubleTap), new List<Combination>(INIT_POOL_SIZE) },
                    { typeof(DirectionPress), new List<Combination>(INIT_POOL_SIZE) },
                    { typeof(NeutralReturn), new List<Combination>(INIT_POOL_SIZE) },
                    { typeof(DirectionHold), new List<Combination>(INIT_POOL_SIZE) }
                };

                ind = new Dictionary<System.Type, int> {
                    { typeof(DoubleTap), 0 },
                    { typeof(DirectionPress), 0 },
                    { typeof(NeutralReturn), 0 },
                    { typeof(DirectionHold), 0 }
                };

                var neutRet = memPools[typeof(NeutralReturn)];
                var dirPress = memPools[typeof(DirectionPress)];
                var doubleTap = memPools[typeof(DoubleTap)];
                var dirHold = memPools[typeof(DirectionHold)];

                for (int n = 0; n < INIT_POOL_SIZE; ++n) {
                    neutRet.Add(new NeutralReturn());
                    dirPress.Add(new DirectionPress());
                    doubleTap.Add(new DoubleTap());
                    dirHold.Add(new DirectionHold());
                }

                empty = new Empty();
            }

            private void IncreasePoolSize(Type type, Action<List<Combination>> callback) {
                List<Combination> oldMemPool = memPools[type];
                List<Combination> newMemPool = new List<Combination>(oldMemPool.Count * 2);
                int n;
                for (n = 0; n < oldMemPool.Count; ++n) {
                    newMemPool.Add(oldMemPool[n]);
                }
                for (; n < oldMemPool.Count * 2; ++n) {
                    callback.Invoke(newMemPool);
                }
                memPools[type] = newMemPool;
            }

            public Combination EmptyInput() {
                return empty;
            }

            public Combination FindNextAvailable(Type type, int frameCurrent) {
                Combination retValue = null;
                int index = ind[type];
                List<Combination> memPool = memPools[type];

                int stopIndex = index;
                if (memPool[index].Stale(frameCurrent)) {
                    retValue = memPool[index];
                }
                for (index = (stopIndex + 1) % memPool.Count; index != stopIndex && retValue == null; index = (index + 1) % memPool.Count) {
                    if (memPool[index].Stale(frameCurrent)) {
                        retValue = memPool[index];
                    }
                }
                ind[type] = index;
                return retValue;
            }

            public T_Combo CreateCombination<T_Combo>(int frameCurrent) where T_Combo : Combination, new() {
                T_Combo combo = null;
                do {
                    combo = (T_Combo) FindNextAvailable(typeof(T_Combo), frameCurrent);
                    if (combo == null) {
                        IncreasePoolSize(typeof(T_Combo), (memPool) => {
                            memPool.Add(new T_Combo());
                        });
                    }
                } while (combo == null);
                return combo;
            }
        }
    }
}