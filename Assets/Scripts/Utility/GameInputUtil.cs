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

            public static InputNotation SelectInputNotation(FightingGameInputCodeBut button, FightingGameInputCodeDir direction) {
                InputNotation notation = InputNotation.None;

                switch (button) {
                    case FightingGameInputCodeBut.A:
                        switch (direction) {
                            case FightingGameInputCodeDir.Neutral:
                            case FightingGameInputCodeDir.None:
                                notation = InputNotation._5A;
                                break;
                            case FightingGameInputCodeDir.DownBack:
                            case FightingGameInputCodeDir.Down:
                            case FightingGameInputCodeDir.DownForward:
                                notation = InputNotation._2A;
                                break;
                            default:
                                notation = InputNotation._5A;
                                break;
                        }
                        break;
                    case FightingGameInputCodeBut.B:
                        switch (direction) {
                            case FightingGameInputCodeDir.Neutral:
                            case FightingGameInputCodeDir.None:
                                notation = InputNotation._5B;
                                break;
                            case FightingGameInputCodeDir.DownBack:
                            case FightingGameInputCodeDir.Down:
                            case FightingGameInputCodeDir.DownForward:
                                notation = InputNotation._2B;
                                break;
                            default:
                                notation = InputNotation._5B;
                                break;
                        }
                        break;
                    case FightingGameInputCodeBut.C:
                        switch (direction) {
                            case FightingGameInputCodeDir.Neutral:
                            case FightingGameInputCodeDir.None:
                                notation = InputNotation._5C;
                                break;
                            case FightingGameInputCodeDir.DownBack:
                            case FightingGameInputCodeDir.Down:
                            case FightingGameInputCodeDir.DownForward:
                                notation = InputNotation._2C;
                                break;
                            default:
                                notation = InputNotation._5C;
                                break;
                        }
                        break;
                    case FightingGameInputCodeBut.D:
                        switch (direction) {
                            case FightingGameInputCodeDir.Neutral:
                            case FightingGameInputCodeDir.None:
                                notation = InputNotation._5D;
                                break;
                            case FightingGameInputCodeDir.DownBack:
                            case FightingGameInputCodeDir.Down:
                            case FightingGameInputCodeDir.DownForward:
                                notation = InputNotation._2D;
                                break;
                            default:
                                notation = InputNotation._5D;
                                break;
                        }
                        break;
                }

                return notation;
            }
        }
    }
}