using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Input.Combinations;

namespace ResonantSpark {
    namespace Input {
        public class Factory {
            private const int INIT_POOL_SIZE = 16;

            private Empty empty;
            private Dictionary<System.Type, List<Combination>> memPools;
            private Dictionary<System.Type, int> ind;

            public Factory() {
                memPools = new Dictionary<System.Type, List<Combination>> {
                    { typeof(DirectionCurrent), new List<Combination>(INIT_POOL_SIZE) },
                    { typeof(ButtonsCurrent), new List<Combination>(INIT_POOL_SIZE) },
                    { typeof(DoubleTap), new List<Combination>(INIT_POOL_SIZE) },
                    { typeof(DirectionPress), new List<Combination>(INIT_POOL_SIZE) },
                    { typeof(NeutralReturn), new List<Combination>(INIT_POOL_SIZE) },
                    { typeof(DirectionLongHold), new List<Combination>(INIT_POOL_SIZE) },
                    { typeof(ButtonPress), new List<Combination>(INIT_POOL_SIZE) },
                    { typeof(Button2Press), new List<Combination>(INIT_POOL_SIZE) },
                    { typeof(Button3Press), new List<Combination>(INIT_POOL_SIZE) },
                    { typeof(ButtonRelease), new List<Combination>(INIT_POOL_SIZE) },
                    { typeof(DirectionPlusButton), new List<Combination>(INIT_POOL_SIZE) },
                    { typeof(QuarterCircle), new List<Combination>(INIT_POOL_SIZE) },
                    { typeof(QuarterCircleButtonPress), new List<Combination>(INIT_POOL_SIZE) },
                };

                ind = new Dictionary<System.Type, int> {
                    { typeof(DirectionCurrent), 0 },
                    { typeof(ButtonsCurrent), 0 },
                    { typeof(DoubleTap), 0 },
                    { typeof(DirectionPress), 0 },
                    { typeof(NeutralReturn), 0 },
                    { typeof(DirectionLongHold), 0 },
                    { typeof(ButtonPress), 0 },
                    { typeof(Button2Press), 0 },
                    { typeof(Button3Press), 0 },
                    { typeof(ButtonRelease), 0 },
                    { typeof(DirectionPlusButton), 0 },
                    { typeof(QuarterCircle), 0 },
                    { typeof(QuarterCircleButtonPress), 0 },
                };

                var dirCurr = memPools[typeof(DirectionCurrent)];
                var butsCurr = memPools[typeof(ButtonsCurrent)];
                var neutRet = memPools[typeof(NeutralReturn)];
                var dirPress = memPools[typeof(DirectionPress)];
                var doubleTap = memPools[typeof(DoubleTap)];
                var dirHold = memPools[typeof(DirectionLongHold)];
                var butPress = memPools[typeof(ButtonPress)];
                var but2Press = memPools[typeof(Button2Press)];
                var but3Press = memPools[typeof(Button3Press)];
                var butRelse = memPools[typeof(ButtonRelease)];
                var dirPlusBut = memPools[typeof(DirectionPlusButton)];
                var quartCir = memPools[typeof(QuarterCircle)];
                var quartCirPlusBut = memPools[typeof(QuarterCircleButtonPress)];

                for (int n = 0; n < INIT_POOL_SIZE; ++n) {
                    //dirCurr.Add(new DirectionCurrent());
                    //butsCurr.Add(new ButtonsCurrent());
                    //neutRet.Add(new NeutralReturn());
                    //dirPress.Add(new DirectionPress());
                    //doubleTap.Add(new DoubleTap());
                    //dirHold.Add(new DirectionLongHold());
                    //butPress.Add(new ButtonPress());
                    //but2Press.Add(new Button2Press());
                    //but3Press.Add(new Button3Press());
                    //butRelse.Add(new ButtonRelease());
                    //dirPlusBut.Add(new DirectionPlusButton());
                    //quartCir.Add(new QuarterCircle());
                    //quartCirPlusBut.Add(new QuarterCircleButtonPress());
                    dirCurr.Add(ScriptableObject.CreateInstance<DirectionCurrent>());
                    butsCurr.Add(ScriptableObject.CreateInstance<ButtonsCurrent>());
                    neutRet.Add(ScriptableObject.CreateInstance<NeutralReturn>());
                    dirPress.Add(ScriptableObject.CreateInstance<DirectionPress>());
                    doubleTap.Add(ScriptableObject.CreateInstance<DoubleTap>());
                    dirHold.Add(ScriptableObject.CreateInstance<DirectionLongHold>());
                    butPress.Add(ScriptableObject.CreateInstance<ButtonPress>());
                    but2Press.Add(ScriptableObject.CreateInstance<Button2Press>());
                    but3Press.Add(ScriptableObject.CreateInstance<Button3Press>());
                    butRelse.Add(ScriptableObject.CreateInstance<ButtonRelease>());
                    dirPlusBut.Add(ScriptableObject.CreateInstance<DirectionPlusButton>());
                    quartCir.Add(ScriptableObject.CreateInstance<QuarterCircle>());
                    quartCirPlusBut.Add(ScriptableObject.CreateInstance<QuarterCircleButtonPress>());
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
                if (!memPool[index].inUse && memPool[index].Stale(frameCurrent)) {
                    retValue = memPool[index];
                }
                for (index = (stopIndex + 1) % memPool.Count; index != stopIndex && retValue == null; index = (index + 1) % memPool.Count) {
                    if (!memPool[index].inUse && memPool[index].Stale(frameCurrent)) {
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
                            //memPool.Add(new T_Combo());
                            memPool.Add(ScriptableObject.CreateInstance<T_Combo>());
                        });
                    }

                    // TODO: Create a way to get out of this infinite loop if next available is ALWAYS null.
                } while (combo == null);
                return combo;
            }
        }
    }
}