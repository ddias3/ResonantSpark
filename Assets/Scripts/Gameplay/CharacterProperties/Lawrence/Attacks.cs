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

                private Dictionary<string, AudioClip> audioMap;
                private Dictionary<string, ParticleEffect> particleMap;
                private Dictionary<string, AnimationCurve> attackMovementMap;
                private Dictionary<string, Projectile> projectileMap;

                private FightingGameCharacter fgChar;

                public Attacks(
                        AllServices services,
                        Dictionary<string, AudioClip> audioMap,
                        Dictionary<string, ParticleEffect> particleMap,
                        Dictionary<string, AnimationCurve> attackMovementMap,
                        Dictionary<string, Projectile> projectileMap) {
                    fgService = services.GetService<IFightingGameService>();
                    audioService = services.GetService<IAudioService>();
                    projectileService = services.GetService<IProjectileService>();
                    particleService = services.GetService<IParticleService>();
                    cameraService = services.GetService<ICameraService>();
                    timeService = services.GetService<ITimeService>();

                    this.audioMap = audioMap;
                    this.particleMap = particleMap;
                    this.attackMovementMap = attackMovementMap;
                    this.projectileMap = projectileMap;
                }

                public void Init(ICharacterPropertiesCallbackObj charBuilder, FightingGameCharacter newFightingGameChar) {
                    fgChar = newFightingGameChar;

                    Attack atkSpw5A = new Attack(atkBuilder => {
                        atkBuilder.Name("southpaw_5A");
                        atkBuilder.Orientation(Orientation.SOUTHPAW);
                        atkBuilder.GroundRelation(GroundRelation.GROUNDED);
                        atkBuilder.Input(InputNotation._4A, InputNotation._5A);
                        atkBuilder.AnimationState("5A");
                        atkBuilder.Movement(xMoveCb: attackMovementMap["southpaw_5A.x"].Evaluate, zMoveCb: attackMovementMap["southpaw_5A.z"].Evaluate);
                        atkBuilder.Frames(
                            FrameUtil.CreateList(fl => {
                                fl.SpecialCancellable(true);
                                fl.CancellableOnWhiff(true);
                                fl.ChainCancellable(true);
                                fl.From(4);
                                    fl.Track((currTarget, actualTarget) => {
                                        Vector3 newTargetPos = TargetUtil.MoveTargetLimited(fgChar.position, currTarget, actualTarget.position, 20.0f);
                                            // for now, just let the setting of this value be instantaneous once per frame.
                                        fgChar.SetTarget(newTargetPos);
                                    });
                                fl.To(8);
                                fl.From(7);
                                    fl.CancellableOnWhiff(false);
                                fl.From(8);
                                    fl.Hit(hit => {
                                        hit.HitDamage(800);
                                        hit.BlockDamage(0);
                                        hit.HitStun(20.0f);
                                        hit.BlockStun(8.0f);
                                        hit.ComboScaling(0.65f);
                                        hit.Priority(AttackPriority.LightAttack);

                                        // hit.Block(); default is to allow any kind of blocking

                                        hit.HitBox(new HitBox(hb => {
                                            hb.Relative(fgChar.transform);
                                            hb.Point0(new Vector3(0, 0, 0));
                                            hb.Point1(new Vector3(0, 1, 0));
                                            hb.Radius(0.25f);
                                            hb.Event("onHitFGChar", 
                                                CommonGroundedOnHitFGChar(
                                                    hitSound: audioMap["weakHit"],
                                                    groundedForceMagnitude: 0.5f,
                                                    airborneForceDirection: new Vector3(0.0f, 1.0f, 1.0f),
                                                    airborneForceMagnitude: 1.0f));
                                        }));
                                    });
                                fl.To(10);
                                fl.From(10);
                                    fl.ChainCancellable(true);
                                fl.To(22);
                            }));
                        atkBuilder.CleanUp(ReturnToStand);
                    });

                    Attack atkSpw5AA = new Attack(atkBuilder => {
                        atkBuilder.Name("southpaw_5AA");
                        atkBuilder.Orientation(Orientation.SOUTHPAW);
                        atkBuilder.GroundRelation(GroundRelation.GROUNDED);
                        atkBuilder.Input(InputNotation._5A);
                        atkBuilder.AnimationState("5AA");
                        atkBuilder.Movement(xMoveCb: attackMovementMap["southpaw_5AA.x"].Evaluate, zMoveCb: attackMovementMap["southpaw_5AA.z"].Evaluate);
                        atkBuilder.Frames(
                            FrameUtil.CreateList(fl => {
                                fl.SpecialCancellable(true);
                                fl.CancellableOnWhiff(false);
                                fl.From(1);
                                    fl.ChainCancellable(false);
                                fl.From(4);
                                    fl.Track((currTarget, actualTarget) => {
                                        Vector3 newTargetPos = TargetUtil.MoveTargetLimited(fgChar.position, currTarget, actualTarget.position, 20.0f);
                                            // for now, just let the setting of this value be instantaneous once per frame.
                                        fgChar.SetTarget(newTargetPos);
                                    });
                                fl.To(12);
                                fl.From(10);
                                    fl.ChainCancellable(true);
                                fl.From(12);
                                    fl.Hit(hit => {
                                        hit.HitDamage(1000);
                                        hit.BlockDamage(0);
                                        hit.HitStun(30.0f);
                                        hit.BlockStun(12.0f);
                                        hit.ComboScaling(1.0f);
                                        hit.Priority(AttackPriority.LightAttack);

                                        HitBox tipperHitBox = new HitBox(hb => {
                                            hb.Relative(fgChar.transform);
                                            hb.Point0(new Vector3(0, 0, 0));
                                            hb.Point1(new Vector3(0, 1, 0));
                                            hb.Radius(0.25f);
                                            hb.Event("onHitFGChar",
                                                CommonGroundedOnHitFGChar(
                                                    hitSound: audioMap["mediumHit"],
                                                    groundedForceMagnitude: 0.5f,
                                                    airborneForceDirection: new Vector3(0.0f, 1.0f, 1.0f),
                                                    airborneForceMagnitude: 1.0f));

                                            hb.Event("onEqualPriorityHitBox", (hitInfo) => {
                                                if (hitInfo.hitEntity != fgChar) {
                                                    fgChar.PredeterminedActions("verticalClash");
                                                }
                                            });
                                        });

                                        hit.HitBox(tipperHitBox);

                                        hit.Event("onHitFGChar", (hitBox, hitInfo) => {
                                            if (hitBox == tipperHitBox) {
                                                // do extra stuff.
                                            }
                                            hitBox.InvokeEvent("onHitFGChar", hitInfo);
                                        });
                                    });
                                fl.To(14);
                                fl.From(14);
                                    // Recovery
                                fl.To(30);
                            }));
                        atkBuilder.CleanUp(ReturnToStand);
                    });

                    Attack atkSpw5AAA = new Attack(atkBuilder => { atkBuilder
                        .Name("southpaw_5AAA")
                        .Orientation(Orientation.SOUTHPAW)
                        .GroundRelation(GroundRelation.GROUNDED)
                        .Input(InputNotation._5A)
                        .AnimationState("5AAA")
                        .Movement(xMoveCb: attackMovementMap["southpaw_5AAA.x"].Evaluate, zMoveCb: attackMovementMap["southpaw_5AAA.z"].Evaluate)
                        .FramesContinuous((localFrame, targetPos) => {
                            if (localFrame >= 22.0f && localFrame <= 30.0f) {
                                float p = (localFrame - 22.0f) / 30.0f;
                                Quaternion rot = Quaternion.Euler(0.0f, -180f * p + Vector3.SignedAngle(Vector3.right, fgChar.position - targetPos, Vector3.up), 0.0f);
                                fgChar.SetRotation(rot);
                                    // fgChar.GetOrientation is done automatically on each frame
                            }
                        })
                        .Frames(
                            FrameUtil.CreateList(f => { f
                                .SpecialCancellable(true)
                                .CancellableOnWhiff(false)
                                .From(1)
                                    .ChainCancellable(false)
                                .To(20)
                                .From(20)
                                    .Hit(hit => {
                                        hit.HitDamage(1000)
                                            .BlockDamage(0)
                                            .HitStun(30.0f)
                                            .BlockStun(12.0f)
                                            .Tracking(true)
                                            .Priority(AttackPriority.MediumAttack);

                                        hit.HitBox(new HitBox(hb => {
                                            hb.Relative(fgChar.transform);
                                            hb.Point0(new Vector3(0, 0, 0));
                                            hb.Point1(new Vector3(0, 1, 0));
                                            hb.Radius(0.25f);
                                            hb.Event("onHitFGChar",
                                                CommonGroundedOnHitFGChar(
                                                    hitSound: audioMap["mediumHit"],
                                                    groundedForceMagnitude: 0.5f,
                                                    airborneForceDirection: new Vector3(0.0f, 1.0f, 1.0f),
                                                    airborneForceMagnitude: 1.0f));

                                            hb.Event("onEqualPriorityHitBox", (hitInfo) => {
                                                if (hitInfo.hitEntity != fgChar) {
                                                    fgChar.PredeterminedActions("horizontalClashSwingFromLeft");
                                                }
                                            });
                                        }));
                                    })
                                .To(21)
                                .From(22)
                                    // Recovery
                                .To(30);
                            }))
                        .CleanUp(ReturnToStand);
                    });

                    Attack atkSpw2A = new Attack(atkBuilder => { atkBuilder
                        .Name("southpaw_2A")
                        .Orientation(Orientation.SOUTHPAW)
                        .GroundRelation(GroundRelation.GROUNDED)
                        .Input(InputNotation._2A)
                        .AnimationState("2A")
                        .Movement(attackMovementMap["southpaw_2A.x"].Evaluate, null, attackMovementMap["southpaw_2A.z"].Evaluate)
                        .Frames(
                            FrameUtil.CreateList(f => { f
                                .SpecialCancellable(true)
                                .ChainCancellable(false)
                                .CancellableOnWhiff(false)
                                .From(8)
                                    .Hit(hit => {
                                        hit.HitDamage(600)
                                            .BlockDamage(0)
                                            .HitStun(10.0f)
                                            .BlockStun(8.0f);

                                        hit.Block(Block.LOW);

                                        hit.HitBox(new HitBox(hb => {
                                            hb.Relative(fgChar.transform);
                                            hb.Point0(new Vector3(0, 0, 0));
                                            hb.Point1(new Vector3(0, 1, 0));
                                            hb.Radius(0.25f);
                                            hb.Event("onHitFGChar", (hitInfo) => {
                                                FightingGameCharacter opponent = (FightingGameCharacter) hitInfo.hitEntity;
                                                if (opponent != fgChar) {
                                                    audioService.PlayOneShot(hitInfo.position, audioMap["weakHit"]);
                                                    fgService.Hit(hitInfo.hitEntity, hitInfo.hitBox, (hitAtSameTimeByAttackPriority) => {
                                                        opponent.ChangeHealth(hitInfo.damage); // hitInfo.damage will include combo scaling.
                                                        opponent.KnockBack(
                                                            launch: false,
                                                            forceDirection: fgChar.transform.rotation * new Vector3(0.0f, 1.0f, 1.0f),
                                                            forceMagnitude: 1.0f);
                                                    });
                                                }
                                            });
                                        }));
                                    })
                                .To(9)
                                .From(14)
                                    .ChainCancellable(true)
                                .To(22);
                            }))
                        .CleanUp(ReturnToCrouch);
                    });

                    Attack atkSpw2AA = new Attack(atkBuilder => { atkBuilder
                        .Name("southpaw_2AA")
                        .Orientation(Orientation.SOUTHPAW)
                        .GroundRelation(GroundRelation.GROUNDED)
                        .Input(InputNotation._2A)
                        .AnimationState("2AA")
                        .Movement(attackMovementMap["southpaw_2AA.x"].Evaluate, null, attackMovementMap["southpaw_2AA.z"].Evaluate)
                        .Frames(
                            FrameUtil.CreateList(f => { f
                                .SpecialCancellable(true)
                                .ChainCancellable(false)
                                .CancellableOnWhiff(false)
                                .From(4)
                                    .Track((currTarget, actualTarget) => {
                                        Vector3 newTargetPos = TargetUtil.MoveTargetLimited(fgChar.position, currTarget, actualTarget.position, 20.0f);
                                            // for now, just let the setting of this value be instantaneous once per frame.
                                        fgChar.SetTarget(newTargetPos);
                                    })
                                .To(13)
                                .From(13)
                                    .Hit(hit => {
                                        hit.HitDamage(800)
                                            .BlockDamage(0)
                                            .HitStun(20.0f)
                                            .BlockStun(20.0f)
                                            .ComboScaling(0.5f);

                                        hit.Block(Block.LOW);

                                        hit.HitBox(new HitBox(hb => {
                                            hb.Relative(fgChar.transform);
                                            hb.Point0(new Vector3(0, 0, 0));
                                            hb.Point1(new Vector3(0, 1, 0));
                                            hb.Radius(0.25f);
                                            hb.Event("onHitFGChar",
                                                CommonGroundedOnHitFGChar(
                                                    hitSound: audioMap["weakHit"],
                                                    groundedForceMagnitude: 0.5f,
                                                    airborneForceDirection: new Vector3(0.0f, 1.0f, 1.0f),
                                                    airborneForceMagnitude: 1.0f));
                                        }));
                                    })
                                .To(14)
                                    //Recovery
                                .To(28);
                            }))
                        .CleanUp(ReturnToCrouch);
                    });

                    Attack atkJumpSpw5A = new Attack(atkBuilder => { atkBuilder
                        .Name("jump_southpaw_5A")
                        .Orientation(Orientation.ORTHODOX)
                        .GroundRelation(GroundRelation.AIRBORNE)
                        .Input(InputNotation._5A)
                        .AnimationState("j.A")
                        .FramesContinuous((localFrame, target) => {
                            if (localFrame >= 8.0f && localFrame <= 11.0f) {
                                float p = Mathf.Lerp(8.0f, 11.0f, localFrame);
                                fgChar.SetStandCollider(p * 0.5f * Vector3.up);
                            }
                        })
                        .Frames(
                            FrameUtil.CreateList(f => { f
                                .SpecialCancellable(true)
                                .ChainCancellable(true)
                                .CancellableOnWhiff(true)
                                .To(8)
                                .From(6)
                                    .CancellableOnWhiff(false)
                                .From(8)
                                    .Hit(hit => {
                                        hit.HitDamage(700)
                                            .BlockDamage(0)
                                            .HitStun(12.0f)
                                            .BlockStun(30.0f);

                                        hit.HitBox(new HitBox(hb => {
                                            hb.Relative(fgChar.transform);
                                            hb.Point0(new Vector3(0, 0, 0));
                                            hb.Point1(new Vector3(0, 1, 0));
                                            hb.Radius(0.25f);
                                            hb.Event("onHitFGChar",
                                                CommonGroundedOnHitFGChar(
                                                    hitSound: audioMap["mediumHit"],
                                                    groundedForceMagnitude: 0.5f,
                                                    airborneForceDirection: new Vector3(0.0f, 1.0f, 1.0f),
                                                    airborneForceMagnitude: 1.0f));
                                        }));
                                    })
                                .To(11)
                                .From(13)
                                    .ChainCancellable(true)
                                .To(23);
                            }))
                        .CleanUp(ReturnToAirborne);
                    });

                    Attack atkJumpSpw5AA = new Attack(atkBuilder => { atkBuilder
                        .Name("jump_southpaw_5AA")
                        .Orientation(Orientation.SOUTHPAW)
                        .GroundRelation(GroundRelation.AIRBORNE)
                        .Input(InputNotation._5A)
                        .AnimationState("j.AA")
                        .FramesContinuous((localFrame, target) => {
                            if (localFrame >= 8.0f && localFrame <= 11.0f) {
                                float p = Mathf.Lerp(8.0f, 11.0f, localFrame);
                                fgChar.SetStandCollider(p * 0.5f * Vector3.up);
                            }
                        })
                        .Frames(
                            FrameUtil.CreateList(f => { f
                                .SpecialCancellable(true)
                                .ChainCancellable(false)
                                .CancellableOnWhiff(false)
                                .From(4)
                                    .Track((currTarget, actualTarget) => {
                                        Vector3 newTargetPos = TargetUtil.MoveTargetLimited(fgChar.position, currTarget, actualTarget.position, 20.0f);
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

                                        hit.HitBox(new HitBox(hb => {
                                            hb.Relative(fgChar.transform);
                                            hb.Point0(new Vector3(0, 0, 0));
                                            hb.Point1(new Vector3(0, 1, 0));
                                            hb.Radius(0.25f);
                                            hb.Event("onHitFGChar",
                                                CommonGroundedOnHitFGChar(
                                                    hitSound: audioMap["weakHit"],
                                                    groundedForceMagnitude: 0.5f,
                                                    airborneForceDirection: new Vector3(0.0f, 1.0f, 1.0f),
                                                    airborneForceMagnitude: 1.0f));
                                        }));
                                    })
                                .To(13)
                                .From(16)
                                    .ChainCancellable(true)
                                .To(30);
                            }))
                        .CleanUp(ReturnToAirborne);
                    });

                    Attack atkJumpSpw5AAA = new Attack(atkBuilder => { atkBuilder
                        .Name("jump_southpaw_5AAA")
                        .Orientation(Orientation.SOUTHPAW)
                        .GroundRelation(GroundRelation.AIRBORNE)
                        .Input(InputNotation._5A)
                        .AnimationState("j.AAA")
                        .FramesContinuous((localFrame, target) => {
                            if (localFrame >= 8.0f && localFrame <= 11.0f) {
                                float p = Mathf.Lerp(8.0f, 11.0f, localFrame);
                                fgChar.SetStandCollider(p * 0.5f * Vector3.up);
                            }
                        })
                        .Frames(
                            FrameUtil.CreateList(f => { f
                                .SpecialCancellable(true)
                                .ChainCancellable(false)
                                .CancellableOnWhiff(false)
                                .From(4)
                                    .Track((currTarget, actualTarget) => {
                                        Vector3 newTargetPos = TargetUtil.MoveTargetLimited(fgChar.position, currTarget, actualTarget.position, 20.0f);
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

                                        hit.HitBox(new HitBox(hb => {
                                            hb.Relative(fgChar.transform);
                                            hb.Point0(new Vector3(0, 0, 0));
                                            hb.Point1(new Vector3(0, 1, 0));
                                            hb.Radius(0.25f);
                                            hb.Event("onHitFGChar", (hitInfo) => {
                                                FightingGameCharacter opponent = (FightingGameCharacter) hitInfo.hitEntity;
                                                if (opponent != fgChar) {
                                                    audioService.PlayOneShot(hitInfo.position, audioMap["strongHit"]);
                                                        // This exists to make characters hitting each other async
                                                    fgService.Hit(hitInfo.hitEntity, hitInfo.hitBox, (hitAtSameTimeByAttackPriority) => {
                                                        opponent.ChangeHealth(hitInfo.damage); // hitInfo.damage will include combo scaling.

                                                        if (hitAtSameTimeByAttackPriority == AttackPriority.HeavyAttack) { // colloquially known as trading
                                                            cameraService.PredeterminedActions<float>("zoomIn", 0.5f);
                                                            timeService.PredeterminedActions<float>("timeSlow", 0.5f);

                                                            opponent.KnockBack(
                                                                launch: false,
                                                                forceDirection: fgChar.transform.rotation * Vector3.forward,
                                                                forceMagnitude: 0.5f);
                                                        }
                                                        else {
                                                            opponent.KnockBack(
                                                                launch: true,
                                                                forceDirection: fgChar.transform.rotation * new Vector3(0.0f, 1.0f, 1.0f),
                                                                forceMagnitude: 2.0f);
                                                        }
                                                    });
                                                }
                                            });
                                            hb.Event("onHitProjectile", (hitInfo) => {
                                                Projectile projectile = (Projectile) hitInfo.hitEntity;
                                                if (projectile.health <= 2) {
                                                    fgService.Hit(projectile, hitInfo.hitBox, (hitAtSameTimeByAttackPriority) => {
                                                        if (hitAtSameTimeByAttackPriority != AttackPriority.None) {
                                                            particleService.PlayOneShot(hitInfo.position, particleMap["destroyParticle"]);
                                                                // these lines might be redundant
                                                            particleService.PlayOneShot(hitInfo.position, projectile.DestroyedParticle());
                                                            projectile.RemoveSelf();
                                                        }
                                                    });
                                                }
                                            });
                                        }));
                                    })
                                .To(21)
                                    // Recovery
                                .To(40);
                            }))
                        .CleanUp(ReturnToAirborne);
                    });

                    Attack hadouken = new Attack(atkBuilder => { atkBuilder
                        .Name("hadouken")
                        .Orientation(Orientation.SOUTHPAW)
                        .GroundRelation(GroundRelation.GROUNDED)
                        .Input(InputNotation._236C)
                        .AnimationState("hadouken")
                        .Movement(null, null, attackMovementMap["hadouken.z"].Evaluate)
                        .Frames(
                            FrameUtil.CreateList(f => { f
                                .SpecialCancellable(false)
                                .ChainCancellable(false)
                                .From(3)
                                    .Sound(audioMap["hadouken"], soundResource => {
                                        soundResource.transform.position = fgChar.GetSpeakPosition();
                                        audioService.Play(soundResource);
                                    })
                                    .Track((currTarget, actualTarget) => {
                                        Vector3 newTargetPos = TargetUtil.MoveTargetLimited(fgChar.position, currTarget, actualTarget.position, 5.0f);
                                            // for now, just let the setting of this value be instantaneous once per frame.
                                        fgChar.SetTarget(newTargetPos);
                                    })
                                .To(46)
                                .From(12)
                                    .Projectile(projectileMap["hadouken"], proj => {
                                        projectileService.FireProjectile(proj.id);
                                    })
                                .To(13);
                            }))
                        .CleanUp(ReturnToStand);
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
                        .Attack(atkJumpSpw5A, (charState, currAttack, prevAttacks) => {
                            if (currAttack == null) {
                                return true;
                            }
                            return false;
                        })
                        .Attack(atkJumpSpw5AA, (charState, currAttack, prevAttacks) => {
                            if (currAttack == atkJumpSpw5A) {
                                return true;
                            }
                            return false;
                        })
                        .Attack(atkJumpSpw5AAA, (charState, currAttack, prevAttacks) => {
                            if (currAttack == atkJumpSpw5AA) {
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

                private Action<HitInfo> CommonGroundedOnHitFGChar(AudioClip hitSound, float groundedForceMagnitude, Vector3 airborneForceDirection, float airborneForceMagnitude) {
                    return hitInfo => {
                        FightingGameCharacter opponent = (FightingGameCharacter) hitInfo.hitEntity;
                        if (opponent != fgChar) {
                            audioService.PlayOneShot(hitInfo.position, hitSound);
                                // This exists to make characters hitting each other async
                            fgService.Hit(hitInfo.hitEntity, hitInfo.hitBox, (hitAtSameTimeByAttackPriority) => {
                                opponent.ChangeHealth(hitInfo.damage); // hitInfo.damage will include combo scaling.

                                switch (opponent.GetGroundRelation()) {
                                    case GroundRelation.GROUNDED:
                                        opponent.KnockBack(
                                            launch: false,
                                            forceDirection: fgChar.transform.rotation * Vector3.forward,
                                            forceMagnitude: groundedForceMagnitude);
                                        break;
                                    case GroundRelation.AIRBORNE:
                                        opponent.KnockBack(
                                            launch: true,
                                            forceDirection: fgChar.transform.rotation * airborneForceDirection,
                                            forceMagnitude: airborneForceMagnitude);
                                        break;
                                }
                            });
                        }
                    };
                }
            }
        }
    }
}