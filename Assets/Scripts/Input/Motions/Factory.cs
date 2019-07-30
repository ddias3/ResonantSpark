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
            private MemPools memPools;
            private Indices ind;

            public Factory() {
                memPools = new MemPools {
                    neutRet = new List<Combination>(INIT_POOL_SIZE),
                    dirPress = new List<Combination>(INIT_POOL_SIZE),
                    doubleTap = new List<Combination>(INIT_POOL_SIZE),
                };
                ind = new Indices {
                    doubleTap = 0,
                    dirPress = 0
                };

                for (int n = 0; n < INIT_POOL_SIZE; ++n) {
                    memPools.neutRet.Add(new NeutralReturn(-1));
                    memPools.dirPress.Add(new DirectionPress(-1, FightingGameInputCodeDir.None));
                    memPools.doubleTap.Add(new DoubleTap(-1, -1));
                }
                empty = new Empty();
            }

            private void IncreasePoolSize(ref List<Combination> oldMemPool, Action<List<Combination>> callback) {
                List<Combination> newMemPool = new List<Combination>(oldMemPool.Count * 2);
                int n;
                for (n = 0; n < oldMemPool.Count; ++n) {
                    newMemPool.Add(oldMemPool[n]);
                }
                for (; n < oldMemPool.Count * 2; ++n) {
                    callback.Invoke(newMemPool);
                }
                oldMemPool = newMemPool;
            }

            public Combination EmptyInput() {
                return empty;
            }

            public Combination FindNextAvailable(List<Combination> memPool, ref int index) {
                Combination retValue = null;
                int stopIndex = index;
                if (memPool[index].Stale(60)) {
                    retValue = memPool[index];
                }
                for (index = stopIndex + 1; index != stopIndex && retValue == null; index = (index + 1) % memPool.Count) {
                    if (memPool[index].Stale(60)) {
                        retValue = memPool[index];
                    }
                }
                return retValue;
            }

            public DoubleTap CreateDoubleTap(int frameGapStart, int frameGatEnd) {
                DoubleTap combo = null;
                do {
                    combo = (DoubleTap) FindNextAvailable(memPools.doubleTap, ref ind.doubleTap);
                    if (combo == null) {
                        IncreasePoolSize(ref memPools.doubleTap, (memPool) => {
                            memPool.Add(new DoubleTap(-1, -1));
                        });
                    }
                } while (combo == null);
                return combo;
            }

            public DirectionPress CreateDirectionPress(int frameTrigger) {
                DirectionPress combo = null;
                do {
                    combo = (DirectionPress) FindNextAvailable(memPools.dirPress, ref ind.dirPress);
                    if (combo == null) {
                        IncreasePoolSize(ref memPools.dirPress, (memPool) => {
                            memPool.Add(new DirectionPress(-1, FightingGameInputCodeDir.None));
                        });
                    }
                } while (combo == null);
                return combo;
            }

            public NeutralReturn CreateNeutralReturn(int frameTrigger) {
                NeutralReturn combo = null;
                do {
                    combo = (NeutralReturn) FindNextAvailable(memPools.neutRet, ref ind.neutRet);
                    if (combo == null) {
                        IncreasePoolSize(ref memPools.neutRet, (memPool) => {
                            memPool.Add(new NeutralReturn(-1));
                        });
                    }
                } while (combo == null);
                return combo;
            }
        }
    }
}