using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using ResonantSpark.Builder;
using ResonantSpark.Utility;
using ResonantSpark.Input;
using ResonantSpark.Character;
using ResonantSpark.Gameplay;
using ResonantSpark.Service;
using ResonantSpark.Particle;

namespace ResonantSpark {
    namespace CharacterProperties {
        namespace Lawrence {
            public class Attacks {

                private IFightingGameService fgService;
                private IAudioService audioService;
                private IProjectileService projectileService;
                private IParticleService particleService;
                private ICameraService cameraService;
                private ITimeService timeService;

                private IHitBoxService hitBoxService;

                private Dictionary<string, AudioClip> audioMap;
                private Dictionary<string, ParticleEffect> particleMap;
                private Dictionary<string, AnimationCurve> animationCurveMap;
                private Dictionary<string, Projectile> projectileMap;

                private FightingGameCharacter fgChar;

                public Attacks(
                        AllServices services,
                        Dictionary<string, AudioClip> audioMap,
                        Dictionary<string, ParticleEffect> particleMap,
                        Dictionary<string, AnimationCurve> animationCurveMap,
                        Dictionary<string, Projectile> projectileMap) {
                    fgService = services.GetService<IFightingGameService>();
                    audioService = services.GetService<IAudioService>();
                    projectileService = services.GetService<IProjectileService>();
                    particleService = services.GetService<IParticleService>();
                    cameraService = services.GetService<ICameraService>();
                    timeService = services.GetService<ITimeService>();

                    hitBoxService = services.GetService<IHitBoxService>();

                    this.audioMap = audioMap;
                    this.particleMap = particleMap;
                    this.animationCurveMap = animationCurveMap;
                    this.projectileMap = projectileMap;
                }

                public void Init(ICharacterPropertiesCallbackObj charBuilder, FightingGameCharacter newFightingGameChar) {
                    fgChar = newFightingGameChar;

                    Attack atkOrt5A = new Attack(atkBuilder => {
                        atkBuilder.Name("orthodox_5A");
                        atkBuilder.Orientation(Orientation.ORTHODOX);
                        atkBuilder.GroundRelation(GroundRelation.GROUNDED);
                        atkBuilder.Input(InputNotation._4A, InputNotation._5A);
                        atkBuilder.AnimationState("5A");
                        atkBuilder.InitCharState((CharacterStates.Attack)fgChar.State("attackGrounded"));
                        atkBuilder.Movement(null, null, null);
                        atkBuilder.Frames(
                            FrameUtil.CreateList(fl => {
                                fl.SpecialCancellable(true);
                                fl.CancellableOnWhiff(false); // TODO: Change this to true when you fix the input buffer
                                fl.ChainCancellable(true);
                                fl.CounterHit(true);
                                fl.From(7);
                                    fl.Hit(hit => {
                                        hit.HitDamage(800);
                                        hit.BlockDamage(0);
                                        hit.HitStun(20.0f);
                                        hit.BlockStun(8.0f);
                                        hit.ComboScaling(0.65f);
                                        hit.Priority(AttackPriority.LightAttack);

                                        // hit.Block(); default is to allow any kind of blocking

                                        HitBox defaultHitBox = hitBoxService.Create(hb => {
                                            hb.Relative(fgChar.transform);
                                            hb.Point0(new Vector3(0, 0, 0.5f));
                                            hb.Point1(new Vector3(0, 1.1f, 1.3f));
                                            hb.Radius(0.25f);
                                            hb.InGameEntity(fgChar);
                                            hb.Validate((HitBox _, HitBox other) => true);
                                            hb.Validate((HitBox _, HurtBox other) => true);
                                        });

                                        hit.OnHit((thisHit, hitLocations, entity, hurtBoxes, hitBoxes) => {
                                            //CommonGroundedOnHitFGChar(hitSound: audioMap["weakHit"], groundedForceMagnitude: 0.5f, airborneForceDirection: new Vector3(0.0f, 1.0f, 1.0f), airborneForceMagnitude: 1.0f);
                                            if (fgService.IsCurrentFGChar(entity) && entity != fgChar) {
                                                FightingGameCharacter opponent = (FightingGameCharacter) entity;
                                                audioService.PlayOneShot(hitLocations[defaultHitBox], audioMap["weakHit"]);

                                                // This exists to make characters hitting each other async
                                                fgService.Hit(entity, fgChar, thisHit, (hitAtSameTimeByAttackPriority, damage) => {
                                                    opponent.ChangeHealth(damage); // damage includes combo scaling.

                                                    switch (opponent.GetGroundRelation()) {
                                                        case GroundRelation.GROUNDED:
                                                            opponent.KnockBack(
                                                                thisHit.priority,
                                                                launch: false,
                                                                knockback: fgChar.transform.rotation * Vector3.forward * 0.5f);
                                                            break;
                                                        case GroundRelation.AIRBORNE:
                                                            opponent.KnockBack(
                                                                thisHit.priority,
                                                                launch: true,
                                                                knockback: fgChar.transform.rotation * new Vector3(0.0f, 1.0f, 1.0f) * 0.7071f);
                                                            break;
                                                    }
                                                });
                                            }
                                        });

                                        hit.HitBox(defaultHitBox);
                                    });
                                fl.To(8);
                                fl.From(8);
                                    fl.CounterHit(false);
                                fl.From(10);
                                    fl.ChainCancellable(true);
                                fl.To(22);
                            }));
                        atkBuilder.CleanUp(ReturnToStand);
                    });

                    Attack atkOrt5AA = new Attack(atkBuilder => {
                        atkBuilder.Name("orthodox_5AA");
                        atkBuilder.Orientation(Orientation.ORTHODOX);
                        atkBuilder.GroundRelation(GroundRelation.GROUNDED);
                        atkBuilder.Input(InputNotation._4A, InputNotation._5A);
                        atkBuilder.AnimationState("5AA");
                        atkBuilder.InitCharState((CharacterStates.Attack)fgChar.State("attackGrounded"));
                        atkBuilder.Movement(zMoveCb: animationCurveMap["orth_5AA.z"].Evaluate);
                        atkBuilder.Frames(
                            FrameUtil.CreateList(fl => {
                                fl.SpecialCancellable(true);
                                fl.CancellableOnWhiff(false);
                                fl.CounterHit(true);
                                fl.From(1);
                                    fl.ChainCancellable(false);
                                fl.From(4);
                                    fl.Track((targetFG) => {
                                        Vector3 newTargetPos = TargetUtil.MoveTargetLimited(fgChar.position, targetFG.targetPos, targetFG.ActualTargetPos(), 20.0f);
                                            // for now, just let the setting of this value be instantaneous once per frame.
                                        fgChar.SetTarget(newTargetPos);
                                    });
                                fl.To(7);
                                fl.From(10);
                                    fl.ChainCancellable(true);
                                fl.From(11);
                                    fl.Hit(hit => {
                                        hit.HitDamage(1000);
                                        hit.BlockDamage(0);
                                        hit.HitStun(30.0f);
                                        hit.BlockStun(12.0f);
                                        hit.ComboScaling(1.0f);
                                        hit.Priority(AttackPriority.LightAttack);

                                        hit.HitBox(hitBoxService.Create(hb => {
                                            hb.Relative(fgChar.transform);
                                            hb.Point0(new Vector3(0, 0, 0.8f));
                                            hb.Point1(new Vector3(0, 1, 0.8f));
                                            hb.Radius(0.25f);
                                            hb.InGameEntity(fgChar);
                                            hb.Validate((HitBox _, HitBox other) => {
                                                return true;
                                            });
                                            hb.Validate((HitBox _, HurtBox other) => {
                                                return true;
                                            });
                                            //hb.Event("onHitFGChar",
                                            //    CommonGroundedOnHitFGChar(
                                            //        hitSound: audioMap["mediumHit"],
                                            //        groundedForceMagnitude: 0.5f,
                                            //        airborneForceDirection: new Vector3(0.0f, 1.0f, 1.0f),
                                            //        airborneForceMagnitude: 1.0f));

                                            //hb.Event("onEqualPriorityHitBox", (hitInfo) => {
                                            //    if (hitInfo.hitEntity != fgChar) {
                                            //        fgChar.PredeterminedActions("horizontalClashSwingFromLeft");
                                            //    }
                                            //});
                                        }));
                                    });
                                fl.To(14);
                                fl.From(14);
                                    fl.CounterHit(false);
                                fl.To(30);
                            }));
                        atkBuilder.CleanUp(ReturnToStand);
                    });

                    Attack atkOrt5AAA = new Attack(atkBuilder => {
                        atkBuilder.Name("orthodox_5AAA");
                        atkBuilder.Orientation(Orientation.ORTHODOX);
                        atkBuilder.GroundRelation(GroundRelation.GROUNDED);
                        atkBuilder.Input(InputNotation._4A, InputNotation._5A);
                        atkBuilder.AnimationState("5AAA");
                        atkBuilder.InitCharState((CharacterStates.Attack)fgChar.State("attackGrounded"));
                        //atkBuilder.FramesContinuous((localFrame, targetPos) => {
                        //    if (localFrame >= 22.0f && localFrame <= 30.0f) {
                        //        float p = (localFrame - 22.0f) / 30.0f;
                        //        Quaternion rot = Quaternion.Euler(0.0f, -180f * p + Vector3.SignedAngle(Vector3.right, fgChar.position - targetPos, Vector3.up), 0.0f);
                        //        Debug.Log(rot);
                        //        fgChar.SetRotation(rot);
                        //            // fgChar.GetOrientation is done automatically on each frame
                        //    }
                        //});
                        atkBuilder.Movement(zMoveCb: animationCurveMap["orth_5AAA.z"].Evaluate);
                        atkBuilder.Frames(
                            FrameUtil.CreateList(f => { f
                                .SpecialCancellable(true)
                                .CancellableOnWhiff(false)
                                .CounterHit(true)
                                .From(1)
                                    .ChainCancellable(false)
                                .To(20)
                                .From(14)
                                    .Hit(hit => {
                                        hit.HitDamage(1000)
                                            .BlockDamage(0)
                                            .HitStun(30.0f)
                                            .BlockStun(12.0f)
                                            .Tracking(true)
                                            .Priority(AttackPriority.MediumAttack);

                                        HitBox farHitBox = hitBoxService.Create(hb => {
                                            hb.Relative(fgChar.transform);
                                            hb.Point0(new Vector3(0, 0, 0.8f));
                                            hb.Point1(new Vector3(0, 1, 0.8f));
                                            hb.Radius(0.25f);
                                            hb.InGameEntity(fgChar);
                                            hb.Validate((HitBox _, HitBox other) => {
                                                return true;
                                            });
                                            hb.Validate((HitBox _, HurtBox other) => {
                                                return true;
                                            });
                                            //hb.Event("onHitFGChar",
                                            //    CommonGroundedOnHitFGChar(
                                            //        hitSound: audioMap["mediumHit"],
                                            //        groundedForceMagnitude: 0.5f,
                                            //        airborneForceDirection: new Vector3(0.0f, 1.0f, 1.0f),
                                            //        airborneForceMagnitude: 1.0f));

                                            //hb.Event("onEqualPriorityHitBox", (hitInfo) => {
                                            //    if (hitInfo.hitEntity != fgChar) {
                                            //        fgChar.PredeterminedActions("verticalClash");
                                            //    }
                                            //});
                                        });

                                        HitBox closeHitBox = hitBoxService.Create(hb => {
                                            hb.Relative(fgChar.transform);
                                            hb.Point0(new Vector3(0, 0, 0.6f));
                                            hb.Point1(new Vector3(0, 1, 0.6f));
                                            hb.Radius(0.5f);
                                            hb.InGameEntity(fgChar);
                                            hb.Validate((HitBox _, HitBox other) => {
                                                return true;
                                            });
                                            hb.Validate((HitBox _, HurtBox other) => {
                                                return true;
                                            });
                                            //hb.Event("onHitFGChar",
                                            //    CommonGroundedOnHitFGChar(
                                            //        hitSound: audioMap["mediumHit"],
                                            //        groundedForceMagnitude: 0.5f,
                                            //        airborneForceDirection: new Vector3(0.0f, 1.0f, 1.0f),
                                            //        airborneForceMagnitude: 1.0f));

                                            //hb.Event("onEqualPriorityHitBox", (hitInfo) => {
                                            //    if (hitInfo.hitEntity != fgChar) {
                                            //        fgChar.PredeterminedActions("verticalClash");
                                            //    }
                                            //});
                                        });

                                        hit.HitBox(farHitBox);
                                        hit.HitBox(closeHitBox);

                                        //hit.Event("onHitFGChar", (hitBoxes, hitInfo) => {
                                        //    if (hitBoxes.Count == 1 && hitBoxes.Contains(farHitBox)) {
                                        //        // do extra stuff.
                                        //    }

                                        //    hitBoxes[0].InvokeEvent("onHitFGChar", hitInfo);
                                        //});
                                    })
                                .To(16)
                                .From(22)
                                    .CounterHit(false)
                                .To(30);
                            }));
                        atkBuilder.CleanUp(ReturnToStand);
                    });

                    Attack atkOrt2A = new Attack(atkBuilder => {
                        atkBuilder.Name("orthodox_2A");
                        atkBuilder.Orientation(Orientation.ORTHODOX);
                        atkBuilder.GroundRelation(GroundRelation.GROUNDED);
                        atkBuilder.Input(InputNotation._1A, InputNotation._2A, InputNotation._3A);
                        atkBuilder.AnimationState("2A");
                        atkBuilder.InitCharState((CharacterStates.Attack)fgChar.State("attackGrounded"));
                        atkBuilder.Movement();
                        atkBuilder.Frames(
                            FrameUtil.CreateList(f => { f
                                .SpecialCancellable(true)
                                .ChainCancellable(false)
                                .CancellableOnWhiff(false)
                                .CounterHit(true)
                                .From(8)
                                    .Hit(hit => {
                                        hit.HitDamage(600)
                                            .BlockDamage(0)
                                            .HitStun(10.0f)
                                            .BlockStun(8.0f);

                                        hit.Block(Block.LOW);

                                        hit.HitBox(hitBoxService.Create(hb => {
                                            hb.Relative(fgChar.transform);
                                            hb.Point0(new Vector3(0, 0.2f, 0.5f));
                                            hb.Point1(new Vector3(0, 0.2f, 1.3f));
                                            hb.Radius(0.35f);
                                            hb.InGameEntity(fgChar);
                                            hb.Validate((HitBox _, HitBox other) => {
                                                return true;
                                            });
                                            hb.Validate((HitBox _, HurtBox other) => {
                                                return true;
                                            });
                                            //hb.Event("onHitFGChar", (hitInfo) => {
                                            //    FightingGameCharacter opponent = (FightingGameCharacter) hitInfo.hitEntity;
                                            //    if (opponent != fgChar) {
                                            //        audioService.PlayOneShot(hitInfo.position, audioMap["weakHit"]);
                                            //        fgService.Hit(hitInfo.hitEntity, fgChar, hitInfo.hitBox, (hitAtSameTimeByAttackPriority) => {
                                            //            opponent.ChangeHealth(hitInfo.damage); // hitInfo.damage will include combo scaling.
                                            //            opponent.KnockBack(
                                            //                hitInfo.hitBox.hit.priority,
                                            //                launch: false,
                                            //                knockbackDirection: fgChar.transform.rotation * new Vector3(0.0f, 1.0f, 1.0f),
                                            //                knockbackMagnitude: 1.0f);
                                            //        });
                                            //    }
                                            //});
                                        }));
                                    })
                                .To(9)
                                .From(14)
                                    .ChainCancellable(true)
                                    .CounterHit(false)
                                .To(22);
                            }));
                        atkBuilder.CleanUp(ReturnToCrouch);
                    });

                    Attack atkOrt2AA = new Attack(atkBuilder => {
                        atkBuilder.Name("orthodox_2AA");
                        atkBuilder.Orientation(Orientation.ORTHODOX);
                        atkBuilder.GroundRelation(GroundRelation.GROUNDED);
                        atkBuilder.Input(InputNotation._1A, InputNotation._2A, InputNotation._3A);
                        atkBuilder.AnimationState("2AA");
                        atkBuilder.InitCharState((CharacterStates.Attack)fgChar.State("attackGrounded"));
                        atkBuilder.Movement();
                        atkBuilder.Frames(
                            FrameUtil.CreateList(f => { f
                                .SpecialCancellable(true)
                                .ChainCancellable(false)
                                .CancellableOnWhiff(false)
                                .CounterHit(true)
                                .From(10)
                                    .Hit(hit => {
                                        hit.HitDamage(800)
                                            .BlockDamage(0)
                                            .HitStun(20.0f)
                                            .BlockStun(20.0f)
                                            .ComboScaling(0.5f);

                                        hit.Block(Block.LOW);

                                        hit.HitBox(hitBoxService.Create(hb => {
                                            hb.Relative(fgChar.transform);
                                            hb.Point0(new Vector3(0, 0.2f, 0.6f));
                                            hb.Point1(new Vector3(0, 0.2f, 1.4f));
                                            hb.Radius(0.25f);
                                            hb.InGameEntity(fgChar);
                                            hb.Validate((HitBox _, HitBox other) => {
                                                return true;
                                            });
                                            hb.Validate((HitBox _, HurtBox other) => {
                                                return true;
                                            });
                                            //hb.Event("onHitFGChar",
                                            //    CommonGroundedOnHitFGChar(
                                            //        hitSound: audioMap["weakHit"],
                                            //        groundedForceMagnitude: 0.5f,
                                            //        airborneForceDirection: new Vector3(0.0f, 1.0f, 1.0f),
                                            //        airborneForceMagnitude: 1.0f));
                                        }));
                                    })
                                .To(14)
                                .From(16)
                                    .ChainCancellable(true)
                                    .CounterHit(true)
                                .To(28);
                            }));
                        atkBuilder.CleanUp(ReturnToCrouch);
                    });

                    Attack atkOrt5B = new Attack(atkBuilder => {
                        atkBuilder.Name("orthodox_5B");
                        atkBuilder.Orientation(Orientation.ORTHODOX);
                        atkBuilder.GroundRelation(GroundRelation.GROUNDED);
                        atkBuilder.Input(InputNotation._4B, InputNotation._5B);
                        atkBuilder.AnimationState("5B");
                        atkBuilder.InitCharState((CharacterStates.Attack)fgChar.State("attackGrounded"));
                        atkBuilder.Movement(zMoveCb: animationCurveMap["orth_5B.z"].Evaluate);
                        atkBuilder.Frames(
                            FrameUtil.CreateList(fl => {
                                fl.SpecialCancellable(true);
                                fl.CancellableOnWhiff(false);
                                fl.CounterHit(true);
                                fl.From(1);
                                    fl.ChainCancellable(false);
                                fl.From(13);
                                    fl.Hit(hit => {
                                        hit.HitDamage(1000);
                                        hit.BlockDamage(0);
                                        hit.HitStun(30.0f);
                                        hit.BlockStun(12.0f);
                                        hit.ComboScaling(1.0f);
                                        hit.Priority(AttackPriority.LightAttack);

                                        hit.HitBox(hitBoxService.Create(hb => {
                                            hb.Relative(fgChar.transform);
                                            hb.Point0(new Vector3(0, 0, 0.8f));
                                            hb.Point1(new Vector3(0, 1, 0.8f));
                                            hb.Radius(0.25f);
                                            hb.InGameEntity(fgChar);
                                            hb.Validate((HitBox _, HitBox other) => {
                                                return true;
                                            });
                                            hb.Validate((HitBox _, HurtBox other) => {
                                                return true;
                                            });
                                            //hb.Event("onHitFGChar",
                                            //    CommonGroundedOnHitFGChar(
                                            //        hitSound: audioMap["mediumHit"],
                                            //        groundedForceMagnitude: 0.5f,
                                            //        airborneForceDirection: new Vector3(0.0f, 1.0f, 1.0f),
                                            //        airborneForceMagnitude: 1.0f));

                                            //hb.Event("onEqualPriorityHitBox", (hitInfo) => {
                                            //    if (hitInfo.hitEntity != fgChar) {
                                            //        fgChar.PredeterminedActions("horizontalClashSwingFromLeft");
                                            //    }
                                            //});
                                        }));
                                    });
                                fl.To(16);
                                fl.From(20);
                                    fl.CounterHit(false);
                                    fl.ChainCancellable(true);
                                fl.To(30);
                            }));
                        atkBuilder.CleanUp(ReturnToStand);
                    });

                    Attack atkOrt5BB = new Attack(atkBuilder => {
                        atkBuilder.Name("orthodox_5BB");
                        atkBuilder.Orientation(Orientation.ORTHODOX);
                        atkBuilder.GroundRelation(GroundRelation.GROUNDED);
                        atkBuilder.Input(InputNotation._4B, InputNotation._5B);
                        atkBuilder.AnimationState("5BB");
                        atkBuilder.InitCharState((CharacterStates.Attack)fgChar.State("attackGrounded"));
                        atkBuilder.Movement(zMoveCb: animationCurveMap["orth_5BB.z"].Evaluate);
                        atkBuilder.Frames(
                            FrameUtil.CreateList(fl => {
                                fl.SpecialCancellable(true);
                                fl.CancellableOnWhiff(false);
                                fl.CounterHit(true);
                                fl.From(4);
                                    fl.Track((targetFG) => {
                                        Vector3 newTargetPos = TargetUtil.MoveTargetLimited(fgChar.position, targetFG.targetPos, targetFG.ActualTargetPos(), 20.0f);
                                            // for now, just let the setting of this value be instantaneous once per frame.
                                        fgChar.SetTarget(newTargetPos);
                                    });
                                fl.To(8);
                                fl.From(13);
                                    fl.Hit(hit => {
                                        hit.HitDamage(1000);
                                        hit.BlockDamage(0);
                                        hit.HitStun(30.0f);
                                        hit.BlockStun(12.0f);
                                        hit.ComboScaling(1.0f);
                                        hit.Priority(AttackPriority.LightAttack);

                                        hit.HitBox(hitBoxService.Create(hb => {
                                            hb.Relative(fgChar.transform);
                                            hb.Point0(new Vector3(0, 0, 0.8f));
                                            hb.Point1(new Vector3(0, 1, 0.8f));
                                            hb.Radius(0.25f);
                                            hb.InGameEntity(fgChar);
                                            hb.Validate((HitBox _, HitBox other) => {
                                                return true;
                                            });
                                            hb.Validate((HitBox _, HurtBox other) => {
                                                return true;
                                            });
                                            //hb.Event("onHitFGChar",
                                            //    CommonGroundedOnHitFGChar(
                                            //        hitSound: audioMap["mediumHit"],
                                            //        groundedForceMagnitude: 0.5f,
                                            //        airborneForceDirection: new Vector3(0.0f, 1.0f, 1.0f),
                                            //        airborneForceMagnitude: 1.0f));

                                            //hb.Event("onEqualPriorityHitBox", (hitInfo) => {
                                            //    if (hitInfo.hitEntity != fgChar) {
                                            //        fgChar.PredeterminedActions("horizontalClashSwingFromLeft");
                                            //    }
                                            //});
                                        }));
                                    });
                                fl.To(16);
                                fl.From(20);
                                    fl.CounterHit(false);
                                    fl.ChainCancellable(true);
                                fl.To(30);
                            }));
                        atkBuilder.CleanUp(ReturnToStand);
                    });

                    Attack atkOrt5BBB = new Attack(atkBuilder => {
                        atkBuilder.Name("orthodox_5BBB");
                        atkBuilder.Orientation(Orientation.ORTHODOX);
                        atkBuilder.GroundRelation(GroundRelation.GROUNDED);
                        atkBuilder.Input(InputNotation._4B, InputNotation._5B);
                        atkBuilder.AnimationState("5BBB");
                        atkBuilder.InitCharState((CharacterStates.Attack)fgChar.State("attackGrounded"));
                        atkBuilder.Movement(zMoveCb: animationCurveMap["orth_5BBB.z"].Evaluate);
                        atkBuilder.Frames(
                            FrameUtil.CreateList(fl => {
                                fl.SpecialCancellable(true);
                                fl.CancellableOnWhiff(false);
                                fl.CounterHit(true);
                                fl.From(15);
                                    fl.Hit(hit => {
                                        hit.HitDamage(1000);
                                        hit.BlockDamage(0);
                                        hit.HitStun(30.0f);
                                        hit.BlockStun(12.0f);
                                        hit.ComboScaling(1.0f);
                                        hit.Priority(AttackPriority.LightAttack);

                                        hit.HitBox(hitBoxService.Create(hb => {
                                            hb.Relative(fgChar.transform);
                                            hb.Point0(new Vector3(0, 0, 0.8f));
                                            hb.Point1(new Vector3(0, 1, 0.8f));
                                            hb.Radius(0.25f);
                                            hb.InGameEntity(fgChar);
                                            hb.Validate((HitBox _, HitBox other) => {
                                                return true;
                                            });
                                            hb.Validate((HitBox _, HurtBox other) => {
                                                return true;
                                            });
                                            //hb.Event("onHitFGChar",
                                            //    CommonGroundedOnHitFGChar(
                                            //        hitSound: audioMap["mediumHit"],
                                            //        groundedForceMagnitude: 0.5f,
                                            //        airborneForceDirection: new Vector3(0.0f, 1.0f, 1.0f),
                                            //        airborneForceMagnitude: 1.0f));

                                            //hb.Event("onEqualPriorityHitBox", (hitInfo) => {
                                            //    if (hitInfo.hitEntity != fgChar) {
                                            //        fgChar.PredeterminedActions("horizontalClashSwingFromLeft");
                                            //    }
                                            //});
                                        }));
                                    });
                                fl.To(19);
                                fl.From(35);
                                    fl.CounterHit(false);
                                    fl.ChainCancellable(true);
                                fl.To(47);
                            }));
                        atkBuilder.CleanUp(ReturnToStand);
                    });

                    Attack atkOrt5BBBB = new Attack(atkBuilder => {
                        atkBuilder.Name("orthodox_5BBBB");
                        atkBuilder.Orientation(Orientation.ORTHODOX);
                        atkBuilder.GroundRelation(GroundRelation.GROUNDED);
                        atkBuilder.Input(InputNotation._4B, InputNotation._5B);
                        atkBuilder.AnimationState("5BBBB");
                        atkBuilder.InitCharState((CharacterStates.Attack)fgChar.State("attackGrounded"));
                        atkBuilder.Movement(zMoveCb: animationCurveMap["orth_5BBBB.z"].Evaluate);
                        atkBuilder.Frames(
                            FrameUtil.CreateList(fl => {
                                fl.SpecialCancellable(true);
                                fl.CancellableOnWhiff(false);
                                fl.CounterHit(true);
                                fl.From(9);
                                    fl.Hit(hit => {
                                        hit.HitDamage(1000);
                                        hit.BlockDamage(0);
                                        hit.HitStun(30.0f);
                                        hit.BlockStun(12.0f);
                                        hit.ComboScaling(1.0f);
                                        hit.Priority(AttackPriority.LightAttack);

                                        hit.HitBox(hitBoxService.Create(hb => {
                                            hb.Relative(fgChar.transform);
                                            hb.Point0(new Vector3(0, 0, 0.8f));
                                            hb.Point1(new Vector3(0, 1, 0.8f));
                                            hb.Radius(0.25f);
                                            hb.InGameEntity(fgChar);
                                            hb.Validate((HitBox _, HitBox other) => {
                                                return true;
                                            });
                                            hb.Validate((HitBox _, HurtBox other) => {
                                                return true;
                                            });
                                            //hb.Event("onHitFGChar",
                                            //    CommonGroundedOnHitFGChar(
                                            //        hitSound: audioMap["mediumHit"],
                                            //        groundedForceMagnitude: 0.5f,
                                            //        airborneForceDirection: new Vector3(0.0f, 1.0f, 1.0f),
                                            //        airborneForceMagnitude: 1.0f));

                                            //hb.Event("onEqualPriorityHitBox", (hitInfo) => {
                                            //    if (hitInfo.hitEntity != fgChar) {
                                            //        fgChar.PredeterminedActions("horizontalClashSwingFromLeft");
                                            //    }
                                            //});
                                        }));
                                    });
                                fl.To(10);
                                fl.From(20);
                                    fl.CounterHit(false);
                                    fl.ChainCancellable(true);
                                fl.To(35);
                            }));
                        atkBuilder.CleanUp(ReturnToStand);
                    });

                    Attack atkJump5A = new Attack(atkBuilder => {
                        atkBuilder.Name("jump_5A");
                        atkBuilder.Orientation(Orientation.ORTHODOX);
                        atkBuilder.GroundRelation(GroundRelation.AIRBORNE);
                        atkBuilder.Input(InputNotation._4A, InputNotation._5A, InputNotation._6A, InputNotation._1A, InputNotation._2A, InputNotation._3A);
                        atkBuilder.AnimationState("j_A");
                        atkBuilder.InitCharState((CharacterStates.Attack)fgChar.State("attackAirborne"));
                        atkBuilder.Movement();
                        atkBuilder.FramesContinuous((localFrame, target) => {
                            if (localFrame >= 8.0f && localFrame <= 11.0f) {
                                float p = Mathf.Lerp(8.0f, 11.0f, localFrame);
                                fgChar.SetStandCollider(p * 0.5f * Vector3.up);
                            }
                        });
                        atkBuilder.Frames(
                            FrameUtil.CreateList(f => { f
                                .SpecialCancellable(true)
                                .ChainCancellable(true)
                                .CancellableOnWhiff(true)
                                .CounterHit(true)
                                .To(8)
                                .From(6)
                                    .CancellableOnWhiff(false)
                                .From(8)
                                    .Hit(hit => {
                                        hit.HitDamage(700)
                                            .BlockDamage(0)
                                            .HitStun(12.0f)
                                            .BlockStun(30.0f);

                                        hit.HitBox(hitBoxService.Create(hb => {
                                            hb.Relative(fgChar.transform);
                                            hb.Point0(new Vector3(0, 0, 0.8f));
                                            hb.Point1(new Vector3(0, 1, 0.8f));
                                            hb.Radius(0.25f);
                                            hb.InGameEntity(fgChar);
                                            hb.Validate((HitBox _, HitBox other) => {
                                                return true;
                                            });
                                            hb.Validate((HitBox _, HurtBox other) => {
                                                return true;
                                            });
                                            //hb.Event("onHitFGChar",
                                            //    CommonGroundedOnHitFGChar(
                                            //        hitSound: audioMap["mediumHit"],
                                            //        groundedForceMagnitude: 0.5f,
                                            //        airborneForceDirection: new Vector3(0.0f, 1.0f, 1.0f),
                                            //        airborneForceMagnitude: 1.0f));
                                        }));
                                    })
                                .To(11)
                                .From(13)
                                    .CounterHit(false)
                                    .ChainCancellable(true)
                                .To(23);
                            }));
                        atkBuilder.CleanUp(ReturnToAirborne);
                    });

                    Attack atkJump5AA = new Attack(atkBuilder => {
                        atkBuilder.Name("jump_5AA");
                        atkBuilder.Orientation(Orientation.ORTHODOX);
                        atkBuilder.GroundRelation(GroundRelation.AIRBORNE);
                        atkBuilder.Input(InputNotation._4A, InputNotation._5A, InputNotation._6A, InputNotation._1A, InputNotation._2A, InputNotation._3A);
                        atkBuilder.AnimationState("j_AA");
                        atkBuilder.InitCharState((CharacterStates.Attack)fgChar.State("attackAirborne"));
                        atkBuilder.Movement();
                        atkBuilder.FramesContinuous((localFrame, target) => {
                            if (localFrame >= 8.0f && localFrame <= 11.0f) {
                                float p = Mathf.Lerp(8.0f, 11.0f, localFrame);
                                fgChar.SetStandCollider(p * 0.5f * Vector3.up);
                            }
                        });
                        atkBuilder.Frames(
                            FrameUtil.CreateList(f => { f
                                .SpecialCancellable(true)
                                .ChainCancellable(false)
                                .CancellableOnWhiff(false)
                                .CounterHit(true)
                                .From(4)
                                    .Track((targetFG) => {
                                        Vector3 newTargetPos = TargetUtil.MoveTargetLimited(fgChar.position, targetFG.targetPos, targetFG.ActualTargetPos(), 20.0f);
                                            // for now, just let the setting of this value be instantaneous once per frame.
                                        fgChar.SetTarget(newTargetPos);
                                    })
                                .To(12)
                                .From(12)
                                    .Hit(hit => {
                                        hit.HitDamage(700)
                                            .BlockDamage(0)
                                            .HitStun(12.0f)
                                            .BlockStun(12.0f);

                                        hit.HitBox(hitBoxService.Create(hb => {
                                            hb.Relative(fgChar.transform);
                                            hb.Point0(new Vector3(0, 0, 0.8f));
                                            hb.Point1(new Vector3(0, 1, 0.8f));
                                            hb.Radius(0.25f);
                                            hb.InGameEntity(fgChar);
                                            hb.Validate((HitBox _, HitBox other) => {
                                                return true;
                                            });
                                            hb.Validate((HitBox _, HurtBox other) => {
                                                return true;
                                            });
                                            //hb.Event("onHitFGChar",
                                            //    CommonGroundedOnHitFGChar(
                                            //        hitSound: audioMap["weakHit"],
                                            //        groundedForceMagnitude: 0.5f,
                                            //        airborneForceDirection: new Vector3(0.0f, 1.0f, 1.0f),
                                            //        airborneForceMagnitude: 1.0f));
                                        }));
                                    })
                                .To(13)
                                .From(16)
                                    .CounterHit(false)
                                    .ChainCancellable(true)
                                .To(30);
                            }));
                        atkBuilder.CleanUp(ReturnToAirborne);
                    });

                    Attack atkJump5AAA = new Attack(atkBuilder => {
                        atkBuilder.Name("jump_5AAA");
                        atkBuilder.Orientation(Orientation.ORTHODOX);
                        atkBuilder.GroundRelation(GroundRelation.AIRBORNE);
                        atkBuilder.Input(InputNotation._4A, InputNotation._5A, InputNotation._6A, InputNotation._1A, InputNotation._2A, InputNotation._3A);
                        atkBuilder.AnimationState("j_AAA");
                        atkBuilder.InitCharState((CharacterStates.Attack)fgChar.State("attackAirborne"));
                        atkBuilder.Movement(yMoveCb: animationCurveMap["jump_5AAA.y"].Evaluate, zMoveCb: animationCurveMap["jump_5AAA.z"].Evaluate);
                        atkBuilder.FramesContinuous((localFrame, target) => {
                            if (localFrame >= 8.0f && localFrame <= 11.0f) {
                                float p = Mathf.Lerp(8.0f, 11.0f, localFrame);
                                fgChar.SetStandCollider(p * 0.5f * Vector3.up);
                            }
                        });
                        atkBuilder.Frames(
                            FrameUtil.CreateList(f => { f
                                .SpecialCancellable(true)
                                .ChainCancellable(false)
                                .CancellableOnWhiff(false)
                                .CounterHit(true)
                                .From(4)
                                    .Track((targetFG) => {
                                        Vector3 newTargetPos = TargetUtil.MoveTargetLimited(fgChar.position, targetFG.targetPos, targetFG.ActualTargetPos(), 20.0f);
                                            // for now, just let the setting of this value be instantaneous once per frame.
                                        fgChar.SetTarget(newTargetPos);
                                    })
                                .To(12)
                                .From(20)
                                    .Hit(hit => {
                                        hit.HitDamage(2000)
                                            .BlockDamage(0)
                                            .HitStun(40.0f)
                                            .BlockStun(30.0f)
                                            .Priority(AttackPriority.HeavyAttack)
                                            .ComboScaling(1.0f);

                                        hit.HitBox(hitBoxService.Create(hb => {
                                            hb.Relative(fgChar.transform);
                                            hb.Point0(new Vector3(0, 0, 0.8f));
                                            hb.Point1(new Vector3(0, 1, 0.8f));
                                            hb.Radius(0.25f);
                                            hb.InGameEntity(fgChar);
                                            hb.Validate((HitBox _, HitBox other) => {
                                                return true;
                                            });
                                            hb.Validate((HitBox _, HurtBox other) => {
                                                return true;
                                            });
                                            //hb.Event("onHitFGChar", (hitInfo) => {
                                            //    FightingGameCharacter opponent = (FightingGameCharacter) hitInfo.hitEntity;
                                            //    if (opponent != fgChar) {
                                            //        audioService.PlayOneShot(hitInfo.position, audioMap["strongHit"]);
                                            //            // This exists to make characters hitting each other async
                                            //        fgService.Hit(hitInfo.hitEntity, fgChar, hitInfo.hitBox, (hitAtSameTimeByAttackPriority) => {
                                            //            opponent.ChangeHealth(hitInfo.damage); // hitInfo.damage will include combo scaling.

                                            //            if (hitAtSameTimeByAttackPriority == AttackPriority.HeavyAttack) { // colloquially known as trading
                                            //                cameraService.PredeterminedActions<float>("zoomIn", 0.5f);
                                            //                timeService.PredeterminedActions<float>("timeSlow", 0.5f);

                                            //                opponent.KnockBack(
                                            //                    hitInfo.hitBox.hit.priority,
                                            //                    launch: false,
                                            //                    knockbackDirection: fgChar.transform.rotation * Vector3.forward,
                                            //                    knockbackMagnitude: 0.5f);
                                            //            }
                                            //            else {
                                            //                opponent.KnockBack(
                                            //                    hitInfo.hitBox.hit.priority,
                                            //                    launch: true,
                                            //                    knockbackDirection: fgChar.transform.rotation * new Vector3(0.0f, 1.0f, 1.0f),
                                            //                    knockbackMagnitude: 2.0f);
                                            //            }
                                            //        });
                                            //    }
                                            //});
                                            //hb.Event("onHitProjectile", (hitInfo) => {
                                            //    Projectile projectile = (Projectile) hitInfo.hitEntity;
                                            //    if (projectile.health <= 2) {
                                            //        fgService.Hit(projectile, fgChar, hitInfo.hitBox, (hitAtSameTimeByAttackPriority) => {
                                            //            if (hitAtSameTimeByAttackPriority != AttackPriority.None) {
                                            //                particleService.PlayOneShot(hitInfo.position, particleMap["destroyParticle"]);
                                            //                    // these lines might be redundant
                                            //                particleService.PlayOneShot(hitInfo.position, projectile.DestroyedParticle());
                                            //                projectile.RemoveSelf();
                                            //            }
                                            //        });
                                            //    }
                                            //});
                                        }));
                                    })
                                .To(21)
                                    .CounterHit(false)
                                .To(40);
                            }));
                        atkBuilder.CleanUp(ReturnToAirborne);
                    });

                    Attack atkOrt1C = new Attack(atkBuilder => {
                        atkBuilder.Name("orthodox_1C");
                        atkBuilder.Orientation(Orientation.ORTHODOX);
                        atkBuilder.GroundRelation(GroundRelation.GROUNDED);
                        atkBuilder.Input(InputNotation._1C);
                        atkBuilder.AnimationState("1C");
                        atkBuilder.InitCharState((CharacterStates.Attack)fgChar.State("attackGrounded"));
                        atkBuilder.Movement(zMoveCb: animationCurveMap["orth_1C.z"].Evaluate);
                        atkBuilder.Frames(
                            FrameUtil.CreateList(fl => {
                                fl.SpecialCancellable(true);
                                fl.CancellableOnWhiff(false);
                                fl.CounterHit(true);
                                fl.From(9);
                                    fl.Hit(hit => {
                                        hit.HitDamage(1000);
                                        hit.BlockDamage(0);
                                        hit.HitStun(30.0f);
                                        hit.BlockStun(12.0f);
                                        hit.ComboScaling(1.0f);
                                        hit.Priority(AttackPriority.LightAttack);

                                        hit.HitBox(hitBoxService.Create(hb => {
                                            hb.Relative(fgChar.transform);
                                            hb.Point0(new Vector3(0, 0, 0.8f));
                                            hb.Point1(new Vector3(0, 1, 0.8f));
                                            hb.Radius(0.25f);
                                            hb.InGameEntity(fgChar);
                                            hb.Validate((HitBox _, HitBox other) => {
                                                return true;
                                            });
                                            hb.Validate((HitBox _, HurtBox other) => {
                                                return true;
                                            });
                                            //hb.Event("onHitFGChar",
                                            //    CommonGroundedOnHitFGChar(
                                            //        hitSound: audioMap["mediumHit"],
                                            //        groundedForceMagnitude: 0.5f,
                                            //        airborneForceDirection: new Vector3(0.0f, 1.0f, 1.0f),
                                            //        airborneForceMagnitude: 1.0f));

                                            //hb.Event("onEqualPriorityHitBox", (hitInfo) => {
                                            //    if (hitInfo.hitEntity != fgChar) {
                                            //        fgChar.PredeterminedActions("horizontalClashSwingFromLeft");
                                            //    }
                                            //});
                                        }));
                                    });
                                fl.To(10);
                                fl.From(20);
                                    fl.CounterHit(false);
                                    fl.ChainCancellable(true);
                                fl.To(35);
                            }));
                        atkBuilder.CleanUp(ReturnToStand);
                    });

                    Attack atkOrt2C = new Attack(atkBuilder => {
                        atkBuilder.Name("orthodox_2C");
                        atkBuilder.Orientation(Orientation.ORTHODOX);
                        atkBuilder.GroundRelation(GroundRelation.GROUNDED);
                        atkBuilder.Input(InputNotation._2C);
                        atkBuilder.AnimationState("2C");
                        atkBuilder.InitCharState((CharacterStates.Attack)fgChar.State("attackGrounded"));
                        atkBuilder.Movement();
                        atkBuilder.Frames(
                            FrameUtil.CreateList(fl => {
                                fl.SpecialCancellable(true);
                                fl.CancellableOnWhiff(false);
                                fl.CounterHit(true);
                                fl.From(9);
                                    fl.Hit(hit => {
                                        hit.HitDamage(1000);
                                        hit.BlockDamage(0);
                                        hit.HitStun(30.0f);
                                        hit.BlockStun(12.0f);
                                        hit.ComboScaling(1.0f);
                                        hit.Priority(AttackPriority.LightAttack);

                                        hit.HitBox(hitBoxService.Create(hb => {
                                            hb.Relative(fgChar.transform);
                                            hb.Point0(new Vector3(0, 0, 0.8f));
                                            hb.Point1(new Vector3(0, 1, 0.8f));
                                            hb.Radius(0.25f);
                                            hb.InGameEntity(fgChar);
                                            hb.Validate((HitBox _, HitBox other) => {
                                                return true;
                                            });
                                            hb.Validate((HitBox _, HurtBox other) => {
                                                return true;
                                            });
                                            //hb.Event("onHitFGChar",
                                            //    CommonGroundedOnHitFGChar(
                                            //        hitSound: audioMap["mediumHit"],
                                            //        groundedForceMagnitude: 0.5f,
                                            //        airborneForceDirection: new Vector3(0.0f, 1.0f, 1.0f),
                                            //        airborneForceMagnitude: 1.0f));

                                            //hb.Event("onEqualPriorityHitBox", (hitInfo) => {
                                            //    if (hitInfo.hitEntity != fgChar) {
                                            //        fgChar.PredeterminedActions("horizontalClashSwingFromLeft");
                                            //    }
                                            //});
                                        }));
                                    });
                                fl.To(10);
                                fl.From(20);
                                    fl.CounterHit(false);
                                    fl.ChainCancellable(true);
                                fl.To(35);
                            }));
                        atkBuilder.CleanUp(ReturnToStand);
                    });

                    Attack hadouken = new Attack(atkBuilder => {
                        atkBuilder.Name("hadouken");
                        atkBuilder.Orientation(Orientation.ORTHODOX);
                        atkBuilder.GroundRelation(GroundRelation.GROUNDED);
                        atkBuilder.Input(InputNotation._236C);
                        atkBuilder.AnimationState("hadouken");
                        atkBuilder.InitCharState((CharacterStates.Attack)fgChar.State("attackGrounded"));
                        atkBuilder.Frames(
                            FrameUtil.CreateList(f => { f
                                .SpecialCancellable(false)
                                .ChainCancellable(false)
                                .CounterHit(true)
                                .From(3)
                                    .Sound(audioMap["hadouken"], soundResource => {
                                        soundResource.transform.position = fgChar.GetSpeakPosition();
                                        audioService.Play(soundResource);
                                    })
                                    .Track((targetFG) => {
                                        Vector3 newTargetPos = TargetUtil.MoveTargetLimited(fgChar.position, targetFG.targetPos, targetFG.ActualTargetPos(), 5.0f);
                                            // for now, just let the setting of this value be instantaneous once per frame.
                                        fgChar.SetTarget(newTargetPos);
                                    })
                                .To(46)
                                .From(12)
                                    .Projectile(projectileMap["hadouken"], proj => {
                                        projectileService.FireProjectile(proj.id);
                                        hitBoxService.Create(hb => {
                                            hb.InGameEntity(proj);
                                        });
                                    })
                                .To(13)
                                .From(16)
                                    .CounterHit(false)
                                .To(46);
                            }));
                        atkBuilder.CleanUp(ReturnToStand);
                    });

                    List<CharacterStates.CharacterBaseState> validGroundedAttackStates = new List<CharacterStates.CharacterBaseState> {
                        fgChar.State("stand"),
                        fgChar.State("crouch"),
                        fgChar.State("attackGrounded"),
                        fgChar.State("backDash"),
                        fgChar.State("forwardDash"),
                        fgChar.State("forwardDashLong"),
                        fgChar.State("dodge"),
                    };

                    List<CharacterStates.CharacterBaseState> validAirborneAttackStates = new List<CharacterStates.CharacterBaseState> {
                        fgChar.State("airborne"),
                        fgChar.State("airDash"),
                        fgChar.State("attackAirborne"),
                    };

                    charBuilder.Attack(atkOrt5A, (charState, currAttack, prevAttacks) => { // prevAttacks is a List<Attack>
                        if (!validGroundedAttackStates.Contains(charState)) {
                            return false;
                        }
                        if (currAttack == null ||
                            currAttack == atkOrt2A ||
                            currAttack == atkOrt2AA) {
                            return true;
                        }
                        return false;
                    });
                    charBuilder.Attack(atkOrt5AA, (charState, currAttack, prevAttacks) => {
                        if (!validGroundedAttackStates.Contains(charState)) {
                            return false;
                        }
                        if (currAttack == atkOrt5A) {
                            return true;
                        }
                        return false;
                    });
                    charBuilder.Attack(atkOrt5AAA, (charState, currAttack, prevAttacks) => {
                        if (!validGroundedAttackStates.Contains(charState)) {
                            return false;
                        }
                        if (currAttack == atkOrt5AA) {
                            return true;
                        }
                        return false;
                    });
                    charBuilder.Attack(atkOrt2A, (charState, currAttack, prevAttacks) => {
                        if (!validGroundedAttackStates.Contains(charState)) {
                            return false;
                        }
                        if (prevAttacks.Where(prev => prev == atkOrt5A).Count() > 1 ||
                            prevAttacks.Contains(atkOrt2A)) {
                            return false;
                        }
                        if (currAttack == null ||
                            currAttack == atkOrt5A) {
                            return true;
                        }
                        return false;
                    });
                    charBuilder.Attack(atkOrt2AA, (charState, currAttack, prevAttacks) => {
                        if (!validGroundedAttackStates.Contains(charState)) {
                            return false;
                        }
                        if (currAttack == atkOrt2A) {
                            return true;
                        }
                        return false;
                    });
                    charBuilder.Attack(atkOrt5B, (charState, currAttack, prevAttacks) => {
                        if (!validGroundedAttackStates.Contains(charState)) {
                            return false;
                        }
                        if (currAttack == null ||
                            currAttack == atkOrt2A ||
                            currAttack == atkOrt2AA ||
                            currAttack == atkOrt5A ||
                            currAttack == atkOrt5AA ||
                            currAttack == atkOrt5AAA) {
                            return true;
                        }
                        return false;
                    });
                    charBuilder.Attack(atkOrt5BB, (charState, currAttack, prevAttacks) => {
                        if (!validGroundedAttackStates.Contains(charState)) {
                            return false;
                        }
                        if (currAttack == atkOrt5B) {
                            return true;
                        }
                        return false;
                    });
                    charBuilder.Attack(atkOrt5BBB, (charState, currAttack, prevAttacks) => {
                        if (!validGroundedAttackStates.Contains(charState)) {
                            return false;
                        }
                        if (currAttack == atkOrt5BB) {
                            return true;
                        }
                        return false;
                    });
                    charBuilder.Attack(atkOrt5BBBB, (charState, currAttack, prevAttacks) => {
                        if (!validGroundedAttackStates.Contains(charState)) {
                            return false;
                        }
                        if (currAttack == atkOrt5BBB) {
                            return true;
                        }
                        return false;
                    });
                    charBuilder.Attack(atkOrt1C, (charState, currAttack, prevAttacks) => {
                        if (!validGroundedAttackStates.Contains(charState)) {
                            return false;
                        }
                        if (currAttack == null ||
                            currAttack == atkOrt2A ||
                            currAttack == atkOrt2AA ||
                            currAttack == atkOrt5A ||
                            currAttack == atkOrt5AA ||
                            currAttack == atkOrt5AAA ||
                            currAttack == atkOrt5B ||
                            currAttack == atkOrt5BB ||
                            currAttack == atkOrt5BBB ||
                            currAttack == atkOrt5BBBB ||
                            currAttack == atkOrt2C) {
                            return true;
                        }
                        return false;
                    });
                    charBuilder.Attack(atkOrt2C, (charState, currAttack, prevAttacks) => {
                        if (!validGroundedAttackStates.Contains(charState)) {
                            return false;
                        }
                        if (currAttack == null ||
                            currAttack == atkOrt2A ||
                            currAttack == atkOrt2AA ||
                            currAttack == atkOrt5A ||
                            currAttack == atkOrt5AA ||
                            currAttack == atkOrt5AAA ||
                            currAttack == atkOrt5B ||
                            currAttack == atkOrt5BB ||
                            currAttack == atkOrt5BBB ||
                            currAttack == atkOrt5BBBB ||
                            currAttack == atkOrt1C) {
                            return true;
                        }
                        return false;
                    });
                    charBuilder.Attack(atkJump5A, (charState, currAttack, prevAttacks) => {
                        if (!validAirborneAttackStates.Contains(charState)) {
                            return false;
                        }
                        if (currAttack == null) {
                            return true;
                        }
                        return false;
                    });
                    charBuilder.Attack(atkJump5AA, (charState, currAttack, prevAttacks) => {
                        if (!validAirborneAttackStates.Contains(charState)) {
                            return false;
                        }
                        if (currAttack == atkJump5A) {
                            return true;
                        }
                        return false;
                    });
                    charBuilder.Attack(atkJump5AAA, (charState, currAttack, prevAttacks) => {
                        if (!validAirborneAttackStates.Contains(charState)) {
                            return false;
                        }
                        if (currAttack == atkJump5AA) {
                            return true;
                        }
                        return false;
                    });
                }

                private void ReturnToStand() {
                    fgChar.SetState(fgChar.State("stand"));
                }

                private void ReturnToCrouch() {
                    fgChar.SetState(fgChar.State("crouch"));
                }

                private void ReturnToAirborne() {
                    fgChar.SetStandCollider(Vector3.zero);
                    fgChar.SetState(fgChar.State("airborne"));
                }

                //private Action<HitInfo> CommonGroundedOnHitFGChar(AudioClip hitSound, float groundedForceMagnitude, Vector3 airborneForceDirection, float airborneForceMagnitude) {
                //    return hitInfo => {
                //        FightingGameCharacter opponent = (FightingGameCharacter) hitInfo.hitEntity;
                //        if (opponent != fgChar) {
                //            audioService.PlayOneShot(hitInfo.position, hitSound);
                //                // This exists to make characters hitting each other async
                //            fgService.Hit(hitInfo.hitEntity, fgChar, hitInfo.hitBox, (hitAtSameTimeByAttackPriority) => {
                //                opponent.ChangeHealth(hitInfo.damage); // hitInfo.damage will include combo scaling.

                //                switch (opponent.GetGroundRelation()) {
                //                    case GroundRelation.GROUNDED:
                //                        opponent.KnockBack(
                //                            hitInfo.hitBox.hit.priority,
                //                            launch: false,
                //                            knockbackDirection: fgChar.transform.rotation * Vector3.forward,
                //                            knockbackMagnitude: groundedForceMagnitude);
                //                        break;
                //                    case GroundRelation.AIRBORNE:
                //                        opponent.KnockBack(
                //                            hitInfo.hitBox.hit.priority,
                //                            launch: true,
                //                            knockbackDirection: fgChar.transform.rotation * airborneForceDirection,
                //                            knockbackMagnitude: airborneForceMagnitude);
                //                        break;
                //                }
                //            });
                //        }
                //    };
                //}
            }
        }
    }
}