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

            public static InputNotation SelectInputNotationDirectionless(InputNotation notation) {
                switch (notation) {
                    case InputNotation._1A:
                    case InputNotation._2A:
                    case InputNotation._3A:
                    case InputNotation._4A:
                    case InputNotation._5A:
                    case InputNotation._6A:
                    case InputNotation._236A:
                    case InputNotation._214A:
                        return InputNotation._A;
                    case InputNotation._1B:
                    case InputNotation._2B:
                    case InputNotation._3B:
                    case InputNotation._4B:
                    case InputNotation._5B:
                    case InputNotation._6B:
                    case InputNotation._236B:
                    case InputNotation._214B:
                        return InputNotation._B;
                    case InputNotation._1C:
                    case InputNotation._2C:
                    case InputNotation._3C:
                    case InputNotation._4C:
                    case InputNotation._5C:
                    case InputNotation._6C:
                    case InputNotation._236C:
                    case InputNotation._214C:
                        return InputNotation._C;
                    case InputNotation._236D:
                        return InputNotation._D;
                    default:
                        return notation;
                }
            }

            public static InputNotation SelectInputNotation(FightingGameInputCodeBut button, FightingGameInputCodeDir direction) {
                InputNotation notation = InputNotation.None;

                switch (button) {
                    case FightingGameInputCodeBut.A:
                        switch (direction) {
                            case FightingGameInputCodeDir.Back:
                                notation = InputNotation._4A;
                                break;
                            case FightingGameInputCodeDir.Neutral:
                            case FightingGameInputCodeDir.None:
                                notation = InputNotation._5A;
                                break;
                            case FightingGameInputCodeDir.Forward:
                                notation = InputNotation._6A;
                                break;
                            case FightingGameInputCodeDir.DownBack:
                                notation = InputNotation._1A;
                                break;
                            case FightingGameInputCodeDir.Down:
                                notation = InputNotation._2A;
                                break;
                            case FightingGameInputCodeDir.DownForward:
                                notation = InputNotation._3A;
                                break;
                            default:
                                notation = InputNotation._5A;
                                break;
                        }
                        break;
                    case FightingGameInputCodeBut.B:
                        switch (direction) {
                            case FightingGameInputCodeDir.Back:
                                notation = InputNotation._4B;
                                break;
                            case FightingGameInputCodeDir.Neutral:
                            case FightingGameInputCodeDir.None:
                                notation = InputNotation._5B;
                                break;
                            case FightingGameInputCodeDir.Forward:
                                notation = InputNotation._6B;
                                break;
                            case FightingGameInputCodeDir.DownBack:
                                notation = InputNotation._1B;
                                break;
                            case FightingGameInputCodeDir.Down:
                                notation = InputNotation._2B;
                                break;
                            case FightingGameInputCodeDir.DownForward:
                                notation = InputNotation._3B;
                                break;
                            default:
                                notation = InputNotation._5B;
                                break;
                        }
                        break;
                    case FightingGameInputCodeBut.C:
                        switch (direction) {
                            case FightingGameInputCodeDir.Back:
                                notation = InputNotation._4C;
                                break;
                            case FightingGameInputCodeDir.Neutral:
                            case FightingGameInputCodeDir.None:
                                notation = InputNotation._5C;
                                break;
                            case FightingGameInputCodeDir.Forward:
                                notation = InputNotation._6C;
                                break;
                            case FightingGameInputCodeDir.DownBack:
                                notation = InputNotation._1C;
                                break;
                            case FightingGameInputCodeDir.Down:
                                notation = InputNotation._2C;
                                break;
                            case FightingGameInputCodeDir.DownForward:
                                notation = InputNotation._3C;
                                break;
                            default:
                                notation = InputNotation._5C;
                                break;
                        }
                        break;
                }

                return notation;
            }
        }
    }
}