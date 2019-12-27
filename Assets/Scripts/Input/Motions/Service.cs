using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Input.Combinations;

namespace ResonantSpark {
    namespace Input {
        public class Service {
            public static void FindCombinations(InputBufferReader reader, Factory inputFactory, int frameIndex, List<Combination> activeInputs) {
                int test0 = FindDoubleDirectionTaps(reader, inputFactory, frameIndex, activeInputs);
                int test1 = FindDirectionPresses(reader, inputFactory, frameIndex, activeInputs);
                int test2 = FindNeutralReturns(reader, inputFactory, frameIndex, activeInputs);
                int test3 = FindDirectionCurrent(reader, inputFactory, frameIndex, activeInputs);
                int test4 = FindDirectionLongHolds(reader, inputFactory, frameIndex, activeInputs);
                int test5 = FindDoubleTaps(reader, inputFactory, frameIndex, activeInputs);
                int test6 = FindButtonPresses(reader, inputFactory, frameIndex, activeInputs);
                int test7 = FindButton2Presses(reader, inputFactory, frameIndex, activeInputs);
                int test8 = FindButton3Presses(reader, inputFactory, frameIndex, activeInputs);
                int test9 = FindDirectionPlusButtons(reader, inputFactory, frameIndex, activeInputs);
                int testA = FindQuarterCircles(reader, inputFactory, frameIndex, activeInputs);
                int testB = FindQuarterCircleButtonPresses(reader, inputFactory, frameIndex, activeInputs);

                int x = test0 + test1 + test2 + test3 + test4 + test5 + test6 + test7 + test8 + test9 + testA + testB;
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

            public static int FindNeutralReturns(InputBufferReader reader, Factory inputFactory, int frameIndex, List<Combination> activeInputs) {
                int numFound = 0;
                reader.ResetCurrIndex();

                bool notNeutral = false;
                while (reader.IsReadable()) {
                    int inputFrameIndex = reader.ReadBuffer(out GameInputStruct curr);

                    if (curr.direction != FightingGameInputCodeDir.Neutral) {
                        notNeutral = true;
                    }
                    else if (notNeutral) {
                        AddToActiveInputs<NeutralReturn>(activeInputs, inputFactory, inputFrameIndex, frameIndex, newInput => {
                            numFound++;
                            newInput.Init(inputFrameIndex);
                        });
                        notNeutral = false;
                    }
                }

                return numFound;
            }

            public static int FindDirectionPresses(InputBufferReader reader, Factory inputFactory, int frameIndex, List<Combination> activeInputs) {
                int numFound = 0;
                //foreach (Match match in rgxDirectionPresses.Matches(buffer)) {
                //    int inputFrameIndex = frameIndex - (buffer.Length - match.Index);
                //    //Match local = match;
                //    //GroupCollection groups = match.Groups;
                //    //Debug.Log("Direction Press: " + local + " @ (lcl: " + match.Index + ", gbl: " + (frameIndex - (buffer.Length - match.Index)) + ") with " + groups.Count + " groups");
                //    //Debug.Log(groups[2].Value + " @ " + groups[2].Index);
                //    DirectionPress input = AddToActiveInputs<DirectionPress>(activeInputs, inputFactory, inputFrameIndex, frameIndex, (newInput) => {
                //        numFound++;
                //        newInput.Init(inputFrameIndex, (FightingGameInputCodeDir) int.Parse(match.Groups[2].Value));
                //    });
                //}
                return numFound;
            }

                // This requires a hold of 20 frames
            public static int FindDirectionLongHolds(InputBufferReader reader, Factory inputFactory, int frameIndex, List<Combination> activeInputs) {
                int numFound = 0;
                //foreach (Match match in rgxDirectionHolds.Matches(buffer)) {
                //    int inputFrameIndex = frameIndex - (buffer.Length - match.Index);
                //    //Match local = match;
                //    //GroupCollection groups = match.Groups;
                //    //Debug.Log("Direction Hold: " + local + " @ (lcl: " + match.Index + ", gbl: " + inputFrameIndex + ") with " + groups.Count + " groups");
                //    //Debug.Log(groups[1].Value + " @ " + groups[1].Index);
                //    FightingGameInputCodeDir direction = (FightingGameInputCodeDir) int.Parse(match.Groups[1].Value);
                //    DirectionLongHold input = AddToActiveInputs<DirectionLongHold>(activeInputs, inputFactory, inputFrameIndex + 19, frameIndex, (newInput) => {
                //        numFound++;
                //        newInput.Init(inputFrameIndex + 19, direction, match.Value.Length);
                //    });
                //}
                return numFound;
            }

            public static int FindDirectionCurrent(InputBufferReader reader, Factory inputFactory, int frameIndex, List<Combination> activeInputs) {
                int numFound = 0;
                //FightingGameInputCodeDir direction = (FightingGameInputCodeDir) int.Parse(buffer[buffer.Length - 1].ToString());
                //DirectionCurrent input = AddToActiveInputs<DirectionCurrent>(activeInputs, inputFactory, frameIndex, frameIndex, (newInput) => {
                //    numFound++;
                //    newInput.Init(frameIndex, direction);
                //});
                return numFound;
            }

            public static int FindDoubleDirectionTaps(InputBufferReader reader, Factory inputFactory, int frameIndex, List<Combination> activeInputs) {
                int numFound = 0;
                //foreach (Match match in rgxDoubleDirectionTaps.Matches(buffer)) {
                //    int relativeFrameGapStart = match.Groups[2].Index;
                //    int relativeFrameGapEnd = match.Index;
                //    //Match local = match;
                //    //GroupCollection groups = match.Groups;
                //    //Debug.Log("Double Tap Direction: " + local + " @ (lcl: " + match.Index + ", gbl: " + (frameIndex - (buffer.Length - relativeFrameGapEnd)) + ") with " + groups.Count + " groups");
                //    //Debug.Log(groups[0].Value + " @ " + groups[0].Index);
                //    //Debug.Log(groups[1].Value + " @ " + groups[1].Index);
                //    //Debug.Log(groups[2].Value + " @ " + groups[2].Index);
                //    FightingGameInputCodeDir direction = (FightingGameInputCodeDir) int.Parse(match.Groups[1].Value);
                //    DoubleTap input = AddToActiveInputs<DoubleTap>(activeInputs, inputFactory, frameIndex - (buffer.Length - relativeFrameGapEnd), frameIndex, (newInput) => {
                //        numFound++;
                //        newInput.Init(frameIndex - (buffer.Length - relativeFrameGapEnd), frameIndex - (buffer.Length - relativeFrameGapStart), direction);
                //    });
                //}
                return numFound;
            }

            public static int FindDoubleTaps(InputBufferReader reader, Factory inputFactory, int frameIndex, List<Combination> activeInputs) {
                return 0;
            }

            public static int FindButtonPresses(InputBufferReader reader, Factory inputFactory, int frameIndex, List<Combination> activeInputs) { return 0; }
            public static int FindButton2Presses(InputBufferReader reader, Factory inputFactory, int frameIndex, List<Combination> activeInputs) { return 0; }
            public static int FindButton3Presses(InputBufferReader reader, Factory inputFactory, int frameIndex, List<Combination> activeInputs) { return 0; }
            public static int FindDirectionPlusButtons(InputBufferReader reader, Factory inputFactory, int frameIndex, List<Combination> activeInputs) { return 0; }
            public static int FindQuarterCircles(InputBufferReader reader, Factory inputFactory, int frameIndex, List<Combination> activeInputs) { return 0; }
            public static int FindQuarterCircleButtonPresses(InputBufferReader reader, Factory inputFactory, int frameIndex, List<Combination> activeInputs) { return 0; }
        }
    }
}