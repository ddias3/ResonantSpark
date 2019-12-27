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
        };
    }
}