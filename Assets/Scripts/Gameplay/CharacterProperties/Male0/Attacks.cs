using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Builder;

namespace ResonantSpark {
    namespace CharacterProperties {
        namespace Male0 {
            public class Attacks : ScriptableObject {
                public Attack atkReg5A;
                public Attack atkReg5AA;
                public Attack atkReg5AAA;
                public Attack atkReg5AAAA;

                public Attack atkGfy5A;
                public Attack atkGfy5AA;
                public Attack atkGfy5AhA;

                public Attack atkReg2A;
                public Attack atkReg2AA;

                public Attack atkGfy2A;

                public Attack atkReg5B;
                public Attack atkReg5BB;
                public Attack atkReg5BBB;

                public Attack atkReg2B;

                public Attack atkGfy2B;

                public void Init(ICharacterPropertiesBuilder charBuilder) {
                    //atkReg5AAA = new Attack(Attack.Orientation.REGULAR, Attack.Input._5AAA);
                    //atkReg5AAAA = new Attack(Attack.Orientation.REGULAR, Attack.Input._5AAAA);

                    //atkReg2A = new Attack(Attack.Orientation.REGULAR, Attack.Input._2A);
                    //atkReg2AA = new Attack(Attack.Orientation.REGULAR, Attack.Input._2AA);

                    //atkGfy2A = new Attack(Attack.Orientation.GOOFY, Attack.Input._2A);

                    //atkReg5B = new Attack(Attack.Orientation.REGULAR, Attack.Input._5B);
                    //atkReg5BB = new Attack(Attack.Orientation.REGULAR, Attack.Input._5BB);
                    //atkReg5BBB = new Attack(Attack.Orientation.REGULAR, Attack.Input._5BBB);

                    //atkGfy5B = new Attack(Attack.Orientation.GOOFY, Attack.Input._5B);
                    //atkGfy5BB = new Attack(Attack.Orientation.GOOFY, Attack.Input._5BB);

                    //atkReg2B = new Attack(Attack.Orientation.REGULAR, Attack.Input._2B);

                    //atkGfy2B = new Attack(Attack.Orientation.GOOFY, Attack.Input._2B);

                    atkReg5A = new Attack(atkBuilder => { atkBuilder
                        .Name("regular_5A")
                        .Orientation(Character.Orientation.REGULAR)
                        .Input(Input.InputNotation._5A)
                        .AnimationState("regular_5A")
                        .Frames(attack => {
                            // Testing Scope
                            atkBuilder.Name("TOAST");
                            // Testing Scope

                            attack.AddFrames(
                                FrameUtil.CreateList(f => { f
                                    .SpecialCancellable(true)
                                    .From(1)
                                        .HitBox()
                                        .ChainCancellable(false)
                                    .To(8)
                                    .From(6)
                                        .ChainCancellable(true)
                                    .From(8)
                                        .HitBox(h => {
                                            h.Add(new Vector3(0, 0, 0));
                                            h.Add(new Vector3(0, 0, 1));
                                            h.Event("hit", () => {
                                                Debug.Log("Regular 5A Hit");
                                            });
                                        })
                                        .Damage(800)
                                        .HitStun(20.0f)
                                        .BlockStun(10.0f)
                                    .To(10)
                                    .From(10)
                                        .HitBox()
                                    .To(16);
                            }));
                        });
                    });

                    atkReg5AA = new Attack(atkBuilder => { atkBuilder
                        .Name("regular_5AA")
                        .Orientation(Character.Orientation.REGULAR)
                        .Input(Input.InputNotation._5AA)
                        .AnimationState("regular_5AA")
                        .Frames(Attack => {
                            Attack.AddFrames(
                                FrameUtil.CreateList(f => { f
                                    .SpecialCancellable(true)
                                    .From(1)
                                        .HitBox()
                                        .ChainCancellable(false)
                                    .To(12)
                                    .From(10)
                                        .ChainCancellable(true)
                                    .From(12)
                                        .HitBox(h => {
                                            h.Add(new Vector3(0, 0, 0));
                                            h.Add(new Vector3(0, 0, 1));
                                            h.Event("hit", () => {
                                                Debug.Log("Regular 5AA hit");
                                                // TODO: Figure out what to do with these events
                                            });
                                        })
                                    .To(14)
                                    .From(14)
                                        .HitBox()
                                    .To(20);
                            }));
                        });
                    });

                    atkGfy5A = new Attack(atkBuilder => { atkBuilder
                        .Name("goofy_5A")
                        .Orientation(Character.Orientation.GOOFY)
                        .Input(Input.InputNotation._5A)
                        .AnimationState("goofy_5A")
                        .Frames(attack => {
                            attack.AddFrames(
                                FrameUtil.CreateList(f => { f
                                    .SpecialCancellable(true)
                                    .From(1)
                                        .HitBox()
                                        .ChainCancellable(false)
                                    .To(8)
                                    .From(6)
                                        .ChainCancellable(true)
                                    .From(8)
                                        .HitBox(h => {
                                            h.Add(new Vector3(0, 0, 0));
                                            h.Add(new Vector3(0, 0, 1));
                                            h.Event("hit", () => {
                                                Debug.Log("Goofy 5A Hit");
                                            });
                                        })
                                        .Damage(800)
                                        .HitStun(20.0f)
                                        .BlockStun(10.0f)
                                    .To(10)
                                    .From(10)
                                        .HitBox()
                                    .To(16);
                            }));
                        });
                    });

                    atkGfy5AA = new Attack(atkBuilder => { atkBuilder
                        .Name("goofy_5AA")
                        .Orientation(Character.Orientation.GOOFY)
                        .Input(Input.InputNotation._5AA)
                        .AnimationState("goofy_5AA")
                        .Frames(Attack => {
                            Attack.AddFrames(
                                FrameUtil.CreateList(f => { f
                                    .SpecialCancellable(true)
                                    .From(1)
                                        .HitBox()
                                        .ChainCancellable(false)
                                    .To(12)
                                    .From(10)
                                        .ChainCancellable(true)
                                    .From(12)
                                        .HitBox(h => {
                                            h.Add(new Vector3(0, 0, 0));
                                            h.Add(new Vector3(0, 0, 1));
                                            h.Event("hit", () => {
                                                Debug.Log("Goofy 5AA hit");
                                                // TODO: Figure out what to do with these events
                                            });
                                        })
                                    .To(14)
                                    .From(14)
                                        .HitBox()
                                    .To(20);
                            }));
                        });
                    });

                    atkGfy5AhA = new Attack(atkBuilder => { atkBuilder
                        .Name("goofy_5A[A]")
                        .Orientation(Character.Orientation.GOOFY)
                        .Input(Input.InputNotation._5AhA)
                        .AnimationState("goofy_5A[A]")
                        .Frames(Attack => {
                            Attack.AddFrames(
                                FrameUtil.CreateList(f => { f
                                    .SpecialCancellable(true)
                                    .From(1)
                                        .HitBox()
                                        .ChainCancellable(false)
                                    .To(12)
                                    .From(10)
                                        .ChainCancellable(true)
                                    .From(12)
                                        .HitBox(h => {
                                            h.Add(new Vector3(0, 0, 0));
                                            h.Add(new Vector3(0, 0, 1));
                                            h.Event("hit", () => {
                                                Debug.Log("Goofy 5AA hit");
                                                // TODO: Figure out what to do with these events
                                            });
                                        })
                                    .To(14)
                                    .From(14)
                                        .HitBox()
                                    .To(20);
                            }));
                        });
                    });

                    charBuilder
                        .Attack(atkReg5A)
                        .Attack(atkReg5AA)
                        .Attack(atkGfy5A)
                        .Attack(atkGfy5AA)
                        .Attack(atkGfy5AhA);

                    //charBuilder.Adapter("1.0", "1.2", new Version1_2Adapter());
                }
            }
        }
    }
}