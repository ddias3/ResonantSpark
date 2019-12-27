using System;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Input {
        namespace Combinations {
            public class Combination : ScriptableObject, IComparable<Combination> {
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

                public bool Stale(int frameCurrent) {
                    return (frameCurrent - frameTrigger) > staleTime;
                }

                public int CompareTo(Combination other) {
                    if (other.priorityValue == this.priorityValue) return other.frameTrigger - this.frameTrigger;
                    else return other.priorityValue - this.priorityValue;
                }
            }

            public class Empty : Combination {
                public Empty() {
                    SetInfo(staleTime: -1, priorityValue: 0);
                }
            }

            public class DirectionCurrent : Combination {
                public FightingGameInputCodeDir direction;

                public DirectionCurrent() {
                    SetInfo(staleTime: 0, priorityValue: 1);
                    this.direction = FightingGameInputCodeDir.None;
                }

                public virtual DirectionCurrent Init(int frameTrigger, FightingGameInputCodeDir direction) {
                    base.Init(frameTrigger);
                    this.direction = direction;
                    return this;
                }
            }

            public class DirectionPress : Combination {
                public FightingGameInputCodeDir direction;

                public DirectionPress() {
                    SetInfo(staleTime: 5, priorityValue: 100);
                    this.direction = FightingGameInputCodeDir.None;
                }

                public virtual DirectionPress Init(int frameTrigger, FightingGameInputCodeDir direction) {
                    base.Init(frameTrigger);
                    this.direction = direction;
                    return this;
                }
            }

            public class DirectionLongHold : DirectionPress {
                public int holdLength;

                public DirectionLongHold() : base() {
                    SetInfo(staleTime: 5, priorityValue: 750);
                }

                public DirectionLongHold Init(int frameTrigger, FightingGameInputCodeDir direction, int holdLength) {
                    base.Init(frameTrigger, direction);
                    this.holdLength = holdLength;
                    return this;
                }
            }

            public class DoubleTap : Combination {
                public int frameStart;
                public int frameEnd;
                public FightingGameInputCodeDir direction;

                public DoubleTap() {
                    SetInfo(staleTime: 15, priorityValue: 1000);
                    this.frameStart = -1;
                    this.frameEnd = -1;
                }

                public DoubleTap Init(int frameEnd, int frameStart, FightingGameInputCodeDir direction) {
                    base.Init(frameEnd);
                    this.direction = direction;
                    this.frameStart = frameStart;
                    this.frameEnd = frameEnd;
                    return this;
                }
            }

            public class NeutralReturn : Combination {
                public NeutralReturn() {
                    SetInfo(staleTime: 3, priorityValue: 5);
                }

                public new NeutralReturn Init(int frameTrigger) {
                    base.Init(frameTrigger);
                    return this;
                }
            }

            public class ButtonPress : Combination {
                public FightingGameInputCodeBut button0;

                public ButtonPress() {
                    SetInfo(staleTime: 3, priorityValue: 250);
                    button0 = FightingGameInputCodeBut.None;
                }

                public ButtonPress Init(int frameTrigger, FightingGameInputCodeBut button0) {
                    base.Init(frameTrigger);
                    this.button0 = button0;
                    return this;
                }
            }

            public class Button2Press : ButtonPress {
                public FightingGameInputCodeBut button1;

                public Button2Press() {
                    SetInfo(staleTime: 8, priorityValue: 500);
                    button1 = FightingGameInputCodeBut.None;
                }

                public Button2Press Init(int frameTrigger, FightingGameInputCodeBut button0, FightingGameInputCodeBut button1) {
                    base.Init(frameTrigger, button0);
                    this.button1 = button1;
                    return this;
                }
            }

            public class Button3Press : ButtonPress {
                public FightingGameInputCodeBut button1;
                public FightingGameInputCodeBut button2;

                public Button3Press() {
                    SetInfo(staleTime: 8, priorityValue: 2000);
                    button1 = FightingGameInputCodeBut.None;
                    button2 = FightingGameInputCodeBut.None;
                }

                public Button3Press Init(int frameTrigger, FightingGameInputCodeBut button0, FightingGameInputCodeBut button1, FightingGameInputCodeBut button2) {
                    base.Init(frameTrigger, button0);
                    this.button1 = button1;
                    this.button2 = button2;
                    return this;
                }
            }

            public class DirectionPlusButton : Combination {
                public FightingGameInputCodeBut button0;
                public FightingGameInputCodeDir direction;

                public DirectionPlusButton() {
                    SetInfo(staleTime: 5, priorityValue: 1200);

                    button0 = FightingGameInputCodeBut.None;
                    direction = FightingGameInputCodeDir.None;
                }

                public DirectionPlusButton Init(int frameTrigger, FightingGameInputCodeBut button0, FightingGameInputCodeDir direction) {
                    base.Init(frameTrigger);
                    this.button0 = button0;
                    this.direction = direction;
                    return this;
                }
            }

            public class QuarterCircle : Combination {
                public FightingGameInputCodeDir endDirection;

                public QuarterCircle() {
                    SetInfo(staleTime: 6, priorityValue: 5000);

                    endDirection = FightingGameInputCodeDir.None;
                }

                public QuarterCircle Init(int frameTrigger, FightingGameInputCodeDir endDirection) {
                    base.Init(frameTrigger);
                    this.endDirection = endDirection;
                    return this;
                }
            }

            public class QuarterCircleButtonPress : QuarterCircle {
                public FightingGameInputCodeBut button0;

                public QuarterCircleButtonPress() {
                    SetInfo(staleTime: 6, priorityValue: 10000);

                    button0 = FightingGameInputCodeBut.None;
                }

                public QuarterCircleButtonPress Init(int frameTrigger, FightingGameInputCodeDir endDirection, FightingGameInputCodeBut button0) {
                    base.Init(frameTrigger, endDirection);
                    this.button0 = button0;
                    return this;
                }
            }
        }
    }
}