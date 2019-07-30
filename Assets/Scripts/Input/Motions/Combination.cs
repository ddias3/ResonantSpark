using System;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Input {
        namespace Combinations {
            public abstract class Combination : ScriptableObject, IComparable<Combination> {
                protected int frameTrigger;
                public bool dirty;

                public Combination(int frameTrigger) {
                    this.frameTrigger = frameTrigger;
                    dirty = false;
                }

                public void Refresh() {
                    dirty = false;
                }

                public abstract bool Stale(int frameCurrent);
                public abstract int CompareTo(Combination other);
            }

            public class Empty : Combination {
                public Empty() : base(-1) {
                    //do nothing
                }

                public override bool Stale(int frameCurrent) {
                    return true;
                }

                public override int CompareTo(Combination other) {
                    if (other.GetType() == typeof(Empty)) return 0;
                    else return -1;
                }
            }

            public class DirectionPress : Combination {
                public FightingGameInputCodeDir direction;

                public DirectionPress() : base(-1) {
                    this.direction = FightingGameInputCodeDir.None;
                }

                public DirectionPress(int frameTrigger, FightingGameInputCodeDir direction) : base(frameTrigger) {
                    this.direction = direction;
                }

                public override bool Stale(int frameCurrent) {
                    return (frameCurrent - frameTrigger) > 5;
                }

                public override int CompareTo(Combination other) {
                        // TODO: Proper Compare To Priority Order
                    if (other.GetType() == typeof(DoubleTap)) return 1;
                    else if (other.GetType() == typeof(DirectionPress)) return 0;
                    return -1;
                }
            }

            public class DoubleTap : Combination {
                public int frameStart;
                public int frameEnd;

                public DoubleTap() : base(-1) {
                    this.frameStart = -1;
                    this.frameEnd = -1;
                }

                public DoubleTap(int frameStart, int frameEnd) : base(frameEnd) {
                    this.frameStart = frameStart;
                    this.frameEnd = frameEnd;
                }

                public override bool Stale(int frameCurrent) {
                    return (frameCurrent - frameTrigger) > 15;
                }

                public override int CompareTo(Combination other) {
                        // TODO: Proper Compare To Priority Order
                    if (other.GetType() == typeof(DoubleTap)) return -1;
                    else if (other.GetType() == typeof(DirectionPress)) return 0;
                    return 1;
                }
            }

            public class NeutralReturn : Combination {
                public NeutralReturn() : base(-1) {
                    // do nothing
                }

                public NeutralReturn(int frameTrigger) : base(frameTrigger) {
                    // do nothing
                }

                public override bool Stale(int frameCurrent) {
                    return (frameCurrent - frameTrigger) > 3;
                }

                public override int CompareTo(Combination other) {
                    // TODO: Proper Compare To Priority Order
                    if (other.GetType() == typeof(NeutralReturn)) return 0;
                    else return -1;
                }
            }
        }
    }
}