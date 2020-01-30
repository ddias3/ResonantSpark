// Attack.cs wishlist

using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

                private IFightingGameService fgService;
                private IAudioService audioService;
                private IProjectileService projectileService;

                private Dictionary<string, AudioClip> audioMap;
                private Dictionary<string, AnimationCurveCallback> attackMovementMap;

                                        // Action<> attackMovementCallback = (frame) => {
                                        //     return animationCurve.Evaluate(frame);
                                        // };

                public Attacks(AllServices services, Dictionary<string, AudioClip> audioMap) {
                    fgService = services.GetService<IFightingGameService>();
                    audioService = services.GetService<IAudioService>();
                    projectileService = services.GetService<IProjectileService>();

                    this.audioMap = audioMap;
                }

                private void ReturnToPreviousState(CharacterStates.BaseState prevState) {
                    fgChar.SetState(prevState);
                }

                public void Init(ICharacterPropertiesCallbackObj charBuilder, FightingGameCharacter fgChar) {

                    Attack atkSpw5A = new Attack(atkBuilder => { atkBuilder
                        .Name("southpaw_5A")
                        .Orientation(Orientation.SOUTHPAW)
                        .GroundRelation(GroundRelation.GROUNDED)
                        .Input(InputNotation._4A, InputNotation._5A)
                        .AnimationState("southpaw_5A")
                        .Movement(attackMovementMap["southpaw_5A.x"], null, attackMovementMap["southpaw_5A.z"])
                        .Frames(
                            FrameUtil.CreateList(f => { f
                                .SpecialCancellable(true)
                                .ChainCancellable(false)
                                .From(1)
                                    .HitBox()
                                .To(8)
                                .From(8)
                                    .Hit(hit => {
                                        hit.HitDamage(800)
                                            .BlockDamage(0)
                                            .HitStun(20.0f)
                                            .BlockStun(8.0f);

                                        // hit.Block(); default is to not require any kind of blocking

                                        hit.HitBox(hb => {
                                            hb.Relative(fgChar.transform);
                                            hb.Point0(new Vector3(0, 0, 0));
                                            hb.Point1(new Vector3(0, 1, 0));
                                            hb.Radius(0.25f);
                                            hb.Event("onHurtBox", (hitInfo) => {
                                                if (hitInfo.hitEntity != fgChar) {
                                                    audioService.PlayOneShot(hitInfo.position, audioMap["weakHit"]);
                                                        // This exists to make characters hitting each other async
                                                    fgService
                                                        .Hit(hitInfo.hitEntity)
                                                        .By(hitInfo.hitBox);
                                                }
                                            });
                                        });
                                    })
                                .To(10)
                                .From(10)
                                    .HitBox()
                                    .ChainCancellable(true)
                                .To(22);
                            }))
                        .OnComplete(ReturnToPreviousState);
                    });

                    Attack atkSpw5AA = new Attack(atkBuilder => { atkBuilder
                        .Name("southpaw_5AA")
                        .Orientation(Orientation.SOUTHPAW)
                        .GroundRelation(GroundRelation.GROUNDED)
                        .Input(InputNotation._5A)
                        .AnimationState("southpaw_5AA")
                        .Movement(attackMovementMap["southpaw_5AA.x"], null, attackMovementMap["southpaw_5AA.z"])
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
                                    .Hit(hit => {
                                        hit.HitDamage(1000)
                                            .BlockDamage(0)
                                            .HitStun(30.0f)
                                            .BlockStun(12.0f);

                                        hit.HitBox(hb => {
                                            hb.Relative(fgChar.transform);
                                            hb.Point0(new Vector3(0, 0, 0));
                                            hb.Point1(new Vector3(0, 1, 0));
                                            hb.Radius(0.25f);
                                            hb.Event("onHurtBox", (hitInfo) => {
                                                if (hitInfo.hitEntity != fgChar) {
                                                    audioService.PlayOneShot(hitInfo.position, audioMap["mediumHit"]);
                                                    fgService
                                                        .Hit(hitInfo.hitEntity)
                                                        .By(hitInfo.hitBox);
                                                }
                                            });
                                        });
                                    })
                                .To(14)
                                .From(14)
                                    .HitBox()
                                .To(30);
                            }))
                        .OnComplete(ReturnToPreviousState);
                    });

                    Attack atkSpw5AAA = new Attack(atkBuilder => { atkBuilder
                        .Name("southpaw_5AAA")
                        .Orientation(Orientation.SOUTHPAW)
                        .GroundRelation(GroundRelation.GROUNDED)
                        .Input(InputNotation._5A)
                        .AnimationState("southpaw_5AAA")
                        .Movement(attackMovementMap["southpaw_5AAA.x"], null, attackMovementMap["southpaw_5AAA.z"])
                        .Frames(
                            FrameUtil.CreateList(f => { f
                                .SpecialCancellable(true)
                                .From(1)
                                    .HitBox()
                                    .ChainCancellable(false)
                                .To(20)
                                .From(20)
                                    .Hit(hit => {
                                        hit.HitDamage(1000)
                                            .BlockDamage(0)
                                            .HitStun(30.0f)
                                            .BlockStun(12.0f);

                                        hit.HitBox(hb => {
                                            hb.Relative(fgChar.transform);
                                            hb.Point0(new Vector3(0, 0, 0));
                                            hb.Point1(new Vector3(0, 1, 0));
                                            hb.Radius(0.25f);
                                            hb.Event("onHurtBox", (hitInfo) => {
                                                if (hitInfo.hitEntity != fgChar) {
                                                    audioService.PlayOneShot(hitInfo.position, audioMap["mediumHit"]);
                                                    fgService
                                                        .Hit(hitInfo.hitEntity)
                                                        .By(hitInfo.hitBox);
                                                }
                                            });
                                        });
                                    })
                                .To(21)
                                .From(22)
                                    .Rotate((localFrame, target) => { // localFrame is an integer between 22 and 30 (in this case), target is a Transform
                                        float p = (localFrame - 22.0f) / 30.0f;
                                        fgChar.SetRotation(Quaternion.Euler(0.0f, -180f * p, 0.0f) * (fgChar.rigidbody.position - target.position));
                                            // fgChar.GetOrientation is done automatically on each frame
                                    })
                                .To(30);
                            }))
                        .OnComplete(ReturnToPreviousState);
                    });

                    Attack atkOrt5A = new Attack(atkBuilder => { atkBuilder
                        .Name("orthodox_5A")
                        .Orientation(Orientation.ORTHODOX)
                        .GroundRelation(GroundRelation.GROUNDED)
                        .Input(InputNotation._4A, InputNotation._5A)
                        .AnimationState("orthodox_5A")
                        .Movement(attackMovementMap["orthodox_5A.x"], null, attackMovementMap["orthodox_5A.z"])
                        .Frames(
                            FrameUtil.CreateList(f => { f
                                .SpecialCancellable(true)
                                .ChainCancellable(false)
                                .From(1)
                                    .HitBox()
                                .To(10)
                                .From(10)
                                    .Hit(hit => {
                                        hit.HitDamage(900)
                                            .BlockDamage(0)
                                            .HitStun(24.0f)
                                            .BlockStun(10.0f);

                                        hit.HitBox(hb => {
                                            hb.Relative(fgChar.transform);
                                            hb.Point0(new Vector3(0, 0, 0));
                                            hb.Point1(new Vector3(0, 1, 0));
                                            hb.Radius(0.25f);
                                            hb.Event("onHurtBox", (hitInfo) => {
                                                if (hitInfo.hitEntity != fgChar) {
                                                    audioService.PlayOneShot(hitInfo.position, audioMap["weakHit"]);
                                                        // This exists to make characters hitting each other async
                                                    fgService
                                                        .Hit(hitInfo.hitEntity)
                                                        .By(hitInfo.hitBox);
                                                }
                                            });
                                        });
                                    })
                                .To(10)
                                .From(10)
                                    .HitBox()
                                    .ChainCancellable(true)
                                .To(22);
                            }))
                        .OnComplete(ReturnToPreviousState);
                    });

                    Attack atkOrt5AA = new Attack(atkBuilder => { atkBuilder
                        .Name("orthodox_5AA")
                        .Orientation(Orientation.ORTHODOX)
                        .GroundRelation(GroundRelation.GROUNDED)
                        .Input(InputNotation._5A)
                        .AnimationState("orthodox_5AA")
                        .Movement(attackMovementMap["orthodox_5AA.x"], null, attackMovementMap["orthodox_5AA.z"])
                        .Frames(
                            FrameUtil.CreateList(f => { f
                                .SpecialCancellable(true)
                                .ChainCancellable(false)
                                .From(1)
                                    .HitBox()
                                .To(16)
                                .From(16)
                                    .Hit(hit => {
                                        hit.HitDamage(1200)
                                            .BlockDamage(0)
                                            .HitStun(24.0f)
                                            .BlockStun(10.0f);

                                        hit.HitBox(hb => {
                                            hb.Relative(fgChar.transform);
                                            hb.Point0(new Vector3(0, 0, 0));
                                            hb.Point1(new Vector3(0, 1, 0));
                                            hb.Radius(0.25f);
                                            hb.Event("onHurtBox", (hitInfo) => {
                                                if (hitInfo.hitEntity != fgChar) {
                                                    audioService.PlayOneShot(hitInfo.position, audioMap["mediumHit"]);
                                                        // This exists to make characters hitting each other async
                                                    fgService
                                                        .Hit(hitInfo.hitEntity)
                                                        .By(hitInfo.hitBox);
                                                }
                                            });
                                        });
                                    })
                                .To(17)
                                .From(17)
                                    .HitBox()
                                .From(20)
                                    .Rotate((localFrame, target) => { // localFrame is an integer between 20 and 26 (in this case), target is a Transform
                                        float p = (localFrame - 20.0f) / 26.0f;
                                        fgChar.SetRotation(Quaternion.Euler(0.0f, 180f * p, 0.0f) * (fgChar.rigidbody.position - target.position));
                                            // fgChar.GetOrientation is done automatically on each frame
                                    })
                                .To(26);
                            }))
                        .OnComplete(ReturnToPreviousState);
                    });

                    Attack atkSpw2A = new Attack(atkBuilder => { atkBuilder
                        .Name("southpaw_2A")
                        .Orientation(Orientation.SOUTHPAW)
                        .GroundRelation(GroundRelation.GROUNDED)
                        .Input(InputNotation._2A)
                        .AnimationState("southpaw_2A")
                        .Movement(attackMovementMap["southpaw_2A.x"], null, attackMovementMap["southpaw_2A.z"])
                        .Frames(
                            FrameUtil.CreateList(f => { f
                                .SpecialCancellable(true)
                                .ChainCancellable(false)
                                .From(1)
                                    .HitBox()
                                .To(8)
                                .From(8)
                                    .Hit(hit => {
                                        hit.HitDamage(600)
                                            .BlockDamage(0)
                                            .HitStun(10.0f)
                                            .BlockStun(8.0f);

                                        hit.Block(Block.LOW);

                                        hit.HitBox(hb => {
                                            hb.Relative(fgChar.transform);
                                            hb.Point0(new Vector3(0, 0, 0));
                                            hb.Point1(new Vector3(0, 1, 0));
                                            hb.Radius(0.25f);
                                            hb.Event("onHurtBox", (hitInfo) => {
                                                if (hitInfo.hitEntity != fgChar) {
                                                    audioService.PlayOneShot(hitInfo.position, audioMap["weakHit"]);
                                                        // This exists to make characters hitting each other async
                                                    fgService
                                                        .Hit(hitInfo.hitEntity)
                                                        .By(hitInfo.hitBox);    // the hitBox should know what hit it's a part of.
                                                }
                                            });
                                        });
                                    })
                                .To(9)
                                .From(9)
                                    .HitBox()
                                .From(14)
                                    .ChainCancellable(true)
                                .To(22);
                            }))
                        .OnComplete(ReturnToPreviousState);
                    });

                    Attack atkSpw2AA = new Attack(atkBuilder => { atkBuilder
                        .Name("southpaw_2AA")
                        .Orientation(Orientation.SOUTHPAW)
                        .GroundRelation(GroundRelation.GROUNDED)
                        .Input(InputNotation._2A)
                        .AnimationState("southpaw_2AA")
                        .Movement(attackMovementMap["southpaw_2AA.x"], null, attackMovementMap["southpaw_2AA.z"])
                        .Frames(
                            FrameUtil.CreateList(f => { f
                                .SpecialCancellable(true)
                                .ChainCancellable(false)
                                .From(1)
                                    .HitBox()
                                .To(13)
                                .From(13)
                                    .Hit(hit => {
                                        hit.HitDamage(800)
                                            .BlockDamage(0)
                                            .HitStun(20.0f)
                                            .BlockStun(20.0f);

                                        hit.Block(Block.LOW);

                                        hit.HitBox(hb => {
                                            hb.Relative(fgChar.transform);
                                            hb.Point0(new Vector3(0, 0, 0));
                                            hb.Point1(new Vector3(0, 1, 0));
                                            hb.Radius(0.25f);
                                            hb.Event("onHurtBox", (hitInfo) => {
                                                if (hitInfo.hitEntity != fgChar) {
                                                    audioService.PlayOneShot(hitInfo.position, audioMap["weakHit"]);
                                                        // This exists to make characters hitting each other async
                                                    fgService
                                                        .Hit(hitInfo.hitEntity)
                                                        .By(hitInfo.hitBox);
                                                }
                                            });
                                        });
                                    })
                                .To(14)
                                .From(14)
                                    .HitBox()
                                .To(28);
                            }))
                        .OnComplete((prevState) => {
                            fgChar.SetState(fgChar.State("stand"));
                        });
                    });

                    Attack atkOrt2A = new Attack(atkBuilder => { atkBuilder
                        .Name("orthodox_2A")
                        .Orientation(Orientation.ORTHODOX)
                        .GroundRelation(GroundRelation.GROUNDED)
                        .Input(InputNotation._2A)
                        .AnimationState("orthodox_2A")
                        .Movement(attackMovementMap["orthodox_2A.x"], null, attackMovementMap["orthodox_2A.z"])
                        .Frames(
                            FrameUtil.CreateList(f => { f
                                .SpecialCancellable(true)
                                .ChainCancellable(false)
                                .From(1)
                                    .HitBox()
                                .To(12)
                                .From(12)
                                    .Hit(hit => {
                                        hit.HitDamage(600)
                                            .BlockDamage(0)
                                            .HitStun(12.0f)
                                            .BlockStun(8.0f);

                                        hit.HitBox(hb => {
                                            hb.Relative(fgChar.transform);
                                            hb.Point0(new Vector3(0, 0, 0));
                                            hb.Point1(new Vector3(0, 1, 0));
                                            hb.Radius(0.25f);
                                            hb.Event("onHurtBox", (hitInfo) => {
                                                if (hitInfo.hitEntity != fgChar) {
                                                    audioService.PlayOneShot(hitInfo.position, audioMap["weakHit"]);
                                                        // This exists to make characters hitting each other async
                                                    fgService
                                                        .Hit(hitInfo.hitEntity)
                                                        .By(hitInfo.hitBox);
                                                }
                                            });
                                        });
                                    })
                                .To(13)
                                .From(13)
                                    .HitBox()
                                .From(15)
                                    .ChainCancellable(true)
                                .To(23);
                            }))
                        .OnComplete(ReturnToPreviousState);
                    });

                    Attack hadouken = new Attack(atkBuilder => { atkBuilder
                        .Name("hadouken")
                        .Orientation(Orientation.SOUTHPAW)
                        .GroundRelation(GroundRelation.GROUNDED)
                        .Input(InputNotation._236C)
                        .AnimationState("hadouken")
                        .Movement(null, null, attackMovementMap["hadouken.z"])
                        .Frames(
                            FrameUtil.CreateList(f => { f
                                .SpecialCancellable(false)
                                .ChainCancellable(false)
                                .From(1)
                                    .HitBox()
                                .From(3)
                                    .Sound(audioMap["hadouken"], soundResource => {
                                        soundResource.transform.position = fgChar.GetSpeakPosition();
                                        audioService.Play(soundResource);
                                    })
                                .From(12)
                                    .Projectile(projectileMap["hadouken"], proj => {
                                        projectileService.FireProjectile(proj.id);
                                    })
                                .To(46);
                            }))
                        .OnComplete(ReturnToPreviousState);
                    });

                    charBuilder
                        .Attack(atkSpw5A, (charState, currAttack, prevAttacks) => { // prevAttacks is a List<Attack>
                            if (prevAttacks.Where(prev => prev == atkSpw5A).Count() >= 3) {
                                return false;
                            }
                            if (currAttack == null ||
                                currAttack == atkSpw2A ||
                                currAttack == atkSpw2AA) {
                                return true;
                            }
                            return false;
                        })
                        .Attack(atkSpw5AA, (charState, currAttack, prevAttacks) => {
                            if (currAttack == atkSpw5A) {
                                return true;
                            }
                            return false;
                        })
                        .Attack(atkSpw5AAA, (charState, currAttack, prevAttacks) => {
                            if (currAttack == atkSpw5AA) {
                                return true;
                            }
                            return false;
                        })
                        .Attack(atkOrt5A, (charState, currAttack, prevAttacks) => {
                            if (prevAttacks.Where(prev => prev == atkOrt5A).Count() >= 2) {
                                return false;
                            }
                            if (currAttack == null ||
                                currAttack == atkOrt5A ||
                                currAttack == atkOrt2A) {
                                return true;
                            }
                            return false;
                        })
                        .Attack(atkOrt5AA, (charState, currAttack, prevAttacks) => {
                            if (currAttack == atkOrt5A) {
                                return true;
                            }
                            return false;
                        })
                        .Attack(atkSpw2A, (charState, currAttack, prevAttacks) => {
                            if (prevAttacks.Where(prev => prev == atkSpw5A).Count() > 1 ||
                                prevAttacks.Contains(atkSpw2A)) {
                                return false;
                            }

                            if (currAttack == null ||
                                currAttack == atkSpw5A) {
                                return true;
                            }
                            return false;
                        })
                        .Attack(atkSpw2AA, (charState, currAttack, prevAttacks) => {
                            if (currAttack == atkSpw2A) {
                                return true;
                            }
                            return false;
                        })
                        .Attack(atkOrt2A, (charState, currAttack, prevAttacks) => {
                            if (prevAttacks.Where(prev => prev == atkOrt5A).Count() > 1 ||
                                prevAttacks.Contains(atkOrt2A)) {
                                return false;
                            }

                            if (currAttack == null ||
                                currAttack == atkOrt2A ||
                                currAttack == atkOrt5A) {
                                return true;
                            }
                            return false;
                        })
                        .Attack(hadouken, (charState, currAttack, prevAttacks) => {
                            if (!prevAttacks.Contains(hadouken)) {
                                return true;
                            }
                            return false;
                        });
                }
            }
        }
    }
}