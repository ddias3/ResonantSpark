using System;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Input {
        namespace Combinations {
            public abstract class Combination : /*ScriptableObject,*/ IComparable<Combination> {
                protected int frameTrigger;
                public bool inUse;
                public bool dirty;

                public Combination(int frameTrigger) {
                    this.frameTrigger = frameTrigger;
                }

                public virtual Combination Init(int frameTrigger) {
                    this.frameTrigger = frameTrigger;
                    dirty = false;
                    inUse = false;
                    return this;
                }

                public void Refresh() {
                    dirty = false;
                }

                public int GetFrame() {
                    return frameTrigger;
                }

                public abstract bool Stale(int frameCurrent);
                public abstract int CompareTo(Combination other);
            }

            public class Empty : Combination {
                public Empty() : base(-0xFFFF) {
                    //do nothing
                }

                public override bool Stale(int frameCurrent) {
                    return true;
                }

                public override int CompareTo(Combination other) {
                    if (other.GetType() == typeof(Empty)) return 0;
                    else return 1;
                }
            }

            public class DirectionPress : Combination {
                public FightingGameInputCodeDir direction;

                public DirectionPress() : base(-0xFFFF) {
                    this.direction = FightingGameInputCodeDir.None;
                }

                public virtual DirectionPress Init(int frameTrigger, FightingGameInputCodeDir direction) {
                    base.Init(frameTrigger);
                    this.direction = direction;
                    return this;
                }

                public override bool Stale(int frameCurrent) {
                    return (frameCurrent - frameTrigger) > 5;
                }

                public override int CompareTo(Combination other) {
                        // TODO: Proper Compare To Priority Order
                    if (other.GetType() == typeof(DoubleTap) || other.GetType() == typeof(DirectionLongHold)) return 1;
                    else if (other.GetType() == typeof(DirectionPress)) return other.GetFrame() - this.GetFrame();
                    else return -1;
                }
            }

            public class DirectionCurrent : Combination {
                public FightingGameInputCodeDir direction;

                public DirectionCurrent() : base(-0xFFFF) {
                    this.direction = FightingGameInputCodeDir.None;
                }

                public virtual DirectionCurrent Init(int frameTrigger, FightingGameInputCodeDir direction) {
                    base.Init(frameTrigger);
                    this.direction = direction;
                    return this;
                }

                public override bool Stale(int frameCurrent) {
                    return (frameCurrent - frameTrigger) > 0;
                }

                public override int CompareTo(Combination other) {
                        // TODO: Proper Compare To Priority Order
                    if (other.GetType() == typeof(DirectionCurrent)) return other.GetFrame() - this.GetFrame();
                    else return 1;
                }
            }

            public class DirectionLongHold : DirectionPress {
                public int holdLength;

                public DirectionLongHold() : base() { }
                public DirectionLongHold Init(int frameTrigger, FightingGameInputCodeDir direction, int holdLength) {
                    base.Init(frameTrigger, direction);
                    this.holdLength = holdLength;
                    return this;
                }

                public override int CompareTo(Combination other) {
                        // TODO: Proper Compare To Priority Order
                    if (other.GetType() == typeof(DoubleTap)) return 1;
                    else if (other.GetType() == typeof(DirectionLongHold)) return other.GetFrame() - this.GetFrame();
                    else return -1;
                }
            }

            public class DoubleTap : Combination {
                public int frameStart;
                public int frameEnd;
                public FightingGameInputCodeDir direction;

                public DoubleTap() : base(-0xFFFF) {
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

                public override bool Stale(int frameCurrent) {
                    return (frameCurrent - frameTrigger) > 15;
                }

                public override int CompareTo(Combination other) {
                        // TODO: Proper Compare To Priority Order
                    if (other.GetType() == typeof(DoubleTap)) return other.GetFrame() - this.GetFrame();
                    else return -1;
                }
            }

            public class NeutralReturn : Combination {
                public NeutralReturn() : base(-0xFFFF) {
                    // do nothing
                }

                public new NeutralReturn Init(int frameTrigger) {
                    base.Init(frameTrigger);
                    return this;
                }

                public override bool Stale(int frameCurrent) {
                    return (frameCurrent - frameTrigger) > 3;
                }

                public override int CompareTo(Combination other) {
                    // TODO: Proper Compare To Priority Order
                    if (other.GetType() == typeof(DirectionCurrent)) return -1;
                    else if (other.GetType() == typeof(NeutralReturn)) return other.GetFrame() - this.GetFrame();
                    else return 1;
                }
            }
        }
    }
}