using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Builder;
using ResonantSpark.Utility;
using ResonantSpark.Input;
using ResonantSpark.Character;
using ResonantSpark.Gameplay;
using ResonantSpark.Service;

namespace ResonantSpark {
    namespace CharacterProperties {
        namespace Male0 {
            public class Attacks {

                private IAudioService audioService;
                private IProjectileService projectileService;
                private Dictionary<string, AudioClip> audioMap;

                public Attacks(AllServices services, Dictionary<string, AudioClip> audioMap) {
                    audioService = services.GetService<IAudioService>();
                    projectileService = services.GetService<IProjectileService>();
                    this.audioMap = audioMap;
                }

                public void Init(ICharacterPropertiesCallbackObj charBuilder, FightingGameCharacter fgChar) {

                    Attack atkReg5A = new Attack(atkBuilder => { atkBuilder
                        .Name("regular_5A")
                        .Orientation(Orientation.REGULAR)
                        .GroundRelation(GroundRelation.GROUNDED)
                        .Input(InputNotation._5A)
                        .AnimationState("regular_5A")
                        .Frames(
                            FrameUtil.CreateList(f => { f
                                .SpecialCancellable(true)
                                .ChainCancellable(false)
                                .From(1)
                                    .HitBox()
                                .To(8)
                                .From(8)
                                    .HitBox(h => {
                                        h.Relative(fgChar.transform);
                                        h.Point0(new Vector3(0, 0, 0));
                                        h.Point1(new Vector3(0, 1, 0));
                                        h.Radius(0.25f);
                                        h.Event("onHurtBox", (hitInfo) => {
                                            if (hitInfo.hitEntity != fgChar) {
                                                Debug.Log("Regular 5A hit enemey");
                                                audioService.PlayOneShot(hitInfo.position, audioMap["regular_5A"]);
                                                Debug.Log("Regular 5A Hit");
                                                // TODO: audioService.Play("male0_regular_5AA")
                                                //                   .At(hitInfo.position); or .Follow(/*supply a trasform*/);
                                                //          change it to -> audioService.Play(soundMap.Get("regular_5A"));
                                                hitInfo.hitEntity.GetHitBy(hitInfo.hitBox);
                                            }
                                        });
                                    })
                                    .HitDamage(800)
                                    .BlockDamage(0)
                                    .HitStun(20.0f)
                                    .BlockStun(10.0f)
                                .To(10)
                                .From(10)
                                    .HitBox()
                                    .ChainCancellable(true)
                                .To(22);
                            })
                        );
                    });

                    Attack atkReg5AA = new Attack(atkBuilder => { atkBuilder
                        .Name("regular_5AA")
                        .Orientation(Orientation.REGULAR)
                        .GroundRelation(GroundRelation.GROUNDED)
                        .Input(InputNotation._5A)
                        .AnimationState("regular_5AA")
                        .Frames(
                            FrameUtil.CreateList(f => { f
                                .SpecialCancellable(true)
                                .From(1)
                                    .HitBox()
                                    .ChainCancellable(false)
                                .To(12)
                                .From(10)
                                    .ChainCancellable(true)
                                .From(12)
                                    // This is a possible way to program the configuration code. I think this has its merits,
                                    //   but having damage values statically defined also has its benefits
                                    //.HitBox(h => {
                                    //    h.Point0(new Vector3(0, 0, 0));
                                    //    h.Point1(new Vector3(0, 1, 0));
                                    //    h.Radius(0.25f);
                                    //    h.Event("onHurtBox", (fgChar) => {
                                    //        Debug.Log("Regular 5AA hit the enemy");
                                    //        // TODO: Figure out what to do with these events
                                    //        fgChar.Hit(hitDamage: 800, blockDamage: 0, hitStun: 20.0f, blockStun: 10.0f);
                                    //    });
                                    //    h.Event("onHitBox", (fgChar) => {
                                    //        Debug.Log("Regular 5AA hit another hitbox");
                                    //        // TODO: Figure out what to do with these events
                                    //    });
                                    //})
                                    .HitBox(h => {
                                        h.Relative(fgChar.gameObject.transform);
                                        h.Point0(new Vector3(0, 0, 0));
                                        h.Point1(new Vector3(0, 1, 0));
                                        h.Radius(0.25f);
                                        h.Event("onHurtBox", (hitInfo) => {
                                            Debug.Log("Regular 5AA hit the enemy");
                                            // TODO: audioService.Play("male0_regular_5AA")
                                            //                   .At(hitInfo.position); or .Follow(/*supply a trasform*/);
                                            //          change it to -> audioService.Play(soundMap.Get("regular_5A"));
                                            if (hitInfo.hitEntity != fgChar) {
                                                hitInfo.hitEntity.GetHitBy(hitInfo.hitBox);
                                            }
                                        });
                                        h.Event("onHitBox", (hitInfo) => {
                                            Debug.Log("Regular 5AA hit another hitbox");
                                            // TODO: audioService.Play("male0_regular_5AA")
                                            //                   .At(hitInfo.position); or .Follow(/*supply a trasform*/);
                                            //          change it to -> audioService.Play(soundMap.Get("regular_5A"));
                                            if (hitInfo.hitEntity != fgChar) {
                                                hitInfo.hitEntity.GetHitBy(hitInfo.hitBox);
                                            }
                                        });
                                    })
                                    .HitDamage(1000)
                                    .BlockDamage(10)
                                    .HitStun(30.0f)
                                    .BlockStun(12.0f)
                                .To(10)
                                //.To(14)
                                .From(14)
                                    .HitBox()
                                .To(30);
                            })
                        );
                    });

                    Attack atkGfy5A = new Attack(atkBuilder => { atkBuilder
                        .Name("goofy_5A")
                        .Orientation(Orientation.GOOFY)
                        .GroundRelation(GroundRelation.GROUNDED)
                        .Input(InputNotation._5A)
                        .AnimationState("goofy_5A")
                        .Frames(
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
                                        h.Relative(fgChar.gameObject.transform);
                                        h.Point0(new Vector3(0, 0, 0));
                                        h.Point1(new Vector3(0, 1, 0));
                                        h.Radius(0.25f);
                                        h.Event("onHurtBox", (hitInfo) => {
                                            Debug.Log("Goofy 5A hit the enemy");
                                            // TODO: audioService.Play("male0_goofy_5A")
                                            //                   .At(hitInfo.position); or .Follow(/*supply a trasform*/);
                                            //          change it to -> audioService.Play(soundMap.Get("regular_5A"));
                                            if (hitInfo.hitEntity != fgChar) {
                                                hitInfo.hitEntity.GetHitBy(hitInfo.hitBox);
                                            }
                                        });
                                    })
                                    .HitDamage(800)
                                    .BlockDamage(50)
                                    .HitStun(20.0f)
                                    .BlockStun(10.0f)
                                .To(10)
                                .From(10)
                                    .HitBox()
                                .To(16);
                            })
                        );
                    });

                    Attack atkGfy5AA = new Attack(atkBuilder => { atkBuilder
                        .Name("goofy_5AA")
                        .Orientation(Orientation.GOOFY)
                        .GroundRelation(GroundRelation.GROUNDED)
                        .Input(InputNotation._5A)
                        .AnimationState("goofy_5AA")
                        .Frames(
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
                                        h.Relative(fgChar.gameObject.transform);
                                        h.Point0(new Vector3(0, 0, 0));
                                        h.Point1(new Vector3(0, 1, 0));
                                        h.Radius(0.25f);
                                        h.Event("onHurtBox", (hitInfo) => {
                                            Debug.Log("Goofy 5AA hit the enemy");
                                            // TODO: audioService.Play("male0_goofy_5AA")
                                            //                   .At(hitInfo.position); or .Follow(/*supply a trasform*/);
                                            //          change it to -> audioService.Play(soundMap.Get("regular_5A"));
                                            if (hitInfo.hitEntity != fgChar) {
                                                hitInfo.hitEntity.GetHitBy(hitInfo.hitBox);
                                            }
                                        });
                                    })
                                .To(14)
                                .From(14)
                                    .HitBox()
                                .To(20);
                            })
                        );
                    });

                    Attack atkGfy5AAh = new Attack(atkBuilder => { atkBuilder
                        .Name("goofy_5A[A]")
                        .Orientation(Orientation.GOOFY)
                        .GroundRelation(GroundRelation.GROUNDED)
                        .Input(InputNotation._5Ah)
                        .AnimationState("goofy_5A[A]")
                        .Frames(
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
                                        h.Relative(fgChar.gameObject.transform);
                                        h.Point0(new Vector3(0, 0, 0));
                                        h.Point1(new Vector3(0, 1, 0));
                                        h.Radius(0.25f);
                                        h.Event("onHurtBox", (hitInfo) => {
                                            Debug.Log("Goofy 5A[A] hit the enemy");
                                            // TODO: Figure out what to do with these events
                                            if (hitInfo.hitEntity != fgChar) {
                                                hitInfo.hitEntity.GetHitBy(hitInfo.hitBox);
                                            }
                                        });
                                    })
                                .To(14)
                                .From(14)
                                    .HitBox()
                                .To(20);
                            })
                        );
                    });

                    //Attack hadouken = new Attack(atkBuilder => { atkBuilder
                    //    .Name("hadouken")
                    //    .Orientation(Orientation.REGULAR)
                    //    .GroundRelation(GroundRelation.GROUNDED)
                    //    .Input(InputNotation._5A)
                    //    .AnimationState("crouch_to_stand")
                    //    .Frames(
                    //        FrameUtil.CreateList(f => { f
                    //            .SpecialCancellable(false)
                    //            .ChainCancellable(false)
                    //            .From(1)
                    //                .HitBox()
                    //            .From(3)
                    //                .Sound(audioMap["hadouken"], soundResource => {
                    //                    soundResource.transform.position = fgChar.transform.position;
                    //                    audioService.Play(soundResource);
                    //                })
                    //                .Sound(audioMap["hadouken"])
                    //            .From(12)
                    //                .Projectile(projectileMap["hadouken"], proj => {
                    //                    projectileService.FireProjectile(proj.id);
                    //                })
                    //            .To(46);
                    //        })
                    //    );
                    //});

                    charBuilder
                        .Attack(atkReg5A, (charState, currAttack) => {
                            //if (charState.activeAttack == null ||
                            //    charState.activeAttack == charState.Notation(InputNotation._2A, Orientation.REGULAR, GroundRelation.CROUCH)) {
                            //          or
                            //    charState.activeAttack == charState.Name("regular_2A")) {
                            //    return true;
                            //}
                            if (currAttack == null) {
                                return true;
                            }
                            return false;
                        })
                        .Attack(atkReg5AA, (charState, currAttack) => {
                            if (currAttack == atkReg5A) {
                                return true;
                            }
                            return false;
                        })
                        .Attack(atkGfy5A, (charState, currAttack) => {
                            //if (charState.activeAttack == null ||
                            //    charState.activeAttack == charState.Notation(InputNotation._2A, Orientation.GOOFY, GroundRelation.CROUCH)) {
                            //    return true;
                            //}
                            if (currAttack == null) {
                                return true;
                            }
                            return false;
                        })
                        .Attack(atkGfy5AA, (charState, currAttack) => {
                            if (currAttack == atkGfy5A) {
                                return true;
                            }
                            return false;
                        })
                        .Attack(atkGfy5AAh, (charState, currAttack) => {
                            if (currAttack == atkGfy5A) {
                                return true;
                            }
                            return false;
                        });
                        //.Attack(hadouken, (charState, currAttack) => {
                        //    if (currAttack == null) {
                        //        return true;
                        //    }
                        //    return false;
                        //});

                    //charBuilder.Adapter("1.0", "1.2", new Version1_2Adapter());
                }
            }
        }
    }
}