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
            _5A,
            _5AA,
            _5AhA,
            _5AAA,
            _5AAhA,
            _5AAAA,
            _5AAAhA,
            _2A,
            _2AA
        }

        [Serializable]
        public struct GameInputStruct {
            public FightingGameInputCodeDir direction;
            public bool butA;
            public bool butB;
            public bool butC;
            public bool butD;
            public bool butS;

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