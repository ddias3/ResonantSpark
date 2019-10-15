using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Input.Combinations;

namespace ResonantSpark {
    namespace Input {
        public class Service {
            public static void FindCombinations(string buffer, Factory inputFactory, int frameIndex, List<Combination> activeInputs) {
                int test0 = FindDoubleDirectionTaps(buffer, inputFactory, frameIndex, activeInputs);
                int test1 = FindDirectionPresses(buffer, inputFactory, frameIndex, activeInputs);
                int test2 = FindNeutralReturns(buffer, inputFactory, frameIndex, activeInputs);
                int test3 = FindDirectionHolds(buffer, inputFactory, frameIndex, activeInputs);

                int x = test0 + test1 + test2; // + test3;
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

            private static readonly Regex rgxNeutralReturn = new Regex(@"(?<=([^5]))(?=[5])", RegexOptions.ECMAScript | RegexOptions.Compiled);
            public static int FindNeutralReturns(string buffer, Factory inputFactory, int frameIndex, List<Combination> activeInputs) {
                int numFound = 0;
                foreach (Match match in rgxNeutralReturn.Matches(buffer)) {
                    int inputFrameIndex = frameIndex - (buffer.Length - match.Index);
                    //Match local = match;
                    //GroupCollection groups = match.Groups;
                    //Debug.Log("Neutral Return: " + local + " @ (lcl: " + match.Index + ", gbl: " + (frameIndex - (buffer.Length - match.Index)) + ") with " + groups.Count + " groups");
                    //Debug.Log(groups[1].Value + " @ " + groups[1].Index);
                    NeutralReturn input = AddToActiveInputs<NeutralReturn>(activeInputs, inputFactory, inputFrameIndex, frameIndex, (newInput) => {
                        numFound++;
                        newInput.Init(inputFrameIndex);
                    });
                    numFound++;
                }
                return numFound;
            }

            private static readonly Regex rgxDirectionPresses = new Regex(@"(?<=([1-9]))(?=([^5]))(?!\1)", RegexOptions.ECMAScript | RegexOptions.Compiled);
            public static int FindDirectionPresses(string buffer, Factory inputFactory, int frameIndex, List<Combination> activeInputs) {
                int numFound = 0;
                foreach (Match match in rgxDirectionPresses.Matches(buffer)) {
                    int inputFrameIndex = frameIndex - (buffer.Length - match.Index);
                    //Match local = match;
                    //GroupCollection groups = match.Groups;
                    //Debug.Log("Direction Press: " + local + " @ (lcl: " + match.Index + ", gbl: " + (frameIndex - (buffer.Length - match.Index)) + ") with " + groups.Count + " groups");
                    //Debug.Log(groups[2].Value + " @ " + groups[2].Index);
                    DirectionPress input = AddToActiveInputs<DirectionPress>(activeInputs, inputFactory, inputFrameIndex, frameIndex, (newInput) => {
                        numFound++;
                        newInput.Init(inputFrameIndex, (FightingGameInputCodeDir) int.Parse(match.Groups[2].Value));
                    });
                }
                return numFound;
            }

                // This requires a hold of 20 frames
            private static readonly Regex rgxDirectionHolds = new Regex(@"(?<=([1-9]))(?=([^5])\2{19,})(?!\1)|^([^5])\3+$", RegexOptions.ECMAScript | RegexOptions.Compiled);
            public static int FindDirectionHolds(string buffer, Factory inputFactory, int frameIndex, List<Combination> activeInputs) {
                int numFound = 0;
                foreach (Match match in rgxDirectionHolds.Matches(buffer)) {
                    int inputFrameIndex = frameIndex - (buffer.Length - match.Index);
                    //Match local = match;
                    //GroupCollection groups = match.Groups;
                    //Debug.Log("Direction Hold: " + local + " @ (lcl: " + match.Index + ", gbl: " + inputFrameIndex + ") with " + groups.Count + " groups");
                    //Debug.Log(groups[1].Value + " @ " + groups[1].Index);
                    FightingGameInputCodeDir direction = (FightingGameInputCodeDir) int.Parse(match.Groups[1].Value);
                    DirectionHold input = AddToActiveInputs<DirectionHold>(activeInputs, inputFactory, inputFrameIndex + 19, frameIndex, (newInput) => {
                        numFound++;
                        newInput.Init(inputFrameIndex + 19, direction, match.Value.Length);
                    });
                }
                return numFound;
            }

            private static readonly Regex rgxDoubleDirectionTaps = new Regex(@"(?<=([^5])[^5\1]{0,4}5{1,7})(?=[^5\1]{0,3}\1)", RegexOptions.ECMAScript | RegexOptions.Compiled);
            public static int FindDoubleDirectionTaps(string buffer, Factory inputFactory, int frameIndex, List<Combination> activeInputs) {
                int numFound = 0;
                foreach (Match match in rgxDoubleDirectionTaps.Matches(buffer)) {
                    int relativeFrameGapStart = match.Groups[2].Index;
                    int relativeFrameGapEnd = match.Index;
                    //Match local = match;
                    //GroupCollection groups = match.Groups;
                    //Debug.Log("Double Tap Direction: " + local + " @ (lcl: " + match.Index + ", gbl: " + (frameIndex - (buffer.Length - relativeFrameGapEnd)) + ") with " + groups.Count + " groups");
                    //Debug.Log(groups[0].Value + " @ " + groups[0].Index);
                    //Debug.Log(groups[1].Value + " @ " + groups[1].Index);
                    //Debug.Log(groups[2].Value + " @ " + groups[2].Index);
                    FightingGameInputCodeDir direction = (FightingGameInputCodeDir) int.Parse(match.Groups[1].Value);
                    DoubleTap input = AddToActiveInputs<DoubleTap>(activeInputs, inputFactory, frameIndex - (buffer.Length - relativeFrameGapEnd), frameIndex, (newInput) => {
                        numFound++;
                        newInput.Init(frameIndex - (buffer.Length - relativeFrameGapEnd), frameIndex - (buffer.Length - relativeFrameGapStart), direction);
                    });
                }
                return numFound;
            }
        }
    }
}