using System;

namespace ResonantSpark {
    namespace Input {

        [Serializable]
        public enum FightingGameInputCodeDir : int {
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

            _5A,
            _5Ah,
            _2A,
            _2Ah,
            _5B,
            _5Bh,
            _2B,
            _2Bh,
            _5C,
            _5Ch,
            _2C,
            _2Ch,
            _5D,
            _5Dh,
            _2D,
            _2Dh,
            _5S,
            _5Sh,
            _2S,
            _2Sh,
        }

        [Serializable]
        public struct GameInputStruct {
            public FightingGameInputCodeDir direction;
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
                    direction = (FightingGameInputCodeDir)(10 - (int) inputStruct.direction),
                };
            }

            public new string ToString() {
                string dir = "";
                switch (direction) {
                    case FightingGameInputCodeDir.None:
                        dir += " ";
                        break;
                    case FightingGameInputCodeDir.DownLeft:
                        dir += "↙";
                        break;
                    case FightingGameInputCodeDir.Down:
                        dir += "↓";
                        break;
                    case FightingGameInputCodeDir.DownRight:
                        dir += "↘";
                        break;
                    case FightingGameInputCodeDir.Left:
                        dir += "←";
                        break;
                    case FightingGameInputCodeDir.Neutral:
                        dir += "⋅";
                        break;
                    case FightingGameInputCodeDir.Right:
                        dir += "→";
                        break;
                    case FightingGameInputCodeDir.UpLeft:
                        dir += "↖";
                        break;
                    case FightingGameInputCodeDir.Up:
                        dir += "↑";
                        break;
                    case FightingGameInputCodeDir.UpRight:
                        dir += "↗";
                        break;
                }

                return "(" + dir + "," + (butA ? "A," : "_,") + (butB ? "B," : "_,") + (butC ? "C," : "_,") + (butD ? "D," : "_,") + (butS ? "S," : "_,") + ")";
            }
        };
    }
}