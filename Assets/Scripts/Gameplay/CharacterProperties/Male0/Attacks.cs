using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Character;

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

                public Attack atkReg2A;
                public Attack atkReg2AA;

                public Attack atkGfy2A;

                public Attack atkReg5B;
                public Attack atkReg5BB;
                public Attack atkReg5BBB;

                public Attack atkReg2B;

                public Attack atkGfy2B;

                public void Init(CharacterPropertiesBuilder builder) {
                    atkReg5A = new Attack(Attack.Orientation.REGULAR, Attack.Input._5A);
                    atkReg5AA = new Attack(Attack.Orientation.REGULAR, Attack.Input._5AA);
                    atkReg5AAA = new Attack(Attack.Orientation.REGULAR, Attack.Input._5AAA);
                    atkReg5AAAA = new Attack(Attack.Orientation.REGULAR, Attack.Input._5AAAA);

                    atkGfy5A = new Attack(Attack.Orientation.GOOFY, Attack.Input._5A);
                    atkGfy5AA = new Attack(Attack.Orientation.GOOFY, Attack.Input._5AA);

                    atkReg2A = new Attack(Attack.Orientation.REGULAR, Attack.Input._2A);
                    atkReg2AA = new Attack(Attack.Orientation.REGULAR, Attack.Input._2AA);

                    atkGfy2A = new Attack(Attack.Orientation.GOOFY, Attack.Input._2A);

                    atkReg5B = new Attack(Attack.Orientation.REGULAR, Attack.Input._5B);
                    atkReg5BB = new Attack(Attack.Orientation.REGULAR, Attack.Input._5BB);
                    atkReg5BBB = new Attack(Attack.Orientation.REGULAR, Attack.Input._5BBB);

                    atkGfy5B = new Attack(Attack.Orientation.GOOFY, Attack.Input._5B);
                    atkGfy5BB = new Attack(Attack.Orientation.GOOFY, Attack.Input._5BB);

                    atkReg2B = new Attack(Attack.Orientation.REGULAR, Attack.Input._2B);

                    atkGfy2B = new Attack(Attack.Orientation.GOOFY, Attack.Input._2B);

                    atkReg5A = new Attack(atkBuilder => { atkBuilder
                        .Name("5A")
                        .Orientation(Attack.Orientation.REGULAR)
                        .Input(Attack.Input._5A)
                        .AnimationState("regular_5A")
                        .Frames(attack => {
                            attack.AddFrames(
                                Frame.CreateList(f => { f
                                    .SpecialCancellable(true)
                                    .From(1)
                                        .HitBox(null)
                                        .ChainCancellable(false)
                                    .To(8)
                                    .From(6)
                                        .ChainCancellable(true)
                                    .From(8)
                                        .HitBox(new Vector3(0, 0, 0), new Vector3(0, 0, 1))
                                        .Damage(800)
                                        .HitStun(20.0f)
                                        .BlockStun(10.0f)
                                    .To(10)
                                    .From(10)
                                        .HitBox(null)
                                    .To(16);
                            }));
                        });
                    });

                    atkReg5AA = new Attack(atkBuilder => { atkBuilder
                        .Name("5AA")
                        .Orientation(Attack.Orientation.REGULAR)
                        .Input(Attack.Input._5AA)
                        .AnimationState("regular_5AA")
                        .Frames(Attack => {
                            Attack.AddFrames(
                                Frame.CreateList(f => { f
                                    .SpecialCancellable(true)
                                    .From(1)
                                        .HitBox(null)
                                        .ChainCancellable(false)
                                    .To(12)
                                    .From(10)
                                        .ChainCancellable(true)
                                    .From(12)
                                        .HitBox(new Vector3(0, 0, 0), new Vector3(0, 0, 1))
                                    .To(14)
                                    .From(14)
                                        .HitBox(null)
                                    .To(20);
                            }));
                        });
                    });
                }
            }
        }
    }
}