using System;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Input {
        namespace Combinations {
            public abstract class Combination : ScriptableObject, IComparable<Combination> {
                protected int frameTrigger;
                protected int staleTime; // in frames
                protected int priorityValue; // instead of comparisons in the compareTo
                public bool inUse;
                public bool dirty;

                // The constructor is for when the buffer is populated the first time.
                public Combination() {
                    this.frameTrigger = -0xFFFF;
                }

                // The init call is for when an existing combination is recycled and being set up.
                public virtual Combination Init(int frameTrigger) {
                    this.frameTrigger = frameTrigger;
                    dirty = false;
                    inUse = false;
                    return this;
                }

                protected void SetInfo(int staleTime, int priorityValue) {
                    //if (infoSet) return; // this line would be necessary if the constructors were called derived first.
                    this.staleTime = staleTime;
                    this.priorityValue = priorityValue;
                }

                public void Refresh() {
                    dirty = false;
                }

                public int GetFrame() {
                    return frameTrigger;
                }

                public int GetPriority() {
                    return priorityValue;
                }

                public bool Stale(int frameCurrent) {
                    return (frameCurrent - frameTrigger) > staleTime;
                }

                public virtual int CompareTo(Combination other) {
                    if (other.GetPriority() == this.priorityValue) return other.GetFrame() - this.frameTrigger;
                    else return other.GetPriority() - this.priorityValue;
                }

                public abstract bool Equals(Combination other);
                public abstract string ToString(Func<FightingGameAbsInputCodeDir, FightingGameInputCodeDir> directionMapFunc);
            }

            public class Empty : Combination {
                public Empty() {
                    SetInfo(staleTime: -0xFFFF, priorityValue: 0);
                }

                public override int CompareTo(Combination other) {
                    return 1;
                }

                public override bool Equals(Combination other) {
                    return other.GetType() == typeof(Empty);
                }

                public override string ToString(Func<FightingGameAbsInputCodeDir, FightingGameInputCodeDir> directionMapFunc) {
                    return "";
                }
            }

            public class DirectionCurrent : Combination {
                public FightingGameAbsInputCodeDir direction { get; private set; }

                public DirectionCurrent() {
                    SetInfo(staleTime: 0, priorityValue: 1);
                    this.direction = FightingGameAbsInputCodeDir.None;
                }

                public virtual DirectionCurrent Init(int frameTrigger, FightingGameAbsInputCodeDir direction) {
                    base.Init(frameTrigger);
                    this.direction = direction;
                    return this;
                }

                public override bool Equals(Combination other) {
                    return other.GetType() == typeof(DirectionCurrent) && other.GetFrame() == this.frameTrigger;
                }

                public override string ToString(Func<FightingGameAbsInputCodeDir, FightingGameInputCodeDir> directionMapFunc) {
                    return ((int)directionMapFunc(direction)).ToString();
                }
            }

            public class ButtonsCurrent : Combination {
                public bool butA { get; private set; }
                public bool butB { get; private set; }
                public bool butC { get; private set; }
                public bool butD { get; private set; }
                public bool butS { get; private set; }

                public ButtonsCurrent() {
                    SetInfo(staleTime: 0, priorityValue: 2);
                    butA = butB = butC = butD = butS = false;
                }

                public virtual ButtonsCurrent Init(int frameTrigger, bool butA, bool butB, bool butC, bool butD, bool butS) {
                    base.Init(frameTrigger);
                    this.butA = butA;
                    this.butB = butB;
                    this.butC = butC;
                    this.butD = butD;
                    this.butS = butS;
                    return this;
                }

                public override bool Equals(Combination other) {
                    return other.GetType() == typeof(ButtonsCurrent) && other.GetFrame() == this.frameTrigger;
                }

                public override string ToString(Func<FightingGameAbsInputCodeDir, FightingGameInputCodeDir> directionMapFunc) {
                    string retString = "";
                    bool first = true;
                    if (butA) {
                        retString += (first) ? "A" : "+A";
                        first = false;
                    }
                    if (butB) {
                        retString += (first) ? "B" : "+B";
                        first = false;
                    }
                    if (butC) {
                        retString += (first) ? "C" : "+C";
                        first = false;
                    }
                    if (butD) {
                        retString += (first) ? "D" : "+D";
                        first = false;
                    }
                    if (butS) {
                        retString += (first) ? "S" : "+S";
                        first = false;
                    }
                    return retString;
                }
            }

            public class DirectionPress : Combination {
                public FightingGameAbsInputCodeDir direction { get; private set; }

                public DirectionPress() {
                    SetInfo(staleTime: 6, priorityValue: 100);
                    this.direction = FightingGameAbsInputCodeDir.None;
                }

                public virtual DirectionPress Init(int frameTrigger, FightingGameAbsInputCodeDir direction) {
                    base.Init(frameTrigger);
                    this.direction = direction;
                    return this;
                }

                public override bool Equals(Combination other) {
                    return other.GetType() == typeof(DirectionPress)
                        && other.GetFrame() == this.frameTrigger
                        && ((DirectionPress) other).direction == this.direction;
                }

                public override string ToString(Func<FightingGameAbsInputCodeDir, FightingGameInputCodeDir> directionMapFunc) {
                    return ((int)directionMapFunc(direction)).ToString();
                }
            }

            public class DirectionLongHold : DirectionPress {
                public int holdLength { get; private set; }

                public DirectionLongHold() : base() {
                    SetInfo(staleTime: 15, priorityValue: 750);
                }

                public DirectionLongHold Init(int frameTrigger, FightingGameAbsInputCodeDir direction, int holdLength) {
                    base.Init(frameTrigger, direction);
                    this.holdLength = holdLength;
                    return this;
                }

                public override bool Equals(Combination other) {
                    return other.GetType() == typeof(DirectionLongHold)
                        && other.GetFrame() == this.frameTrigger
                        && ((DirectionLongHold) other).direction == this.direction
                        && ((DirectionLongHold) other).holdLength == this.holdLength;
                }

                public override string ToString(Func<FightingGameAbsInputCodeDir, FightingGameInputCodeDir> directionMapFunc) {
                    return "[" + ((int)directionMapFunc(direction)).ToString() + "]";
                }
            }

            public class DoubleTap : Combination {
                public FightingGameAbsInputCodeDir direction { get; private set; }

                public DoubleTap() {
                    SetInfo(staleTime: 12, priorityValue: 1000);
                }

                public DoubleTap Init(int frameTrigger, FightingGameAbsInputCodeDir direction) {
                    base.Init(frameTrigger);
                    this.direction = direction;
                    return this;
                }

                public override bool Equals(Combination other) {
                    return other.GetType() == typeof(DoubleTap)
                        && other.GetFrame() == this.frameTrigger;
                }

                public override string ToString(Func<FightingGameAbsInputCodeDir, FightingGameInputCodeDir> directionMapFunc) {
                    return ((int)directionMapFunc(direction)).ToString() + ((int)directionMapFunc(direction)).ToString();
                }
            }

            public class NeutralReturn : Combination {
                public NeutralReturn() {
                    SetInfo(staleTime: 6, priorityValue: 5);
                }

                public new NeutralReturn Init(int frameTrigger) {
                    base.Init(frameTrigger);
                    return this;
                }

                public override bool Equals(Combination other) {
                    return other.GetType() == typeof(NeutralReturn)
                        && other.GetFrame() == this.frameTrigger;
                }

                public override string ToString(Func<FightingGameAbsInputCodeDir, FightingGameInputCodeDir> directionMapFunc) {
                    return "5";
                }
            }

            public class ButtonRelease : Combination {
                public FightingGameInputCodeBut button0 { get; private set; }

                public ButtonRelease() {
                    SetInfo(staleTime: 6, priorityValue: 200);
                    button0 = FightingGameInputCodeBut.None;
                }

                public ButtonRelease Init(int frameTrigger, FightingGameInputCodeBut button0) {
                    base.Init(frameTrigger);
                    this.button0 = button0;
                    return this;
                }

                public override int CompareTo(Combination other) {
                    int baseCompareTo = base.CompareTo(other);
                    if (baseCompareTo == 0) return (int) ((ButtonRelease) other).button0 - (int) this.button0;
                    else return baseCompareTo;
                }

                public override bool Equals(Combination other) {
                    return other.GetType() == typeof(ButtonRelease)
                        && other.GetFrame() == this.frameTrigger
                        && ((ButtonRelease) other).button0 == this.button0;
                }

                public override string ToString(Func<FightingGameAbsInputCodeDir, FightingGameInputCodeDir> directionMapFunc) {
                    switch (button0) {
                        case FightingGameInputCodeBut.A:
                            return "]A[";
                        case FightingGameInputCodeBut.B:
                            return "]B[";
                        case FightingGameInputCodeBut.C:
                            return "]C[";
                        case FightingGameInputCodeBut.D:
                            return "]D[";
                        case FightingGameInputCodeBut.S:
                            return "]S[";
                        default:
                            return "][";
                    }
                }
            }

            public class ButtonPress : Combination {
                public FightingGameInputCodeBut button0 { get; private set; }

                public ButtonPress() {
                    SetInfo(staleTime: 6, priorityValue: 250);
                    button0 = FightingGameInputCodeBut.None;
                }

                public ButtonPress Init(int frameTrigger, FightingGameInputCodeBut button0) {
                    base.Init(frameTrigger);
                    this.button0 = button0;
                    return this;
                }

                public override int CompareTo(Combination other) {
                    int baseCompareTo = base.CompareTo(other);
                    if (baseCompareTo == 0) return (int) ((ButtonPress) other).button0 - (int) this.button0;
                    else return baseCompareTo;
                }

                public override bool Equals(Combination other) {
                    return other.GetType() == typeof(ButtonPress)
                        && other.GetFrame() == this.frameTrigger
                        && ((ButtonPress) other).button0 == this.button0;
                }

                public override string ToString(Func<FightingGameAbsInputCodeDir, FightingGameInputCodeDir> directionMapFunc) {
                    switch (button0) {
                        case FightingGameInputCodeBut.A:
                            return "A";
                        case FightingGameInputCodeBut.B:
                            return "B";
                        case FightingGameInputCodeBut.C:
                            return "C";
                        case FightingGameInputCodeBut.D:
                            return "D";
                        case FightingGameInputCodeBut.S:
                            return "S";
                        default:
                            return "";
                    }
                }
            }

            public class Button2Press : ButtonPress {
                public FightingGameInputCodeBut button1 { get; private set; }

                public Button2Press() {
                    SetInfo(staleTime: 12, priorityValue: 500);
                    button1 = FightingGameInputCodeBut.None;
                }

                public Button2Press Init(int frameTrigger, FightingGameInputCodeBut button0, FightingGameInputCodeBut button1) {
                    if (button0 > button1) {
                        base.Init(frameTrigger, button0);
                        this.button1 = button1;
                    }
                    else {
                        base.Init(frameTrigger, button1);
                        this.button1 = button0;
                    }
                    return this;
                }

                public override int CompareTo(Combination other) {
                    int baseCompareTo = base.CompareTo(other);
                    if (baseCompareTo == 0) return (int) ((Button2Press) other).button1 - (int) this.button1;
                    else return baseCompareTo;
                }

                public override bool Equals(Combination other) {
                    return other.GetType() == typeof(Button2Press)
                        && other.GetFrame() == this.frameTrigger
                        && ((Button2Press) other).button0 == this.button0
                        && ((Button2Press) other).button1 == this.button1;
                }

                public override string ToString(Func<FightingGameAbsInputCodeDir, FightingGameInputCodeDir> directionMapFunc) {
                    string but0 = base.ToString(directionMapFunc);
                    switch (button1) {
                        case FightingGameInputCodeBut.A:
                            return but0 + "+A";
                        case FightingGameInputCodeBut.B:
                            return but0 + "+B";
                        case FightingGameInputCodeBut.C:
                            return but0 + "+C";
                        case FightingGameInputCodeBut.D:
                            return but0 + "+D";
                        case FightingGameInputCodeBut.S:
                            return but0 + "+S";
                        default:
                            return but0 + "";
                    }
                }
            }

            public class Button3Press : Button2Press {
                public FightingGameInputCodeBut button2 { get; private set; }

                public Button3Press() {
                    SetInfo(staleTime: 18, priorityValue: 2000);
                    button2 = FightingGameInputCodeBut.None;
                }

                public Button3Press Init(int frameTrigger, FightingGameInputCodeBut button0, FightingGameInputCodeBut button1, FightingGameInputCodeBut button2) {
                    if (button0 > button1) {
                        if (button1 > button2) {
                            // 0 > 1 > 2
                            base.Init(frameTrigger, button0, button1);
                            this.button2 = button2;
                        }
                        else {
                            // 0|2 > 1
                            base.Init(frameTrigger, button0, button2);
                            this.button2 = button1;
                        }
                    }
                    else {
                        if (button0 > button2) {
                            // 1 > 0 > 2
                            base.Init(frameTrigger, button0, button1);
                            this.button2 = button2;
                        }
                        else {
                            // 1|2 > 0
                            base.Init(frameTrigger, button1, button2);
                            this.button2 = button0;
                        }
                    }
                    return this;
                }

                public override int CompareTo(Combination other) {
                    int baseCompareTo = base.CompareTo(other);
                    if (baseCompareTo == 0) return (int) ((Button3Press) other).button2 - (int) this.button2;
                    else return baseCompareTo;
                }

                public override bool Equals(Combination other) {
                    return other.GetType() == typeof(Button3Press)
                        && other.GetFrame() == this.frameTrigger
                        && ((Button3Press) other).button0 == this.button0
                        && ((Button3Press) other).button1 == this.button1
                        && ((Button3Press) other).button2 == this.button2;
                }

                public override string ToString(Func<FightingGameAbsInputCodeDir, FightingGameInputCodeDir> directionMapFunc) {
                    string but01 = base.ToString(directionMapFunc);
                    switch (button1) {
                        case FightingGameInputCodeBut.A:
                            return but01 + "+A";
                        case FightingGameInputCodeBut.B:
                            return but01 + "+B";
                        case FightingGameInputCodeBut.C:
                            return but01 + "+C";
                        case FightingGameInputCodeBut.D:
                            return but01 + "+D";
                        case FightingGameInputCodeBut.S:
                            return but01 + "+S";
                        default:
                            return but01 + "";
                    }
                }
            }

            public class DirectionPlusButton : Combination {
                public FightingGameInputCodeBut button0 { get; private set; }
                public FightingGameAbsInputCodeDir direction { get; private set; }

                public DirectionPlusButton() {
                    SetInfo(staleTime: 15, priorityValue: 1200);

                    button0 = FightingGameInputCodeBut.None;
                    direction = FightingGameAbsInputCodeDir.None;
                }

                public DirectionPlusButton Init(int frameTrigger, FightingGameInputCodeBut button0, FightingGameAbsInputCodeDir direction) {
                    base.Init(frameTrigger);
                    this.button0 = button0;
                    this.direction = direction;
                    return this;
                }

                public override int CompareTo(Combination other) {
                    int baseCompareTo = base.CompareTo(other);
                    if (baseCompareTo == 0) return (int) ((DirectionPlusButton) other).button0 - (int) this.button0;
                    else return baseCompareTo;
                }

                public override bool Equals(Combination other) {
                    return other.GetType() == typeof(DirectionPlusButton)
                        && other.GetFrame() == this.frameTrigger
                        && ((DirectionPlusButton) other).button0 == this.button0
                        && ((DirectionPlusButton) other).direction == this.direction;
                }

                public override string ToString(Func<FightingGameAbsInputCodeDir, FightingGameInputCodeDir> directionMapFunc) {
                    switch (button0) {
                        case FightingGameInputCodeBut.A:
                            return ((int)directionMapFunc(direction)).ToString() + "⋅A";
                        case FightingGameInputCodeBut.B:
                            return ((int)directionMapFunc(direction)).ToString() + "⋅B";
                        case FightingGameInputCodeBut.C:
                            return ((int)directionMapFunc(direction)).ToString() + "⋅C";
                        case FightingGameInputCodeBut.D:
                            return ((int)directionMapFunc(direction)).ToString() + "⋅D";
                        case FightingGameInputCodeBut.S:
                            return ((int)directionMapFunc(direction)).ToString() + "⋅S";
                        default:
                            return ((int)directionMapFunc(direction)).ToString() + "";
                    }
                }
            }

            public class QuarterCircle : Combination {
                public FightingGameAbsInputCodeDir endDirection { get; private set; }

                public QuarterCircle() {
                    SetInfo(staleTime: 24, priorityValue: 5000);

                    endDirection = FightingGameAbsInputCodeDir.None;
                }

                public QuarterCircle Init(int frameTrigger, FightingGameAbsInputCodeDir endDirection) {
                    base.Init(frameTrigger);
                    this.endDirection = endDirection;
                    return this;
                }

                public override bool Equals(Combination other) {
                    return other.GetType() == typeof(QuarterCircle)
                        && other.GetFrame() == this.frameTrigger
                        && ((QuarterCircle) other).endDirection == this.endDirection;
                }

                public override string ToString(Func<FightingGameAbsInputCodeDir, FightingGameInputCodeDir> directionMapFunc) {
                    switch (directionMapFunc(endDirection)) {
                        case FightingGameInputCodeDir.Forward:
                            return "236";
                        case FightingGameInputCodeDir.Back:
                            return "214";
                        default:
                            return "";
                    }
                }
            }

            public class QuarterCircleButtonPress : QuarterCircle {
                public FightingGameInputCodeBut button0 { get; private set; }

                public QuarterCircleButtonPress() {
                    SetInfo(staleTime: 20, priorityValue: 10000);

                    button0 = FightingGameInputCodeBut.None;
                }

                public QuarterCircleButtonPress Init(int frameTrigger, FightingGameAbsInputCodeDir endDirection, FightingGameInputCodeBut button0) {
                    base.Init(frameTrigger, endDirection);
                    this.button0 = button0;
                    return this;
                }

                public override int CompareTo(Combination other) {
                    int baseCompareTo = base.CompareTo(other);
                    if (baseCompareTo == 0) return (int) ((QuarterCircleButtonPress) other).button0 - (int) this.button0;
                    else return baseCompareTo;
                }

                public override bool Equals(Combination other) {
                    return other.GetType() == typeof(QuarterCircleButtonPress)
                        && other.GetFrame() == this.frameTrigger
                        && ((QuarterCircleButtonPress) other).endDirection == this.endDirection
                        && ((QuarterCircleButtonPress) other).button0 == this.button0;
                }

                public override string ToString(Func<FightingGameAbsInputCodeDir, FightingGameInputCodeDir> directionMapFunc) {
                    string quarterCircle = base.ToString(directionMapFunc);
                    switch (button0) {
                        case FightingGameInputCodeBut.A:
                            return quarterCircle + "⋅A";
                        case FightingGameInputCodeBut.B:
                            return quarterCircle + "⋅B";
                        case FightingGameInputCodeBut.C:
                            return quarterCircle + "⋅C";
                        case FightingGameInputCodeBut.D:
                            return quarterCircle + "⋅D";
                        case FightingGameInputCodeBut.S:
                            return quarterCircle + "⋅S";
                        default:
                            return quarterCircle;
                    }
                }
            }
        }
    }
}