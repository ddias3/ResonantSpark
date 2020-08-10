using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Input;
using ResonantSpark.Input.Combinations;

namespace ResonantSpark {
    namespace Utility {
        public static class GameInputUtil {
            public static bool Forward(FightingGameInputCodeDir input) {
                return input == FightingGameInputCodeDir.DownForward
                    || input == FightingGameInputCodeDir.Forward
                    || input == FightingGameInputCodeDir.UpForward;
            }

            public static bool Back(FightingGameInputCodeDir input) {
                return input == FightingGameInputCodeDir.DownBack
                    || input == FightingGameInputCodeDir.Back
                    || input == FightingGameInputCodeDir.UpBack;
            }

            public static bool Down(FightingGameInputCodeDir input) {
                return input == FightingGameInputCodeDir.DownBack
                    || input == FightingGameInputCodeDir.Down
                    || input == FightingGameInputCodeDir.DownForward;
            }

            public static bool Up(FightingGameInputCodeDir input) {
                return input == FightingGameInputCodeDir.UpBack
                    || input == FightingGameInputCodeDir.Up
                    || input == FightingGameInputCodeDir.UpForward;
            }

            public static List<string> CreateInputComboNotation(List<Combination> inputCombos, Func<FightingGameAbsInputCodeDir, FightingGameInputCodeDir> directionMapFunc) {
                List<string> inputNotation = new List<string>();
                for (int n = 0; n < inputCombos.Count; ++n) {
                    string[] notations = inputCombos[n].ToString(directionMapFunc).Split('⋅');
                    for (int i = 0; i < notations.Length; ++i) {
                        inputNotation.Add(notations[i]);
                    }
                }
                return inputNotation;
            }

            public static bool ValidateTemplate(string template, string actual) {
                //return template == actual;
                string[] split = template.Split('|');
                for (int n = 0; n < split.Length; ++n) {
                    if (SimultaneousButtons(split[n], actual)) {
                        return true;
                    }
                }
                return false;
            }

            private static bool SimultaneousButtons(string template, string actual) {
                string[] templateSplit = template.Split('+');
                string[] actualSplit = actual.Split('+');
                if (templateSplit.Length != actualSplit.Length) {
                    return false;
                }

                bool[] exists = new bool[templateSplit.Length];
                for (int t = 0; t < templateSplit.Length; ++t) {
                    for (int a = 0; a < actualSplit.Length; ++a) {
                        if (templateSplit[t] == actualSplit[a]) {
                            if (!exists[t]) {
                                exists[t] = true;
                            }
                            else {
                                throw new Exception("Actual string of buttons contains the same button twice simulatenously.");
                            }
                        }
                    }
                }
                for (int n = 0; n < exists.Length; ++n) {
                    if (!exists[n]) {
                        return false;
                    }
                }
                return true;
            }
        }
    }
}