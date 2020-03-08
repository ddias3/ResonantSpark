using System;

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
        public enum InputNotation : int {
            None = 0,

            _A,
            _B,
            _C,
            _D,

            _4A,
            _5A,
            _5Ah,
            _6A,
            _2A,
            _2Ah,
            _5B,
            _5Bh,
            _6B,
            _2B,
            _2Bh,
            _5C,
            _5Ch,
            _6C,
            _2C,
            _2Ch,
            _5D,
            _5Dh,
            _6D,
            _2D,
            _2Dh,
            _5S,
            _5Sh,
            _2S,
            _2Sh,
            _236A,
            _236B,
            _236C,
            _236D,
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