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
                //int test4 = FindDirectionLongHolds(reader, inputFactory, activeInputs);   // TODO: Fix this, but the feature may not even exist.
                int test6 = FindButtonPresses(reader, inputFactory, activeInputs);
                int test7 = FindButton2Presses(reader, inputFactory, activeInputs);
                //int test8 = FindButton3Presses(reader, inputFactory, activeInputs);       // TODO: Fix this, but the feature may not even exist.
                int testC = FindButtonReleases(reader, inputFactory, activeInputs);
                int test9 = FindDirectionPlusButtons(reader, inputFactory, activeInputs);
                int testA = FindQuarterCircles(reader, inputFactory, activeInputs);
                int testB = FindQuarterCircleButtonPresses(reader, inputFactory, activeInputs);

                //int x = test0 + test1 + test2 + test3 + test4 + test6 + test7 + test8 + test9 + testA + testB;
                activeInputs.Sort();
            }

            private static T_Combo AddToActiveInputs<T_Combo>(List<Combination> activeInputs, Factory inputFactory, int frameCurrent, Action<T_Combo> initCallback) where T_Combo : Combination, new() {
                T_Combo newInput = inputFactory.CreateCombination<T_Combo>(frameCurrent);
                initCallback.Invoke(newInput);
                foreach (var cmb in activeInputs) {
                    if (cmb.Equals(newInput)) return (T_Combo) cmb;
                }
                activeInputs.Add(newInput);
                return newInput;
            }

            private static int ButtonCodeToIndex(FightingGameInputCodeBut buttonCode) {
                int buttonIndex = 0;
                int number = (int) buttonCode;
                while (number > 0b00000001) {
                    number >>= 1;
                    buttonIndex++;
                }
                return buttonIndex;
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
                        AddToActiveInputs<NeutralReturn>(activeInputs, inputFactory, reader.currentFrame, newInput => {
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
                        AddToActiveInputs<DirectionPress>(activeInputs, inputFactory, reader.currentFrame, newInput => {
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
                            DirectionLongHold input = AddToActiveInputs<DirectionLongHold>(activeInputs, inputFactory, reader.currentFrame, (newInput) => {
                                numFound++;
                                newInput.Init(inputFrameIndex, FightingGameInputCodeDir.Neutral + horizontalHold, inputFrameIndex - horizontalStart);
                            });
                        }

                        if (inputFrameIndex - verticalStart >= 40) {
                            DirectionLongHold input = AddToActiveInputs<DirectionLongHold>(activeInputs, inputFactory, reader.currentFrame, (newInput) => {
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
                DirectionCurrent input = AddToActiveInputs<DirectionCurrent>(activeInputs, inputFactory, reader.currentFrame, newInput => {
                    numFound++;
                    newInput.Init(reader.currentFrame, direction);
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
                                        DoubleTap input = AddToActiveInputs<DoubleTap>(activeInputs, inputFactory, reader.currentFrame, newInput => {
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

            private static void FindSingleButtonRelease(bool butAPrev, bool butBPrev, bool butCPrev, bool butDPrev, bool butSPrev, GameInputStruct curr, FightingGameInputCodeBut ignoreBut, Action<FightingGameInputCodeBut> callback) {
                FindSingleButtonPress(!butAPrev, !butBPrev, !butCPrev, !butDPrev, !butSPrev, !curr, ignoreBut, callback);
            }

            private static void FindSingleButtonPress(bool butAPrev, bool butBPrev, bool butCPrev, bool butDPrev, bool butSPrev, GameInputStruct curr, FightingGameInputCodeBut ignoreBut, Action<FightingGameInputCodeBut> callback) {
                if (ignoreBut != FightingGameInputCodeBut.A && !butAPrev && curr.butA) callback.Invoke(FightingGameInputCodeBut.A);
                if (ignoreBut != FightingGameInputCodeBut.B && !butBPrev && curr.butB) callback.Invoke(FightingGameInputCodeBut.B);
                if (ignoreBut != FightingGameInputCodeBut.C && !butCPrev && curr.butC) callback.Invoke(FightingGameInputCodeBut.C);
                if (ignoreBut != FightingGameInputCodeBut.D && !butDPrev && curr.butD) callback.Invoke(FightingGameInputCodeBut.D);
                if (ignoreBut != FightingGameInputCodeBut.S && !butSPrev && curr.butS) callback.Invoke(FightingGameInputCodeBut.S);
            }

            public static int FindButtonPresses(InputBufferReader reader, Factory inputFactory, List<Combination> activeInputs) {
                int numFound = 0;
                reader.ResetCurrIndex();

                bool butAPrev = true;
                bool butBPrev = true;
                bool butCPrev = true;
                bool butDPrev = true;
                bool butSPrev = true;

                while (reader.ReadyNext()) {
                    int inputFrameIndex = reader.ReadBuffer(out GameInputStruct curr);

                    FindSingleButtonPress(butAPrev, butBPrev, butCPrev, butDPrev, butSPrev, curr, ignoreBut: FightingGameInputCodeBut.None, callback: buttonCode => {
                        AddToActiveInputs<ButtonPress>(activeInputs, inputFactory, reader.currentFrame, newInput => {
                            numFound++;
                            newInput.Init(inputFrameIndex, buttonCode);
                        });
                    });

                    butAPrev = curr.butA;
                    butBPrev = curr.butB;
                    butCPrev = curr.butC;
                    butDPrev = curr.butD;
                    butSPrev = curr.butS;
                }

                return numFound;
            }

            public static int FindButtonReleases(InputBufferReader reader, Factory inputFactory, List<Combination> activeInputs) {
                int numFound = 0;
                reader.ResetCurrIndex();

                bool butAPrev = false;
                bool butBPrev = false;
                bool butCPrev = false;
                bool butDPrev = false;
                bool butSPrev = false;

                while (reader.ReadyNext()) {
                    int inputFrameIndex = reader.ReadBuffer(out GameInputStruct curr);

                    FindSingleButtonRelease(butAPrev, butBPrev, butCPrev, butDPrev, butSPrev, curr, ignoreBut: FightingGameInputCodeBut.None, callback: buttonCode => {
                        AddToActiveInputs<ButtonRelease>(activeInputs, inputFactory, reader.currentFrame, newInput => {
                            numFound++;
                            newInput.Init(inputFrameIndex, buttonCode);
                        });
                    });

                    butAPrev = curr.butA;
                    butBPrev = curr.butB;
                    butCPrev = curr.butC;
                    butDPrev = curr.butD;
                    butSPrev = curr.butS;
                }

                return numFound;
            }

            public static int FindButton2Presses(InputBufferReader reader, Factory inputFactory, List<Combination> activeInputs) {
                int numFound = 0;
                reader.ResetCurrIndex();

                bool butAPrev = true;
                bool butBPrev = true;
                bool butCPrev = true;
                bool butDPrev = true;
                bool butSPrev = true;

                while (reader.ReadyNext()) {
                    int inputFrameIndex = reader.ReadBuffer(out GameInputStruct curr);

                    FindSingleButtonPress(butAPrev, butBPrev, butCPrev, butDPrev, butSPrev, curr, ignoreBut: FightingGameInputCodeBut.None, callback: buttonCode0 => {
                        FindSingleButtonPress(butAPrev, butBPrev, butCPrev, butDPrev, butSPrev, curr, ignoreBut: buttonCode0, callback: buttonCode1 => {
                            AddToActiveInputs<Button2Press>(activeInputs, inputFactory, reader.currentFrame, newInput => {
                                numFound++;
                                newInput.Init(inputFrameIndex, buttonCode1, buttonCode0);
                            });
                        });

                        for (int n = 0; n < 2 && reader.ReadyNextLookBehind(); ++n) {
                            int inputLookBehindFrameIndex = reader.LookBehind(out GameInputStruct lb);

                            // If you both reverse the order of the 2 frames AND negate both values, it's the same as checking in the correct order
                            FindSingleButtonPress(!butAPrev, !butBPrev, !butCPrev, !butDPrev, !butSPrev, !lb, ignoreBut: buttonCode0, callback: buttonCode1 => {
                                AddToActiveInputs<Button2Press>(activeInputs, inputFactory, reader.currentFrame, newInput => {
                                    numFound++;
                                    //newInput.Init(inputLookBehindFrameIndex + 1, buttonCode0, buttonCode1); // This uses the button press as the trigger
                                    newInput.Init(inputFrameIndex, buttonCode0, buttonCode1);                 // This uses the end of the quarter circle as the trigger
                                });
                            });

                            butAPrev = lb.butA;
                            butBPrev = lb.butB;
                            butCPrev = lb.butC;
                            butDPrev = lb.butD;
                            butSPrev = lb.butS;
                        }
                    });

                    butAPrev = curr.butA;
                    butBPrev = curr.butB;
                    butCPrev = curr.butC;
                    butDPrev = curr.butD;
                    butSPrev = curr.butS;
                }

                return numFound;
            }

            // TODO: Accept 3 buttons at the same time.
            public static int FindButton3Presses(InputBufferReader reader, Factory inputFactory, List<Combination> activeInputs) { return 0; }

            public static int FindDirectionPlusButtons(InputBufferReader reader, Factory inputFactory, List<Combination> activeInputs) {
                int numFound = 0;
                reader.ResetCurrIndex();

                List<Combination> buttonPresses = activeInputs.FindAll(combo => {
                    return combo.GetType() == typeof(ButtonPress);
                });

                foreach (ButtonPress bp in buttonPresses) {
                    bool continueSearch = false;
                    reader.SetReadIndex(-(reader.currentFrame - bp.GetFrame()));

                    int inputFrameIndex = reader.ReadBuffer(out GameInputStruct curr);

                    //for (int n = 0; n < 3 && reader.ReadyNextLookBehind(); ++n) {
                    //    // I'm NOT going to do a look behind for the direction. The frame of the button press is ALL that matters.
                    //}

                    for (int n = 0; n < 5 && reader.ReadyNextLookAhead(); ++n) {
                        int lookAheadFrameIndex = reader.LookAhead(out GameInputStruct la);

                        if (la.direction != curr.direction) {
                            continueSearch = true;
                            break;
                        }
                    }

                    if (!continueSearch && curr.direction != FightingGameInputCodeDir.Neutral) {
                        AddToActiveInputs<DirectionPlusButton>(activeInputs, inputFactory, reader.currentFrame, newInput => {
                            numFound++;
                            newInput.Init(bp.GetFrame(), bp.button0, curr.direction);
                        });
                    }
                    else {
                        reader.ResetLookAhead();
                        int lookAheadFrameIndex = bp.GetFrame();
                        int holdLength = 0;
                        FightingGameInputCodeDir holdDir = curr.direction;
                        for (int n = 0; n < 15 && reader.ReadyNextLookAhead(); ++n) {
                            lookAheadFrameIndex = reader.LookAhead(out GameInputStruct la);

                            if (la.direction == holdDir) {
                                holdLength++;
                                if (holdLength >= 5) {
                                    continueSearch = false;
                                    break;
                                }
                            }
                            else {
                                holdDir = la.direction;
                                holdLength = 0;
                            }
                        }

                        if (!continueSearch && holdDir != FightingGameInputCodeDir.Neutral) {
                            AddToActiveInputs<DirectionPlusButton>(activeInputs, inputFactory, reader.currentFrame, newInput => {
                                numFound++;
                                newInput.Init(lookAheadFrameIndex, bp.button0, holdDir);
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
                            QuarterCircle input = AddToActiveInputs<QuarterCircle>(activeInputs, inputFactory, reader.currentFrame, newInput => {
                                numFound++;
                                newInput.Init(lookAheadFrameIndex, FightingGameInputCodeDir.Left);
                            });
                        }

                        reader.ResetLookAhead();
                        if ((lookAheadFrameIndex = FindMotion(curr, reader, qcRight, qcSearchLength)) >= 0) {
                            QuarterCircle input = AddToActiveInputs<QuarterCircle>(activeInputs, inputFactory, reader.currentFrame, newInput => {
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
                    reader.SetReadIndex(-(reader.currentFrame - qc.GetFrame()) - 1);
                    reader.ReadBuffer(out GameInputStruct curr);

                    bool butAPrev = curr.butA;
                    bool butBPrev = curr.butB;
                    bool butCPrev = curr.butC;
                    bool butDPrev = curr.butD;
                    bool butSPrev = curr.butS;

                    for (int n = 0; n < 8 && reader.ReadyNextLookAhead(); ++n) {
                        int inputFrameIndex = reader.LookAhead(out GameInputStruct la);

                        FindSingleButtonPress(butAPrev, butBPrev, butCPrev, butDPrev, butSPrev, la, ignoreBut: FightingGameInputCodeBut.None, callback: buttonCode => {
                            AddToActiveInputs<QuarterCircleButtonPress>(activeInputs, inputFactory, reader.currentFrame, newInput => {
                                numFound++;
                                newInput.Init(inputFrameIndex, qc.endDirection, buttonCode);
                            });
                        });

                        butAPrev = la.butA;
                        butBPrev = la.butB;
                        butCPrev = la.butC;
                        butDPrev = la.butD;
                        butSPrev = la.butS;
                    }

                    butAPrev = curr.butA;
                    butBPrev = curr.butB;
                    butCPrev = curr.butC;
                    butDPrev = curr.butD;
                    butSPrev = curr.butS;

                    for (int n = 0; n < 8 && reader.ReadyNextLookBehind(); ++n) {
                        int inputFrameIndex = reader.LookBehind(out GameInputStruct lb);

                        // If you both reverse the order of the 2 frames AND negate both values, it's the same as checking in the correct order
                        FindSingleButtonPress(!butAPrev, !butBPrev, !butCPrev, !butDPrev, !butSPrev, !lb, ignoreBut: FightingGameInputCodeBut.None, callback: buttonCode => {
                            AddToActiveInputs<QuarterCircleButtonPress>(activeInputs, inputFactory, reader.currentFrame, newInput => {
                                numFound++;
                                //newInput.Init(inputFrameIndex + 1, qc.endDirection, buttonCode); // This uses the button press as the trigger
                                newInput.Init(qc.GetFrame(), qc.endDirection, buttonCode);         // This uses the end of the quarter circle as the trigger
                            });
                        });

                        butAPrev = lb.butA;
                        butBPrev = lb.butB;
                        butCPrev = lb.butC;
                        butDPrev = lb.butD;
                        butSPrev = lb.butS;
                    }
                }

                return numFound;
            }
        }
    }
}