using System;
using System.Collections.Generic;

using ResonantSpark.Utility;

namespace ResonantSpark {
    namespace Input {

        [Serializable]
        public enum FightingGameAbsInputCodeDir : int {
            // [7,8,9]       [A] [B] [S]
            // [4,5,6]       [D] [C]
            // [1,2,3]

            None = 0,

            DownLeft,
            Down,
            DownRight,
            Left,
            Neutral,
            Right,
            UpLeft,
            Up,
            UpRight,
        };

        public enum FightingGameInputCodeDir : int {
            // [7,8,9]       [A] [B] [S]
            // [4,5,6]       [D] [C]
            // [1,2,3]

            None = 0,

            DownBack    = 1,
            Down        = 2,
            DownForward = 3,
            Back        = 4,
            Neutral     = 5,
            Forward     = 6,
            UpBack      = 7,
            Up          = 8,
            UpForward   = 9,
        };

        [Serializable]
        public enum FightingGameInputCodeBut : int {
            // [7,8,9]       [A] [B] [S]
            // [4,5,6]       [D] [C]
            // [1,2,3]

            None = 0,

            A = 0b00000001,
            B = 0b00000010,
            C = 0b00000100,
            D = 0b00001000,
            S = 0b00010000,
        };

        [Serializable]
        public class InputNotation {
            private List<string> inputString;
            private List<bool> validateInput;

            public InputNotation(List<string> inputString) {
                this.inputString = inputString;
                validateInput = new List<bool>();
                for (int n = 0; n < this.inputString.Count; ++n) {
                    validateInput.Add(false);
                }
                ResetValidateInput();
            }

            public bool ValidInputCombos(List<string> inputNotation) {
                ResetValidateInput();
                for (int n = 0; n < inputString.Count; ++n) {
                    string input = inputString[n];
                    for (int i = 0; i < inputNotation.Count; ++i) {
                        if (GameInputUtil.ValidateTemplate(input, inputNotation[i])) {
                            validateInput[n] = true;
                            break;
                        }
                    }
                }
                bool ret = true;
                foreach (bool val in validateInput) {
                    if (!val) {
                        ret = false;
                        break;
                    }
                }
                return ret;
            }

            private void ResetValidateInput() {
                for (int n = 0; n < this.inputString.Count; ++n) {
                    validateInput[n] = false;
                }
            }
        }

        [Serializable]
        public struct GameInputStruct {
            public FightingGameAbsInputCodeDir direction;
            public bool butA;
            public bool butB;
            public bool butC;
            public bool butD;
            public bool butS;

            public static GameInputStruct operator! (GameInputStruct inputStruct) {
                return new GameInputStruct {
                    butA = !inputStruct.butA,
                    butB = !inputStruct.butB,
                    butC = !inputStruct.butC,
                    butD = !inputStruct.butD,
                    butS = !inputStruct.butS,
                    direction = (FightingGameAbsInputCodeDir)(10 - (int) inputStruct.direction),
                };
            }

            public new string ToString() {
                string dir = "";
                switch (direction) {
                    case FightingGameAbsInputCodeDir.None:
                        dir += " ";
                        break;
                    case FightingGameAbsInputCodeDir.DownLeft:
                        dir += "↙";
                        break;
                    case FightingGameAbsInputCodeDir.Down:
                        dir += "↓";
                        break;
                    case FightingGameAbsInputCodeDir.DownRight:
                        dir += "↘";
                        break;
                    case FightingGameAbsInputCodeDir.Left:
                        dir += "←";
                        break;
                    case FightingGameAbsInputCodeDir.Neutral:
                        dir += "⋅";
                        break;
                    case FightingGameAbsInputCodeDir.Right:
                        dir += "→";
                        break;
                    case FightingGameAbsInputCodeDir.UpLeft:
                        dir += "↖";
                        break;
                    case FightingGameAbsInputCodeDir.Up:
                        dir += "↑";
                        break;
                    case FightingGameAbsInputCodeDir.UpRight:
                        dir += "↗";
                        break;
                }

                return "(" + dir + "," + (butA ? "A," : "_,") + (butB ? "B," : "_,") + (butC ? "C," : "_,") + (butD ? "D," : "_,") + (butS ? "S," : "_,") + ")";
            }
        };
    }
}