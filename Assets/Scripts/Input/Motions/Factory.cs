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

            [System.Serializable]
            private struct Indices {
                public int neutRet;
                public int doubleTap;
                public int dirPress;
            }

            [System.Serializable]
            private struct MemPools {
                public List<Combination> neutRet;
                public List<Combination> dirPress;
                public List<Combination> doubleTap;
            }
            
            private Empty empty;
            private Dictionary<System.Type, List<Combination>> memPools;
            private Dictionary<System.Type, int> ind;
            private Dictionary<System.Type, Action<List<Combination>>> expandCallbacks;

            public Factory() {
                memPools = new Dictionary<System.Type, List<Combination>>();
                memPools.Add(typeof(DoubleTap), new List<Combination>(INIT_POOL_SIZE));
                memPools.Add(typeof(DirectionPress), new List<Combination>(INIT_POOL_SIZE));
                memPools.Add(typeof(NeutralReturn), new List<Combination>(INIT_POOL_SIZE));

                ind = new Dictionary<System.Type, int>();
                ind.Add(typeof(DoubleTap), 0);
                ind.Add(typeof(DirectionPress), 0);
                ind.Add(typeof(NeutralReturn), 0);

                expandCallbacks = new Dictionary<System.Type, Action<List<Combination>>>();
                expandCallbacks.Add(typeof(DoubleTap), (memPool) => {
                    memPool.Add(new DoubleTap(-1, -1));
                });
                expandCallbacks.Add(typeof(DirectionPress), (memPool) => {
                    memPool.Add(new DirectionPress(-1, FightingGameInputCodeDir.None));
                });
                expandCallbacks.Add(typeof(NeutralReturn), (memPool) => {
                    memPool.Add(new NeutralReturn(-1));
                });

                var neutRet = memPools[typeof(NeutralReturn)];
                var dirPress = memPools[typeof(DirectionPress)];
                var doubleTap = memPools[typeof(DoubleTap)];

                for (int n = 0; n < INIT_POOL_SIZE; ++n) {
                    neutRet.Add(new NeutralReturn(-1));
                    dirPress.Add(new DirectionPress(-1, FightingGameInputCodeDir.None));
                    doubleTap.Add(new DoubleTap(-1, -1));
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

            public Combination FindNextAvailable(Type type) {
                Combination retValue = null;
                int index = ind[type];
                List<Combination> memPool = memPools[type];

                int stopIndex = index;
                if (memPool[index].Stale(60)) {
                    retValue = memPool[index];
                }
                for (index = stopIndex + 1; index != stopIndex && retValue == null; index = (index + 1) % memPool.Count) {
                    if (memPool[index].Stale(60)) {
                        retValue = memPool[index];
                    }
                }
                ind[type] = index;
                return retValue;
            }

            public T_Combo CreateCombination<T_Combo>(int frameTrigger) where T_Combo : Combination {
                T_Combo combo = null;
                do {
                    combo = (T_Combo) FindNextAvailable(typeof(T_Combo));
                    if (combo == null) {
                        IncreasePoolSize(typeof(T_Combo), expandCallbacks[typeof(T_Combo)]);
                    }
                } while (combo == null);
                return combo;
            }
        }
    }
}