using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Input.Combinations;

namespace ResonantSpark {
    namespace Input {
        public class Service {
            public static void FindCombinations(InputBufferReader reader, Factory inputFactory, List<Combination> activeInputs) {
                int test0 = FindDirectionPresses(reader, inputFactory, activeInputs);
                int test1 = FindNeutralReturns(reader, inputFactory, activeInputs);
                int test2 = FindDoubleTaps(reader, inputFactory, activeInputs);
                int test3 = FindDirectionCurrent(reader, inputFactory, activeInputs);
                //int test4 = FindDirectionLongHolds(reader, inputFactory, activeInputs);    // TODO: Fix this, but the feature may not even exist.
                //int test6 = FindButtonPresses(reader, inputFactory, activeInputs);
                //int test7 = FindButton2Presses(reader, inputFactory, activeInputs);
                //int test8 = FindButton3Presses(reader, inputFactory, activeInputs);
                //int test9 = FindDirectionPlusButtons(reader, inputFactory, activeInputs);
                int testA = FindQuarterCircles(reader, inputFactory, activeInputs);
                //int testB = FindQuarterCircleButtonPresses(reader, inputFactory, activeInputs);

                //int x = test0 + test1 + test2 + test3 + test4 + test6 + test7 + test8 + test9 + testA + testB;
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

            // regex = /(?<=([^5]))(?=[5])/g
            public static int FindNeutralReturns(InputBufferReader reader, Factory inputFactory, List<Combination> activeInputs) {
                int numFound = 0;
                reader.ResetCurrIndex();

                bool notNeutral = false;
                while (reader.ReadyNext()) {
                    int inputFrameIndex = reader.ReadBuffer(out GameInputStruct curr);

                    if (curr.direction != FightingGameInputCodeDir.Neutral) {
                        notNeutral = true;
                    }
                    else if (notNeutral) {
                        AddToActiveInputs<NeutralReturn>(activeInputs, inputFactory, inputFrameIndex, reader.GetCurrentFrame(), newInput => {
                            numFound++;
                            newInput.Init(inputFrameIndex);
                        });
                        notNeutral = false;
                    }
                }

                return numFound;
            }

            // regex = /(?<=([1-9]))(?=([^5]))(?!\1)/g
            public static int FindDirectionPresses(InputBufferReader reader, Factory inputFactory, List<Combination> activeInputs) {
                int numFound = 0;
                reader.ResetCurrIndex();

                FightingGameInputCodeDir prevDir = FightingGameInputCodeDir.None;
                while (reader.ReadyNext()) {
                    int inputFrameIndex = reader.ReadBuffer(out GameInputStruct curr);

                    if (curr.direction != FightingGameInputCodeDir.Neutral && prevDir != curr.direction) {
                        AddToActiveInputs<DirectionPress>(activeInputs, inputFactory, inputFrameIndex, reader.GetCurrentFrame(), newInput => {
                            numFound++;
                            newInput.Init(inputFrameIndex, curr.direction);
                        });
                    }
                    prevDir = curr.direction;
                }
                return numFound;
            }

                // This requires a hold of 20 frames
            // regex = /(?<=([1-9]))(?=([^5])\2{19,})(?!\1)|^([^5])\3+$/g

                // This one is for 40 frames
            public static int FindDirectionLongHolds(InputBufferReader reader, Factory inputFactory, List<Combination> activeInputs) {
                int numFound = 0;
                reader.ResetCurrIndex();

                int horizontalHold = 0;
                int horizontalStart = 0;
                int verticalHold = 0;
                int verticalStart = 0;
                while (reader.ReadyNext()) {
                    int inputFrameIndex = reader.ReadBuffer(out GameInputStruct curr);

                    if (curr.direction == FightingGameInputCodeDir.Neutral) {
                        horizontalHold = 0;
                        verticalHold = 0;
                        horizontalStart = inputFrameIndex;
                        verticalStart = inputFrameIndex;
                    }
                    else {
                        if (curr.direction == FightingGameInputCodeDir.Left || curr.direction == FightingGameInputCodeDir.DownLeft || curr.direction == FightingGameInputCodeDir.UpLeft) {
                            horizontalHold = -1;
                        }
                        else if (curr.direction == FightingGameInputCodeDir.Right || curr.direction == FightingGameInputCodeDir.DownRight || curr.direction == FightingGameInputCodeDir.UpRight) {
                            horizontalHold = 1;
                        }

                        if (curr.direction == FightingGameInputCodeDir.Up || curr.direction == FightingGameInputCodeDir.UpLeft || curr.direction == FightingGameInputCodeDir.UpRight) {
                            verticalHold = 1;
                        }
                        else if (curr.direction == FightingGameInputCodeDir.Down || curr.direction == FightingGameInputCodeDir.DownLeft || curr.direction == FightingGameInputCodeDir.DownRight) {
                            verticalHold = -1;
                        }

                        if (inputFrameIndex - horizontalStart >= 40) {
                            DirectionLongHold input = AddToActiveInputs<DirectionLongHold>(activeInputs, inputFactory, inputFrameIndex, reader.GetCurrentFrame(), (newInput) => {
                                numFound++;
                                newInput.Init(inputFrameIndex, FightingGameInputCodeDir.Neutral + horizontalHold, inputFrameIndex - horizontalStart);
                            });
                        }

                        if (inputFrameIndex - verticalStart >= 40) {
                            DirectionLongHold input = AddToActiveInputs<DirectionLongHold>(activeInputs, inputFactory, inputFrameIndex, reader.GetCurrentFrame(), (newInput) => {
                                numFound++;
                                newInput.Init(inputFrameIndex, FightingGameInputCodeDir.Neutral + 3 * verticalHold, inputFrameIndex - verticalStart);
                            });
                        }
                    }
                }

                return numFound;
            }

            public static int FindDirectionCurrent(InputBufferReader reader, Factory inputFactory, List<Combination> activeInputs) {
                int numFound = 0;
                reader.ResetCurrIndex();
                reader.SetReadIndex(-1);
                reader.ReadBuffer(out GameInputStruct curr);
                FightingGameInputCodeDir direction = curr.direction;
                DirectionCurrent input = AddToActiveInputs<DirectionCurrent>(activeInputs, inputFactory, reader.GetCurrentFrame(), reader.GetCurrentFrame(), newInput => {
                    numFound++;
                    newInput.Init(reader.GetCurrentFrame(), direction);
                });
                //FightingGameInputCodeDir direction = (FightingGameInputCodeDir) int.Parse(buffer[buffer.Length - 1].ToString());
                return numFound;
            }

            // regex = /(?<=([^5])[^5\1]{0,4}5{1,7})(?=[^5\1]{0,3}\1)/g
            public static int FindDoubleTaps(InputBufferReader reader, Factory inputFactory, List<Combination> activeInputs) {
                int numFound = 0;
                reader.ResetCurrIndex();
                FightingGameInputCodeDir currDir = FightingGameInputCodeDir.None;
                while (reader.ReadyNext()) {
                    int inputFrameIndex = reader.ReadBuffer(out GameInputStruct curr);

                    if (currDir != curr.direction) {
                        currDir = curr.direction;

                        FightingGameInputCodeDir direction = FightingGameInputCodeDir.None;
                        if (curr.direction != FightingGameInputCodeDir.Neutral) {
                            direction = curr.direction;

                            bool continueSearch = true;
                            int n = 0;
                            //FightingGameInputCodeDir prevDir = direction;

                            n = 0;
                            while (continueSearch && reader.ReadyNextLookBehind()) {
                                if (n < 5) {
                                    int lookBehindFrameIndex = reader.LookBehind(out GameInputStruct lb);

                                    if (lb.direction == FightingGameInputCodeDir.Neutral) break;
                                    ++n;
                                }
                                else {
                                    continueSearch = false;
                                    break;
                                }
                            }

                            n = 0;
                            while (continueSearch && reader.ReadyNextLookBehind()) {
                                if (n < 8) {
                                    int lookBehindFrameIndex = reader.LookBehind(out GameInputStruct lb);

                                    if (lb.direction != FightingGameInputCodeDir.Neutral) break;
                                    ++n;
                                }
                                else {
                                    continueSearch = false;
                                    break;
                                }
                            }

                            n = 0;
                            while (continueSearch && reader.ReadyNextLookBehind()) {
                                if (n < 4) {
                                    int lookBehindFrameIndex = reader.LookBehind(out GameInputStruct lb);

                                    if (lb.direction == direction) {
                                        DoubleTap input = AddToActiveInputs<DoubleTap>(activeInputs, inputFactory, inputFrameIndex, reader.GetCurrentFrame(), newInput => {
                                            numFound++;
                                            newInput.Init(inputFrameIndex, lookBehindFrameIndex, direction);
                                        });
                                    }
                                    ++n;
                                }
                                else {
                                    continueSearch = false;
                                    break;
                                }
                            }
                        }
                    }
                }
                return numFound;
            }

            public static int FindButtonPresses(InputBufferReader reader, Factory inputFactory, List<Combination> activeInputs) {
                int numFound = 0;
                reader.ResetCurrIndex();

                while (reader.ReadyNext()) {
                    int inputFrameIndex = reader.ReadBuffer(out GameInputStruct curr);

                    if (curr.butA) {
                        ButtonPress input = AddToActiveInputs<ButtonPress>(activeInputs, inputFactory, inputFrameIndex, reader.GetCurrentFrame(), newInput => {
                            numFound++;
                            newInput.Init(inputFrameIndex, FightingGameInputCodeBut.A);
                        });
                    }
                    if (curr.butB) {
                        ButtonPress input = AddToActiveInputs<ButtonPress>(activeInputs, inputFactory, inputFrameIndex, reader.GetCurrentFrame(), newInput => {
                            numFound++;
                            newInput.Init(inputFrameIndex, FightingGameInputCodeBut.B);
                        });
                    }
                    if (curr.butC) {
                        ButtonPress input = AddToActiveInputs<ButtonPress>(activeInputs, inputFactory, inputFrameIndex, reader.GetCurrentFrame(), newInput => {
                            numFound++;
                            newInput.Init(inputFrameIndex, FightingGameInputCodeBut.C);
                        });
                    }
                    if (curr.butD) {
                        ButtonPress input = AddToActiveInputs<ButtonPress>(activeInputs, inputFactory, inputFrameIndex, reader.GetCurrentFrame(), newInput => {
                            numFound++;
                            newInput.Init(inputFrameIndex, FightingGameInputCodeBut.D);
                        });
                    }
                    if (curr.butS) {
                        ButtonPress input = AddToActiveInputs<ButtonPress>(activeInputs, inputFactory, inputFrameIndex, reader.GetCurrentFrame(), newInput => {
                            numFound++;
                            newInput.Init(inputFrameIndex, FightingGameInputCodeBut.S);
                        });
                    }
                }

                return numFound;
            }

            public static int FindButton2Presses(InputBufferReader reader, Factory inputFactory, List<Combination> activeInputs) {
                int numFound = 0;
                reader.ResetCurrIndex();

                while (reader.ReadyNext()) {
                    int inputFrameIndex = reader.ReadBuffer(out GameInputStruct curr);

                    if (curr.butA) {
                        for (int n = 0; reader.ReadyNextLookBehind() && n < 2; ++n) {
                            int lookBehindFrameIndex = reader.LookBehind(out GameInputStruct lb);

                            if (lb.butB) AddToActiveInputs<Button2Press>(activeInputs, inputFactory, inputFrameIndex, reader.GetCurrentFrame(), newInput => {
                                numFound++;
                                newInput.Init(inputFrameIndex, FightingGameInputCodeBut.A, FightingGameInputCodeBut.B);
                            });

                            if (lb.butC) AddToActiveInputs<Button2Press>(activeInputs, inputFactory, inputFrameIndex, reader.GetCurrentFrame(), newInput => {
                                numFound++;
                                newInput.Init(inputFrameIndex, FightingGameInputCodeBut.A, FightingGameInputCodeBut.C);
                            });

                            if (lb.butD) AddToActiveInputs<Button2Press>(activeInputs, inputFactory, inputFrameIndex, reader.GetCurrentFrame(), newInput => {
                                numFound++;
                                newInput.Init(inputFrameIndex, FightingGameInputCodeBut.A, FightingGameInputCodeBut.D);
                            });

                            if (lb.butS) AddToActiveInputs<Button2Press>(activeInputs, inputFactory, inputFrameIndex, reader.GetCurrentFrame(), newInput => {
                                numFound++;
                                newInput.Init(inputFrameIndex, FightingGameInputCodeBut.A, FightingGameInputCodeBut.S);
                            });
                        }
                    }
                    if (curr.butB) {
                        for (int n = 0; reader.ReadyNextLookBehind() && n < 2; ++n) {
                            int lookBehindFrameIndex = reader.LookBehind(out GameInputStruct lb);

                            if (lb.butA) AddToActiveInputs<Button2Press>(activeInputs, inputFactory, inputFrameIndex, reader.GetCurrentFrame(), newInput => {
                                numFound++;
                                newInput.Init(inputFrameIndex, FightingGameInputCodeBut.A, FightingGameInputCodeBut.B);
                            });

                            if (lb.butC) AddToActiveInputs<Button2Press>(activeInputs, inputFactory, inputFrameIndex, reader.GetCurrentFrame(), newInput => {
                                numFound++;
                                newInput.Init(inputFrameIndex, FightingGameInputCodeBut.B, FightingGameInputCodeBut.C);
                            });

                            if (lb.butD) AddToActiveInputs<Button2Press>(activeInputs, inputFactory, inputFrameIndex, reader.GetCurrentFrame(), newInput => {
                                numFound++;
                                newInput.Init(inputFrameIndex, FightingGameInputCodeBut.B, FightingGameInputCodeBut.D);
                            });

                            if (lb.butS) AddToActiveInputs<Button2Press>(activeInputs, inputFactory, inputFrameIndex, reader.GetCurrentFrame(), newInput => {
                                numFound++;
                                newInput.Init(inputFrameIndex, FightingGameInputCodeBut.B, FightingGameInputCodeBut.S);
                            });
                        }
                    }

                    if (curr.butC) {
                        for (int n = 0; reader.ReadyNextLookBehind() && n < 2; ++n) {
                            int lookBehindFrameIndex = reader.LookBehind(out GameInputStruct lb);

                            if (lb.butA) AddToActiveInputs<Button2Press>(activeInputs, inputFactory, inputFrameIndex, reader.GetCurrentFrame(), newInput => {
                                numFound++;
                                newInput.Init(inputFrameIndex, FightingGameInputCodeBut.A, FightingGameInputCodeBut.C);
                            });

                            if (lb.butB) AddToActiveInputs<Button2Press>(activeInputs, inputFactory, inputFrameIndex, reader.GetCurrentFrame(), newInput => {
                                numFound++;
                                newInput.Init(inputFrameIndex, FightingGameInputCodeBut.B, FightingGameInputCodeBut.C);
                            });

                            if (lb.butD) AddToActiveInputs<Button2Press>(activeInputs, inputFactory, inputFrameIndex, reader.GetCurrentFrame(), newInput => {
                                numFound++;
                                newInput.Init(inputFrameIndex, FightingGameInputCodeBut.C, FightingGameInputCodeBut.D);
                            });

                            if (lb.butS) AddToActiveInputs<Button2Press>(activeInputs, inputFactory, inputFrameIndex, reader.GetCurrentFrame(), newInput => {
                                numFound++;
                                newInput.Init(inputFrameIndex, FightingGameInputCodeBut.C, FightingGameInputCodeBut.S);
                            });
                        }
                    }
                    if (curr.butD) {
                        for (int n = 0; reader.ReadyNextLookBehind() && n < 2; ++n) {
                            int lookBehindFrameIndex = reader.LookBehind(out GameInputStruct lb);

                            if (lb.butA) AddToActiveInputs<Button2Press>(activeInputs, inputFactory, inputFrameIndex, reader.GetCurrentFrame(), newInput => {
                                numFound++;
                                newInput.Init(inputFrameIndex, FightingGameInputCodeBut.A, FightingGameInputCodeBut.D);
                            });

                            if (lb.butB) AddToActiveInputs<Button2Press>(activeInputs, inputFactory, inputFrameIndex, reader.GetCurrentFrame(), newInput => {
                                numFound++;
                                newInput.Init(inputFrameIndex, FightingGameInputCodeBut.B, FightingGameInputCodeBut.D);
                            });

                            if (lb.butC) AddToActiveInputs<Button2Press>(activeInputs, inputFactory, inputFrameIndex, reader.GetCurrentFrame(), newInput => {
                                numFound++;
                                newInput.Init(inputFrameIndex, FightingGameInputCodeBut.C, FightingGameInputCodeBut.D);
                            });

                            if (lb.butS) AddToActiveInputs<Button2Press>(activeInputs, inputFactory, inputFrameIndex, reader.GetCurrentFrame(), newInput => {
                                numFound++;
                                newInput.Init(inputFrameIndex, FightingGameInputCodeBut.D, FightingGameInputCodeBut.S);
                            });
                        }
                    }
                    if (curr.butS) {
                        for (int n = 0; reader.ReadyNextLookBehind() && n < 2; ++n) {
                            int lookBehindFrameIndex = reader.LookBehind(out GameInputStruct lb);

                            if (lb.butA) AddToActiveInputs<Button2Press>(activeInputs, inputFactory, inputFrameIndex, reader.GetCurrentFrame(), newInput => {
                                numFound++;
                                newInput.Init(inputFrameIndex, FightingGameInputCodeBut.A, FightingGameInputCodeBut.S);
                            });

                            if (lb.butB) AddToActiveInputs<Button2Press>(activeInputs, inputFactory, inputFrameIndex, reader.GetCurrentFrame(), newInput => {
                                numFound++;
                                newInput.Init(inputFrameIndex, FightingGameInputCodeBut.B, FightingGameInputCodeBut.S);
                            });

                            if (lb.butC) AddToActiveInputs<Button2Press>(activeInputs, inputFactory, inputFrameIndex, reader.GetCurrentFrame(), newInput => {
                                numFound++;
                                newInput.Init(inputFrameIndex, FightingGameInputCodeBut.C, FightingGameInputCodeBut.S);
                            });

                            if (lb.butD) AddToActiveInputs<Button2Press>(activeInputs, inputFactory, inputFrameIndex, reader.GetCurrentFrame(), newInput => {
                                numFound++;
                                newInput.Init(inputFrameIndex, FightingGameInputCodeBut.D, FightingGameInputCodeBut.S);
                            });
                        }
                    }
                }

                return numFound;
            }

                // TODO: Accept 3 buttons at the same time.
            public static int FindButton3Presses(InputBufferReader reader, Factory inputFactory, List<Combination> activeInputs) { return 0; }

            public static int FindDirectionPlusButtons(InputBufferReader reader, Factory inputFactory, List<Combination> activeInputs) {
                int numFound = 0;
                reader.ResetCurrIndex();

                while (reader.ReadyNext()) {
                    int inputFrameIndex = reader.ReadBuffer(out GameInputStruct curr);

                    if (curr.direction != FightingGameInputCodeDir.Neutral) {
                        for (int n = 0; reader.ReadyNextLookBehind() && n < 3; ++n) {
                            int lookBehindFrameIndex = reader.LookBehind(out GameInputStruct lb);

                            if (lb.butA) AddToActiveInputs<DirectionPlusButton>(activeInputs, inputFactory, inputFrameIndex, reader.GetCurrentFrame(), newInput => {
                                numFound++;
                                newInput.Init(inputFrameIndex, FightingGameInputCodeBut.A, curr.direction);
                            });

                            if (lb.butB) AddToActiveInputs<DirectionPlusButton>(activeInputs, inputFactory, inputFrameIndex, reader.GetCurrentFrame(), newInput => {
                                numFound++;
                                newInput.Init(inputFrameIndex, FightingGameInputCodeBut.B, curr.direction);
                            });

                            if (lb.butC) AddToActiveInputs<DirectionPlusButton>(activeInputs, inputFactory, inputFrameIndex, reader.GetCurrentFrame(), newInput => {
                                numFound++;
                                newInput.Init(inputFrameIndex, FightingGameInputCodeBut.C, curr.direction);
                            });

                            if (lb.butD) AddToActiveInputs<DirectionPlusButton>(activeInputs, inputFactory, inputFrameIndex, reader.GetCurrentFrame(), newInput => {
                                numFound++;
                                newInput.Init(inputFrameIndex, FightingGameInputCodeBut.D, curr.direction);
                            });

                            if (lb.butS) AddToActiveInputs<DirectionPlusButton>(activeInputs, inputFactory, inputFrameIndex, reader.GetCurrentFrame(), newInput => {
                                numFound++;
                                newInput.Init(inputFrameIndex, FightingGameInputCodeBut.S, curr.direction);
                            });
                        }
                    }
                }
                return numFound;
            }

            private static FightingGameInputCodeDir[] qcLeft = { FightingGameInputCodeDir.Down, FightingGameInputCodeDir.DownLeft, FightingGameInputCodeDir.Left };
            private static FightingGameInputCodeDir[] qcRight = { FightingGameInputCodeDir.Down, FightingGameInputCodeDir.DownRight, FightingGameInputCodeDir.Right };
            private static int[] qcSearchLength = { 5, 4 };

            private static int FindMotion(GameInputStruct curr, InputBufferReader reader, FightingGameInputCodeDir[] motion, int[] searchLength) {
                if (curr.direction == motion[0]) {
                    for (int motionIndex = 1; motionIndex < motion.Length; ++motionIndex) {
                        bool stopSearch = true;
                        for (int n = 0; reader.ReadyNextLookAhead() && n < searchLength[motionIndex - 1]; ++n) {
                            int lookAheadFrameIndex = reader.LookAhead(out GameInputStruct la);
                            if (la.direction == motion[motionIndex]) {
                                if (motionIndex == motion.Length - 1) {
                                    return lookAheadFrameIndex;
                                }
                                else {
                                    stopSearch = false;
                                    break;
                                }
                            }
                        }
                        if (stopSearch) return -1;
                    }
                }
                return -1;
            }

            public static int FindQuarterCircles(InputBufferReader reader, Factory inputFactory, List<Combination> activeInputs) {
                int numFound = 0;
                reader.ResetCurrIndex();
                FightingGameInputCodeDir currDir = FightingGameInputCodeDir.None;
                while (reader.ReadyNext()) {
                    int inputFrameIndex = reader.ReadBuffer(out GameInputStruct curr);

                    if (currDir != curr.direction) {
                        currDir = curr.direction;
                        int lookAheadFrameIndex;

                        reader.ResetLookAhead();
                        if ((lookAheadFrameIndex = FindMotion(curr, reader, qcLeft, qcSearchLength)) >= 0) {
                            QuarterCircle input = AddToActiveInputs<QuarterCircle>(activeInputs, inputFactory, lookAheadFrameIndex, reader.GetCurrentFrame(), newInput => {
                                numFound++;
                                newInput.Init(lookAheadFrameIndex, FightingGameInputCodeDir.Left);
                            });
                        }

                        reader.ResetLookAhead();
                        if ((lookAheadFrameIndex = FindMotion(curr, reader, qcRight, qcSearchLength)) >= 0) {
                            QuarterCircle input = AddToActiveInputs<QuarterCircle>(activeInputs, inputFactory, lookAheadFrameIndex, reader.GetCurrentFrame(), newInput => {
                                numFound++;
                                newInput.Init(lookAheadFrameIndex, FightingGameInputCodeDir.Right);
                            });
                        }
                    }
                }
                return numFound;
            }

            public static int FindQuarterCircleButtonPresses(InputBufferReader reader, Factory inputFactory, List<Combination> activeInputs) {
                int numFound = 0;
                reader.ResetCurrIndex();

                List<Combination> quarterCircles = activeInputs.FindAll(combo => {
                    return combo.GetType() == typeof(QuarterCircle);
                });

                foreach (QuarterCircle qc in quarterCircles) {
                    reader.SetReadIndex(reader.GetCurrentFrame() - qc.GetFrame());

                    for (int n = 0; n < 3 && reader.ReadyNextLookBehind(); ++n) {
                        int lookBehindFrameIndex = reader.LookBehind(out GameInputStruct lb);

                        if (lb.butA) AddToActiveInputs<DirectionPlusButton>(activeInputs, inputFactory, qc.GetFrame(), reader.GetCurrentFrame(), newInput => {
                            numFound++;
                            newInput.Init(qc.GetFrame(), FightingGameInputCodeBut.A, qc.endDirection);
                        });

                        if (lb.butB) AddToActiveInputs<DirectionPlusButton>(activeInputs, inputFactory, qc.GetFrame(), reader.GetCurrentFrame(), newInput => {
                            numFound++;
                            newInput.Init(qc.GetFrame(), FightingGameInputCodeBut.B, qc.endDirection);
                        });

                        if (lb.butC) AddToActiveInputs<DirectionPlusButton>(activeInputs, inputFactory, qc.GetFrame(), reader.GetCurrentFrame(), newInput => {
                            numFound++;
                            newInput.Init(qc.GetFrame(), FightingGameInputCodeBut.C, qc.endDirection);
                        });

                        if (lb.butD) AddToActiveInputs<DirectionPlusButton>(activeInputs, inputFactory, qc.GetFrame(), reader.GetCurrentFrame(), newInput => {
                            numFound++;
                            newInput.Init(qc.GetFrame(), FightingGameInputCodeBut.D, qc.endDirection);
                        });

                        if (lb.butS) AddToActiveInputs<DirectionPlusButton>(activeInputs, inputFactory, qc.GetFrame(), reader.GetCurrentFrame(), newInput => {
                            numFound++;
                            newInput.Init(qc.GetFrame(), FightingGameInputCodeBut.S, qc.endDirection);
                        });
                    }

                    for (int n = 0; n < 3 && reader.ReadyNextLookAhead(); ++n) {
                        int lookAheadFrameIndex = reader.LookAhead(out GameInputStruct la);

                        if (la.butA) AddToActiveInputs<DirectionPlusButton>(activeInputs, inputFactory, lookAheadFrameIndex, reader.GetCurrentFrame(), newInput => {
                            numFound++;
                            newInput.Init(lookAheadFrameIndex, FightingGameInputCodeBut.A, qc.endDirection);
                        });

                        if (la.butB) AddToActiveInputs<DirectionPlusButton>(activeInputs, inputFactory, lookAheadFrameIndex, reader.GetCurrentFrame(), newInput => {
                            numFound++;
                            newInput.Init(lookAheadFrameIndex, FightingGameInputCodeBut.B, qc.endDirection);
                        });

                        if (la.butC) AddToActiveInputs<DirectionPlusButton>(activeInputs, inputFactory, lookAheadFrameIndex, reader.GetCurrentFrame(), newInput => {
                            numFound++;
                            newInput.Init(lookAheadFrameIndex, FightingGameInputCodeBut.C, qc.endDirection);
                        });

                        if (la.butD) AddToActiveInputs<DirectionPlusButton>(activeInputs, inputFactory, lookAheadFrameIndex, reader.GetCurrentFrame(), newInput => {
                            numFound++;
                            newInput.Init(lookAheadFrameIndex, FightingGameInputCodeBut.D, qc.endDirection);
                        });

                        if (la.butS) AddToActiveInputs<DirectionPlusButton>(activeInputs, inputFactory, lookAheadFrameIndex, reader.GetCurrentFrame(), newInput => {
                            numFound++;
                            newInput.Init(lookAheadFrameIndex, FightingGameInputCodeBut.S, qc.endDirection);
                        });
                    }
                }

                return numFound;
            }
        }
    }
}