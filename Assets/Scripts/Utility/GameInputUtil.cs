using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Input;

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
        }
    }
}