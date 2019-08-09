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

            private static readonly Regex rgxNeutralReturn = new Regex(@"[^5]+(5)", RegexOptions.Compiled | RegexOptions.RightToLeft);
            public static int FindNeutralReturns(string buffer, Factory inputFactory, int frameIndex, List<Combination> activeInputs) {
                int numFound = 0;
                foreach (Match match in rgxNeutralReturn.Matches(buffer)) {
                    NeutralReturn input = AddToActiveInputs<NeutralReturn>(activeInputs, inputFactory, frameIndex - match.Groups[0].Index, frameIndex, (newInput) => {
                        newInput.Init(frameIndex - match.Groups[0].Index);
                    });
                    numFound++;
                }
                return numFound;
            }

            private static readonly Regex rgxDirectionPresses = new Regex(@"([^5])\1*(?=[^\1])", RegexOptions.Compiled | RegexOptions.RightToLeft);
            public static int FindDirectionPresses(string buffer, Factory inputFactory, int frameIndex, List<Combination> activeInputs) {
                int numFound = 0;
                foreach (Match match in rgxDirectionPresses.Matches(buffer)) {
                    DirectionPress input = AddToActiveInputs<DirectionPress>(activeInputs, inputFactory, frameIndex - match.Index, frameIndex, (newInput) => {
                        newInput.Init(frameIndex - match.Index, (FightingGameInputCodeDir) int.Parse(match.Groups[0].Value));
                    });
                }
                return numFound;
            }

            private static readonly Regex rgxDirectionHolds = new Regex(@"([^5])\1{19,}", RegexOptions.Compiled | RegexOptions.RightToLeft);
            public static int FindDirectionHolds(string buffer, Factory inputFactory, int frameIndex, List<Combination> activeInputs) {
                int numFound = 0;
                foreach (Match match in rgxDirectionHolds.Matches(buffer)) {
                    FightingGameInputCodeDir direction = (FightingGameInputCodeDir) int.Parse(match.Groups[0].Value);
                    DirectionHold input = AddToActiveInputs<DirectionHold>(activeInputs, inputFactory, match.Index, frameIndex, (newInput) => {
                        newInput.Init(frameIndex - match.Index + 19, direction, match.Value.Length);
                    });
                }
                return numFound;
            }

            private static readonly Regex rgxDoubleDirectionTaps = new Regex(@"(([^5])\2*[^5\2]*)(5{1,6})(\2+)(?=[^\1])", RegexOptions.Compiled | RegexOptions.RightToLeft);
            public static int FindDoubleDirectionTaps(string buffer, Factory inputFactory, int frameIndex, List<Combination> activeInputs) {
                int numFound = 0;
                foreach (Match match in rgxDoubleDirectionTaps.Matches(buffer)) {
                    int relativeFrameGapStart = match.Groups[2].Index;
                    int relativeFrameGapEnd = match.Groups[3].Index;
                    FightingGameInputCodeDir direction = (FightingGameInputCodeDir) int.Parse(match.Groups[1].Value);
                    DoubleTap input = AddToActiveInputs<DoubleTap>(activeInputs, inputFactory, frameIndex - relativeFrameGapEnd, frameIndex, (newInput) => {
                        newInput.Init(frameIndex - relativeFrameGapEnd, frameIndex - relativeFrameGapStart, direction);
                    });
                }
                return numFound;
            }
        }
    }
}