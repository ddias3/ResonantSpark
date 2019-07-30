using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Input.Combinations;

namespace ResonantSpark {
    namespace Input {
        public class Service {
            public static void FindCombinations(FightingGameInputCodeDir[] buffer, Factory inputFactory, List<Combination> result) {
                int test0 = FindDoubleDirectionTaps(buffer, inputFactory, result);
                int test1 = FindDirectionPresses(buffer, inputFactory, result);
                int test2 = FindNeutralReturns(buffer, inputFactory, result);

                int x = test0 + test1 + test2;
                result.Sort();
            }

            public static int FindNeutralReturns(FightingGameInputCodeDir[] buffer, Factory inputFactory, List<Combination> result) {
                int numFound = 0;
                for (int n = 0; n < buffer.Length; ++n) {
                    FightingGameInputCodeDir currDir = buffer[n];
                    if (currDir == FightingGameInputCodeDir.Neutral) {
                        for (; n < buffer.Length; ++n) {
                            if (buffer[n] != FightingGameInputCodeDir.Neutral) {
                                result.Add(inputFactory.CreateCombination<NeutralReturn>(n));
                                numFound++;
                                break;
                            }
                        }
                    }
                }
                return numFound;
            }

            public static int FindDirectionPresses(FightingGameInputCodeDir[] buffer, Factory inputFactory, List<Combination> result) {
                int numFound = 0;
                for (int n = 0; n < buffer.Length; ++n) {
                    FightingGameInputCodeDir currDir = buffer[n];
                    if (currDir != FightingGameInputCodeDir.Neutral) {
                        for (; n < buffer.Length; ++n) {
                            if (buffer[n] != currDir) {
                                result.Add(inputFactory.CreateCombination<DirectionPress>(n));
                                numFound++;
                                break;
                            }
                        }
                    }
                }
                return numFound;
            }

            public static int FindDirectionHolds(FightingGameInputCodeDir[] buffer, Factory inputFactory, List<Combination> result) {
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

            public static int FindDoubleDirectionTaps(FightingGameInputCodeDir[] buffer, Factory inputFactory, List<Combination> result) {
                int numFound = 0;
                for (int n = 0; n < buffer.Length; ++n) {
                    FightingGameInputCodeDir currDir = FightingGameInputCodeDir.None;
                    int frameGapStart = -1;
                    int frameGapEnd = -1;
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
                                    frameGapStart = n;
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

                    switch (currDir) {
                        case FightingGameInputCodeDir.Right:
                            for (; n < buffer.Length; ++n) {
                                if (buffer[n] == FightingGameInputCodeDir.Right
                                    /*|| buffer[n] == FightingGameInputCodeDir.DownRight || buffer[n] == FightingGameInputCodeDir.UpRight*/) {
                                    frameGapEnd = n;
                                    result.Add(inputFactory.CreateCombination<DoubleTap>(frameGapEnd));
                                    numFound++;
                                    break;
                                }
                            }
                            break;
                        case FightingGameInputCodeDir.Down:
                            for (; n < buffer.Length; ++n) {
                                if (buffer[n] == FightingGameInputCodeDir.Down
                                    /*|| buffer[n] == FightingGameInputCodeDir.DownLeft || buffer[n] == FightingGameInputCodeDir.DownRight*/) {
                                    frameGapEnd = n;
                                    result.Add(inputFactory.CreateCombination<DoubleTap>(frameGapEnd));
                                    numFound++;
                                    break;
                                }
                            }
                            break;
                        case FightingGameInputCodeDir.Left:
                            for (; n < buffer.Length; ++n) {
                                if (buffer[n] == FightingGameInputCodeDir.Left
                                    /*|| buffer[n] == FightingGameInputCodeDir.UpLeft || buffer[n] == FightingGameInputCodeDir.DownLeft*/) {
                                    frameGapEnd = n;
                                    result.Add(inputFactory.CreateCombination<DoubleTap>(frameGapEnd));
                                    numFound++;
                                    break;
                                }
                            }
                            break;
                        case FightingGameInputCodeDir.Up:
                            for (; n < buffer.Length; ++n) {
                                if (buffer[n] == FightingGameInputCodeDir.Up
                                    /*|| buffer[n] == FightingGameInputCodeDir.UpRight || buffer[n] == FightingGameInputCodeDir.UpLeft*/) {
                                    frameGapEnd = n;
                                    result.Add(inputFactory.CreateCombination<DoubleTap>(frameGapEnd));
                                    numFound++;
                                    break;
                                }
                            }
                            break;
                    }
                }
                return numFound;
            }
        }
    }
}