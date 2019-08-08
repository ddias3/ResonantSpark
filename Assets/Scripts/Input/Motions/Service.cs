using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Input.Combinations;

namespace ResonantSpark {
    namespace Input {
        public class Service {
            public static void FindCombinations(FightingGameInputCodeDir[] buffer, Factory inputFactory, int frameIndex, List<Combination> activeInputs) {
                int test0 = FindDoubleDirectionTaps(buffer, inputFactory, frameIndex, activeInputs);
                int test1 = FindDirectionPresses(buffer, inputFactory, frameIndex, activeInputs);
                int test2 = FindNeutralReturns(buffer, inputFactory, frameIndex, activeInputs);

                int x = test0 + test1 + test2;
                activeInputs.Sort();
            }

            private static T_Combo AddToActiveInputs<T_Combo>(List<Combination> activeInputs, Factory inputFactory, int frameTrigger, int frameCurrent, Action<T_Combo> initCallback) where T_Combo : Combination, new() {
                foreach (var cmb in activeInputs) {
                    if (cmb.GetType() == typeof(T_Combo) && cmb.GetFrame() == frameTrigger) {// && !cmb.Stale(frameCurrent)) {
                        return (T_Combo) cmb;
                    }
                }
                T_Combo newInput = inputFactory.CreateCombination<T_Combo>(frameCurrent);
                initCallback.Invoke(newInput);
                activeInputs.Add(newInput);
                return newInput;
            }

            public static int FindNeutralReturns(FightingGameInputCodeDir[] buffer, Factory inputFactory, int frameIndex, List<Combination> activeInputs) {
                int numFound = 0;
                for (int n = 0; n < buffer.Length; ++n) {
                    FightingGameInputCodeDir currDir = buffer[n];
                    if (currDir == FightingGameInputCodeDir.Neutral) {
                        for (; n < buffer.Length; ++n) {
                            if (buffer[n] != FightingGameInputCodeDir.Neutral) {
                                NeutralReturn input = AddToActiveInputs<NeutralReturn>(activeInputs, inputFactory, frameIndex - n, frameIndex, (newInput) => {
                                    newInput.Init(frameIndex - n);
                                });
                                numFound++;
                                break;
                            }
                        }
                    }
                }
                return numFound;
            }

            public static int FindDirectionPresses(FightingGameInputCodeDir[] buffer, Factory inputFactory, int frameIndex, List<Combination> activeInputs) {
                int numFound = 0;
                for (int n = 0; n < buffer.Length; ++n) {
                    FightingGameInputCodeDir currDir = buffer[n];
                    if (currDir != FightingGameInputCodeDir.Neutral) {
                        for (; n < buffer.Length; ++n) {
                            if (buffer[n] != currDir) {
                                DirectionPress input = AddToActiveInputs<DirectionPress>(activeInputs, inputFactory, frameIndex - n, frameIndex, (newInput) => {
                                    newInput.Init(frameIndex - n, buffer[n]);
                                });

                                numFound++;
                                break;
                            }
                        }
                    }
                }
                return numFound;
            }

            public static int FindDirectionHolds(FightingGameInputCodeDir[] buffer, Factory inputFactory, int frameIndex, List<Combination> activeInputs) {
                    // TODO: This is entirely wrong.
                //int numFound = 0;
                //for (int n = 0; n < buffer.Length; ++n) {
                //    FightingGameInputCodeDir currDir = buffer[n];
                //    int start = n;
                //    bool valid = false;
                //    for (; n < buffer.Length; ++n) {
                //        if (currDir == buffer[n] && n >= start + 12) {
                //            valid = true;
                //            break;
                //        }
                //    }
                //    if (valid) {
                //        for (; n < buffer.Length; ++n) {

                //        }
                //    }
                //    if (buffer[n] != currDir) {
                //            result.Add(inputFactory.CreateCombination<DirectionPress>(n));
                //            numFound++;
                //        break;
                //    }
                //}
                //return numFound;
                return 0;
            }

            public static int FindDoubleDirectionTaps(FightingGameInputCodeDir[] buffer, Factory inputFactory, int frameIndex, List<Combination> activeInputs) {
                int numFound = 0;
                for (int n = 0; n < buffer.Length; ++n) {
                    FightingGameInputCodeDir currDir = FightingGameInputCodeDir.None;
                    int relativeFrameGapStart = -1;
                    int relativeFrameGapEnd = -1;
                    for (; n < buffer.Length; ++n) {
                        if (buffer[n] == FightingGameInputCodeDir.Right
                                || buffer[n] == FightingGameInputCodeDir.Down
                                || buffer[n] == FightingGameInputCodeDir.Left
                                || buffer[n] == FightingGameInputCodeDir.Up) {
                            currDir = buffer[n];
                            break;
                        }
                    }

                    if (currDir == FightingGameInputCodeDir.None) {
                        return 0;
                    }

                    switch (currDir) {
                        case FightingGameInputCodeDir.Right:
                        case FightingGameInputCodeDir.Down:
                        case FightingGameInputCodeDir.Left:
                        case FightingGameInputCodeDir.Up:
                            for (; n < buffer.Length; ++n) {
                                if (buffer[n] == FightingGameInputCodeDir.Neutral) {
                                    relativeFrameGapEnd = n;
                                    break;
                                }
                            }
                            break;
                    }

                    for (; n < buffer.Length; ++n) {
                        if (buffer[n] != FightingGameInputCodeDir.Neutral) {
                            break;
                        }
                    }

                        // The gap between double taps can't be too large
                    if (n - relativeFrameGapEnd > 5) {
                        break;
                    }

                    switch (currDir) {
                        case FightingGameInputCodeDir.Right:
                            for (; n < buffer.Length; ++n) {
                                if (buffer[n] == FightingGameInputCodeDir.Right
                                    /*|| buffer[n] == FightingGameInputCodeDir.DownRight || buffer[n] == FightingGameInputCodeDir.UpRight*/) {
                                    relativeFrameGapStart = n;
                                    AddDoubleTap(activeInputs, inputFactory, frameIndex - relativeFrameGapEnd, frameIndex, FightingGameInputCodeDir.Right, frameIndex - relativeFrameGapStart, frameIndex - relativeFrameGapEnd);
                                    numFound++;
                                    break;
                                }
                            }
                            break;
                        case FightingGameInputCodeDir.Down:
                            for (; n < buffer.Length; ++n) {
                                if (buffer[n] == FightingGameInputCodeDir.Down
                                    /*|| buffer[n] == FightingGameInputCodeDir.DownLeft || buffer[n] == FightingGameInputCodeDir.DownRight*/) {
                                    relativeFrameGapStart = n;
                                    AddDoubleTap(activeInputs, inputFactory, frameIndex - relativeFrameGapEnd, frameIndex, FightingGameInputCodeDir.Down, frameIndex - relativeFrameGapStart, frameIndex - relativeFrameGapEnd);
                                    numFound++;
                                    break;
                                }
                            }
                            break;
                        case FightingGameInputCodeDir.Left:
                            for (; n < buffer.Length; ++n) {
                                if (buffer[n] == FightingGameInputCodeDir.Left
                                    /*|| buffer[n] == FightingGameInputCodeDir.UpLeft || buffer[n] == FightingGameInputCodeDir.DownLeft*/) {
                                    relativeFrameGapStart = n;
                                    AddDoubleTap(activeInputs, inputFactory, frameIndex - relativeFrameGapEnd, frameIndex, FightingGameInputCodeDir.Left, frameIndex - relativeFrameGapStart, frameIndex - relativeFrameGapEnd);
                                    numFound++;
                                    break;
                                }
                            }
                            break;
                        case FightingGameInputCodeDir.Up:
                            for (; n < buffer.Length; ++n) {
                                if (buffer[n] == FightingGameInputCodeDir.Up
                                    /*|| buffer[n] == FightingGameInputCodeDir.UpRight || buffer[n] == FightingGameInputCodeDir.UpLeft*/) {
                                    relativeFrameGapStart = n;
                                    AddDoubleTap(activeInputs, inputFactory, frameIndex - relativeFrameGapEnd, frameIndex, FightingGameInputCodeDir.Up, frameIndex - relativeFrameGapStart, frameIndex - relativeFrameGapEnd);
                                    numFound++;
                                    break;
                                }
                            }
                            break;
                    }
                }
                return numFound;
            }

            private static void AddDoubleTap(List<Combination> activeInputs, Factory inputFactory, int frameTrigger, int frameCurrent, FightingGameInputCodeDir direction, int frameGapStart, int frameGapEnd) {
                AddToActiveInputs<DoubleTap>(activeInputs, inputFactory, frameTrigger, frameCurrent, (newInput) => {
                    newInput.Init(frameGapEnd, frameGapStart, direction);
                });
            }
        }
    }
}