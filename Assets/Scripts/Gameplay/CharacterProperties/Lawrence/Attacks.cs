﻿using System;
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

                private IHitService hitService;
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

                    hitService = services.GetService<IHitService>();
                    hitBoxService = services.GetService<IHitBoxService>();

                    this.audioMap = audioMap;
                    this.particleMap = particleMap;
                    this.animationCurveMap = animationCurveMap;
                    this.projectileMap = projectileMap;
                }

                public void Init(ICharacterPropertiesCallbackObj charBuilder, FightingGameCharacter newFightingGameChar) {
                    fgChar = newFightingGameChar;

                    Attack throwNormal = null;
                    throwNormal = new Attack(atkBuilder => {
                        atkBuilder.Name("orthodox_throwNormal");
                        atkBuilder.Requires(Orientation.ORTHODOX, GroundRelation.GROUNDED);
                        atkBuilder.Input("A+D");
                        atkBuilder.MovementDetails(additive:false);
                        atkBuilder.StartGroup("grabInitiate");
                        atkBuilder.Group("grabInitiate", atkBuilderGrouping => {
                            atkBuilderGrouping.AnimationState("grab");
                            atkBuilderGrouping.InitCharState((CharacterStates.Attack)fgChar.State("attackGrounded"));
                            atkBuilderGrouping.Movement(null, null, zMoveCb: animationCurveMap["orth_throwNormal_init.z"].Evaluate);
                            atkBuilderGrouping.Frames(
                                FrameUtil.CreateList(fl => {
                                    fl.SpecialCancellable(false);
                                    fl.CancellableOnWhiff(false); // TODO: Change this to true when you fix the input buffer
                                    fl.ChainCancellable(false);
                                    fl.CounterHit(true);
                                    fl.From(2);
                                    fl.Execute(() => {
                                        audioService.PlayOneShot(fgChar.GetSpeakPosition(), audioMap["attacksound_04"]);
                                    });
                                    fl.To(3);
                                    fl.From(12);
                                    fl.Hit(hit => {
                                        hit.Priority(AttackPriority.Throw);

                                        HitBox defaultHitBox = hitBoxService.Create(hb => {
                                            hb.Relative(fgChar.transform);
                                            hb.Point0(new Vector3(0, 0, 0.5f));
                                            hb.Point1(new Vector3(0, 1.1f, 0.5f));
                                            hb.Radius(0.2f);
                                            hb.InGameEntity(fgChar);
                                            hb.Validate((HitBox _, HitBox other) => false);
                                            hb.Validate((HitBox _, HurtBox other) => true);
                                        });

                                        hit.OnHit((thisHit, hitLocations, entity, hurtBoxes, hitBoxes) => {
                                            if (fgService.IsCurrentFGChar(entity) && entity != fgChar) {
                                                FightingGameCharacter opponent = (FightingGameCharacter)entity;
                                                fgService.Throw(opponent, fgChar, thisHit, onGrabbed: (hitAtSameTimeByAttackPriority) => {
                                                    if (hitAtSameTimeByAttackPriority == AttackPriority.Throw) {
                                                        fgChar.PredeterminedActions("grabBreak");
                                                        opponent.PredeterminedActions("grabBreak");
                                                    }
                                                    else if (hitAtSameTimeByAttackPriority == AttackPriority.None) {
                                                        throwNormal.SaveEntity("opponent", opponent);
                                                        throwNormal.UseGroup("grabConnect");
                                                        throwNormal.ResetTracker();
                                                        throwNormal.SetAnimation();

                                                        fgService.Grabs(fgChar, opponent, true, fgChar.position + fgChar.rigidFG.rotation * Vector3.forward * 0.5f, 15);
                                                    }
                                                    else {
                                                        // lose to strike
                                                    }
                                                });
                                            }
                                        });

                                        hit.HitBox(defaultHitBox);
                                    });
                                    fl.To(17);
                                    fl.From(13);
                                    fl.CounterHit(false);
                                    fl.To(22);
                                }));
                            atkBuilderGrouping.CleanUp(ReturnToStand);
                        });
                        atkBuilder.Group("grabConnect", atkBuilderGrouping => {
                            atkBuilderGrouping.AnimationState("grab_hit");
                            atkBuilderGrouping.InitCharState((CharacterStates.Attack)fgChar.State("attackGrounded"));
                            atkBuilderGrouping.Movement(null, null, zMoveCb: animationCurveMap["orth_throwNormal_connect.z"].Evaluate);
                            Hit grabStrike0 = hitService.Create(ht => {
                                ht.HitDamage(400);
                                ht.HitStun(32.0f);
                                ht.ComboScaling(1.0f);
                            });
                            Hit grabStrike1 = hitService.Create(ht => {
                                ht.HitDamage(800);
                                ht.HitStun(60.0f);
                                ht.ComboScaling(1.0f);
                            });
                            atkBuilderGrouping.FramesContinuous((localFrame, target) => {
                                if (localFrame >= 0.0f && localFrame <= 25.0f) {
                                    fgService.MaintainsGrab(fgChar, (FightingGameCharacter) throwNormal.GetEntity("opponent"));
                                }
                            });
                            atkBuilderGrouping.Frames(
                                FrameUtil.CreateList(fl => {
                                    fl.From(1);
                                        fl.SpecialCancellable(false);
                                        fl.CancellableOnWhiff(false); // TODO: Change this to true when you fix the input buffer
                                        fl.ChainCancellable(false);
                                        fl.CounterHit(false);
                                    fl.To(16);
                                    fl.From(15);
                                        fl.Execute(() => {
                                            fgService.SetGrabBreakability(fgChar, false);
                                        });
                                    fl.To(16);
                                    fl.From(16);
                                        //fl.Hit(grabStrike0);
                                        fl.Execute(() => {
                                            FightingGameCharacter opponent = (FightingGameCharacter) throwNormal.GetEntity("opponent");
                                            audioService.PlayOneShot(fgChar.GetSpeakPosition(), audioMap["hit"]);
                                            PlayRandomGrunt(fgChar.GetSpeakPosition());

                                            particleService.PlayOneShot(fgChar.position + fgChar.rigidFG.rotation * new Vector3(0, 1.0f, 1.0f) * 1.0f, "hit");

                                            fgService.IncrementComboCounter(opponent);
                                            opponent.ReceiveHit(grabStrike0.hitStun, false);
                                            opponent.ChangeHealth(fgService.GetComboScaleDamage(opponent, grabStrike0.comboScaling, grabStrike0.hitDamage));
                                        });
                                    fl.To(17);
                                    fl.From(24);
                                        //fl.Hit(grabStrike1);
                                        fl.Execute(() => {
                                            FightingGameCharacter opponent = (FightingGameCharacter) throwNormal.GetEntity("opponent");
                                            audioService.PlayOneShot(fgChar.GetSpeakPosition(), audioMap["hit"]);
                                            PlayRandomGrunt(fgChar.GetSpeakPosition());

                                            particleService.PlayOneShot(fgChar.position + fgChar.rigidFG.rotation * new Vector3(0, 1.0f, 1.0f) * 1.0f, "hit");

                                            fgService.IncrementComboCounter(opponent);
                                            opponent.ReceiveHit(grabStrike1.hitStun, false);
                                            opponent.KnockBack(
                                                grabStrike1.priority,
                                                launch: false,
                                                knockback: fgChar.rigidFG.rotation * Vector3.forward * 3.0f);
                                            opponent.ChangeHealth(fgService.GetComboScaleDamage(opponent, grabStrike1.comboScaling, grabStrike1.hitDamage));
                                        });
                                    fl.To(25);
                                        fl.CounterHit(false);
                                    fl.To(45);
                                }));
                            atkBuilderGrouping.CleanUp(() => {
                                fgChar.SetState(fgChar.State("stand"));
                            });
                        });
                    });

                    Attack atkOrt5A = new Attack(atkBuilder => {
                        atkBuilder.Name("orthodox_5A");
                        atkBuilder.Requires(Orientation.ORTHODOX, GroundRelation.GROUNDED);
                        atkBuilder.Input("4|5", "A");
                        atkBuilder.AnimationState("5A");
                        atkBuilder.InitCharState((CharacterStates.Attack)fgChar.State("attackGrounded"));
                        atkBuilder.Movement(null, null, null);
                        atkBuilder.MovementDetails(additive:false);
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
                                        hit.HitStun(24.0f);
                                        hit.BlockStun(8.0f);
                                        hit.ComboScaling(0.65f);
                                        hit.Priority(AttackPriority.LightAttack);

                                        hit.ValidBlock(BlockType.LOW, BlockType.HIGH);

                                        HitBox defaultHitBox = hitBoxService.Create(hb => {
                                            hb.Relative(fgChar.transform);
                                            hb.Point0(new Vector3(0, 0, 0.5f));
                                            hb.Point1(new Vector3(0, 1.1f, 1.3f));
                                            hb.Radius(0.25f);
                                            hb.InGameEntity(fgChar);
                                            hb.Validate((HitBox _, HitBox other) => false);
                                            hb.Validate((HitBox _, HurtBox other) => true);
                                        });

                                        hit.OnHit((thisHit, hitLocations, entity, hurtBoxes, hitBoxes) => {
                                            if (fgService.IsCurrentFGChar(entity) && entity != fgChar) {
                                                FightingGameCharacter opponent = (FightingGameCharacter) entity;

                                                // This exists to make characters hitting each other async
                                                fgService.Strike(entity, fgChar, thisHit, onHit:(hitAtSameTimeByAttackPriority, damage) => {
                                                    audioService.PlayOneShot(hitLocations[defaultHitBox], audioMap["hit"]);
                                                    PlayRandomGrunt(hitLocations[defaultHitBox]);

                                                    particleService.PlayOneShot(fgChar.position + fgChar.rigidFG.rotation * new Vector3(0, 1.0f, 1.0f) * 1.0f, "hit");

                                                    switch (opponent.GetGroundRelation()) {
                                                        case GroundRelation.GROUNDED:
                                                            opponent.ReceiveHit(thisHit.hitStun, false);
                                                            opponent.KnockBack(
                                                                thisHit.priority,
                                                                launch: false,
                                                                knockback: fgChar.rigidFG.rotation * Vector3.forward * 2.0f);
                                                            break;
                                                        case GroundRelation.AIRBORNE:
                                                            opponent.ReceiveHit(thisHit.hitStun, true);
                                                            opponent.KnockBack(
                                                                thisHit.priority,
                                                                launch: true,
                                                                knockback: fgChar.rigidFG.rotation * new Vector3(0.0f, 1.0f, 1.0f) * 0.7071f);
                                                            break;
                                                    }
                                                    opponent.ChangeHealth(damage); // damage includes combo scaling.

                                                }, onBlock: (hitAtSameTimeByAttackPriority, damage) => {
                                                    audioService.PlayOneShot(hitLocations[defaultHitBox], audioMap["block"]);

                                                    particleService.PlayOneShot(fgChar.position + fgChar.rigidFG.rotation * new Vector3(0, 1.0f, 1.0f) * 1.0f, "block");

                                                    // I may put all of these functions into the same call.
                                                    opponent.ReceiveBlocked(thisHit.blockStun, false);
                                                    opponent.KnockBack(
                                                        thisHit.priority,
                                                        launch: false,
                                                        knockback: fgChar.rigidFG.rotation * Vector3.forward * 0.5f);
                                                    opponent.ChangeHealth(damage); // damage is hit.blockDamage
                                                });
                                            }
                                        });

                                        hit.HitBox(defaultHitBox);
                                    });
                                fl.To(8);
                                fl.From(8);
                                    fl.CounterHit(false);
                                    fl.CancellableOnWhiff(true);
                                fl.From(10);
                                    fl.ChainCancellable(true);
                                fl.To(22);
                            }));
                        atkBuilder.CleanUp(ReturnToStand);
                    });

                    Attack atkOrt5AA = new Attack(atkBuilder => {
                        atkBuilder.Name("orthodox_5A,A");
                        atkBuilder.Requires(Orientation.ORTHODOX, GroundRelation.GROUNDED);
                        atkBuilder.Input("4|5", "A");
                        atkBuilder.AnimationState("5AA");
                        atkBuilder.InitCharState((CharacterStates.Attack)fgChar.State("attackGrounded"));
                        atkBuilder.Movement(zMoveCb: animationCurveMap["orth_5AA.z"].Evaluate);
                        atkBuilder.MovementDetails(additive:false);
                        atkBuilder.Frames(
                            FrameUtil.CreateList(fl => {
                                fl.SpecialCancellable(true);
                                fl.CancellableOnWhiff(false);
                                fl.CounterHit(true);
                                fl.From(1);
                                    fl.ChainCancellable(false);
                                fl.From(2);
                                fl.Execute(() => {
                                    audioService.PlayOneShot(fgChar.GetSpeakPosition(), audioMap["attacksound_05"]);
                                });
                                fl.To(3);
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
                                        hit.BlockStun(16.0f);
                                        hit.ComboScaling(1.0f);
                                        hit.Priority(AttackPriority.LightAttack);

                                        //hit.ValidBlock(BlockType.LOW, BlockType.HIGH); // defaults to low and high blocking to be valid.

                                        HitBox defaultHitBox = hitBoxService.Create(hb => {
                                            hb.Relative(fgChar.transform);
                                            hb.Point0(new Vector3(0, 0, 0.8f));
                                            hb.Point1(new Vector3(0, 1, 0.8f));
                                            hb.Radius(0.25f);
                                            hb.InGameEntity(fgChar);
                                            hb.Validate((HitBox _, HitBox other) => {
                                                return false;
                                            });
                                            hb.Validate((HitBox _, HurtBox other) => {
                                                return true;
                                            });
                                        });

                                        hit.OnHit((thisHit, hitLocations, entity, hurtBoxes, hitBoxes) => {
                                            if (fgService.IsCurrentFGChar(entity) && entity != fgChar) {
                                                FightingGameCharacter opponent = (FightingGameCharacter) entity;

                                                // This exists to make characters hitting each other async
                                                fgService.Strike(entity, fgChar, thisHit, (hitAtSameTimeByAttackPriority, damage) => {
                                                    audioService.PlayOneShot(hitLocations[defaultHitBox], audioMap["hit"]);
                                                    PlayRandomGrunt(hitLocations[defaultHitBox]);

                                                    particleService.PlayOneShot(fgChar.position + fgChar.rigidFG.rotation * new Vector3(0, 1.0f, 1.0f) * 1.0f, "hit");

                                                    switch (opponent.GetGroundRelation()) {
                                                        case GroundRelation.GROUNDED:
                                                            opponent.ReceiveHit(thisHit.hitStun, false);
                                                            opponent.KnockBack(
                                                                thisHit.priority,
                                                                launch: false,
                                                                knockback: fgChar.transform.rotation * Vector3.forward * 3.0f);
                                                            break;
                                                        case GroundRelation.AIRBORNE:
                                                            opponent.ReceiveHit(thisHit.hitStun, true);
                                                            opponent.KnockBack(
                                                                thisHit.priority,
                                                                launch: true,
                                                                knockback: fgChar.rigidFG.rotation * new Vector3(0.0f, 1.0f, 1.0f) * 0.7071f);
                                                            break;
                                                    }
                                                    opponent.ChangeHealth(damage); // damage includes combo scaling.

                                                }, onBlock: (hitAtSameTimeByAttackPriority, damage) => {
                                                    audioService.PlayOneShot(hitLocations[defaultHitBox], audioMap["block"]);

                                                    particleService.PlayOneShot(fgChar.position + fgChar.rigidFG.rotation * new Vector3(0, 1.0f, 1.0f) * 1.0f, "block");

                                                    opponent.ReceiveBlocked(thisHit.blockStun, false);
                                                    opponent.KnockBack(
                                                        thisHit.priority,
                                                        launch: false,
                                                        knockback: fgChar.rigidFG.rotation * Vector3.forward * 0.5f);
                                                    opponent.ChangeHealth(damage); // damage is hit.blockDamage
                                                });
                                            }
                                        });

                                        hit.HitBox(defaultHitBox);
                                    });
                                fl.To(14);
                                fl.From(14);
                                    fl.CounterHit(false);
                                    fl.CancellableOnWhiff(true);
                                fl.To(30);
                            }));
                        atkBuilder.CleanUp(ReturnToStand);
                    });

                    Attack atkOrt5AAA = new Attack(atkBuilder => {
                        atkBuilder.Name("orthodox_5A,A,A");
                        atkBuilder.Requires(Orientation.ORTHODOX, GroundRelation.GROUNDED);
                        atkBuilder.Input("4|5", "A");
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
                        atkBuilder.MovementDetails(additive:false);
                        atkBuilder.Frames(
                            FrameUtil.CreateList(f => { f
                                .SpecialCancellable(true)
                                .CancellableOnWhiff(false)
                                .CounterHit(true)
                                .From(1)
                                    .ChainCancellable(false)
                                .From(2)
                                    .Execute(() => {
                                        audioService.PlayOneShot(fgChar.GetSpeakPosition(), audioMap["attacksound_03"]);
                                    })
                                .To(3)
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
                                                return false;
                                            });
                                            hb.Validate((HitBox _, HurtBox other) => {
                                                return true;
                                            });
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
                                        });

                                        hit.HitBox(farHitBox);
                                        hit.HitBox(closeHitBox);

                                        hit.OnHit((thisHit, hitLocations, entity, hurtBoxes, hitBoxes) => {
                                            if (fgService.IsCurrentFGChar(entity) && entity != fgChar) {
                                                FightingGameCharacter opponent = (FightingGameCharacter)entity;

                                                // This exists to make characters hitting each other async
                                                fgService.Strike(entity, fgChar, thisHit, onHit: (hitAtSameTimeByAttackPriority, damage) => {
                                                    audioService.PlayOneShot(hitLocations[closeHitBox], audioMap["hit"]);
                                                    PlayRandomGrunt(hitLocations[closeHitBox]);

                                                    particleService.PlayOneShot(fgChar.position + fgChar.rigidFG.rotation * new Vector3(0, 1.0f, 1.0f) * 1.0f, "hit");

                                                    opponent.ReceiveHit(thisHit.hitStun, true);
                                                    opponent.KnockBack(
                                                        thisHit.priority,
                                                        launch: true,
                                                        knockback: fgChar.rigidFG.rotation * new Vector3(0.0f, 4.5f, 1.5f));
                                                    opponent.ChangeHealth(damage); // damage includes combo scaling.

                                                }, onBlock: (hitAtSameTimeByAttackPriority, damage) => {
                                                    audioService.PlayOneShot(hitLocations[closeHitBox], audioMap["block"]);

                                                    particleService.PlayOneShot(fgChar.position + fgChar.rigidFG.rotation * new Vector3(0, 1.0f, 1.0f) * 1.0f, "block");

                                                    opponent.ReceiveBlocked(thisHit.blockStun, false);
                                                    opponent.KnockBack(
                                                        thisHit.priority,
                                                        launch: false,
                                                        knockback: fgChar.rigidFG.rotation * Vector3.forward * 0.5f);
                                                    opponent.ChangeHealth(damage); // damage is hit.blockDamage
                                                });
                                            }
                                        });
                                    })
                                .To(16)
                                .From(21)
                                    .ChainCancellable(true)
                                    .CancellableOnWhiff(true)
                                    .CounterHit(false)
                                .To(30);
                            }));
                        atkBuilder.CleanUp(ReturnToStand);
                    });

                    Attack atkOrt2A = new Attack(atkBuilder => {
                        atkBuilder.Name("orthodox_2A");
                        atkBuilder.Requires(Orientation.ORTHODOX, GroundRelation.GROUNDED);
                        atkBuilder.Input("1|2|3", "A");
                        atkBuilder.AnimationState("2A");
                        atkBuilder.InitCharState((CharacterStates.Attack)fgChar.State("attackGrounded"));
                        atkBuilder.Movement();
                        atkBuilder.MovementDetails(additive:false);
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
                                            .HitStun(22.0f)
                                            .BlockStun(20.0f);

                                        hit.ValidBlock(BlockType.LOW);

                                        HitBox defaultHitBox = hitBoxService.Create(hb => {
                                            hb.Relative(fgChar.transform);
                                            hb.Point0(new Vector3(0, 0.1f, 0.0f));
                                            hb.Point1(new Vector3(0, 0.1f, 1.3f));
                                            hb.Radius(0.25f);
                                            hb.InGameEntity(fgChar);
                                            hb.Validate((HitBox _, HitBox other) => false);
                                            hb.Validate((HitBox _, HurtBox other) => true);
                                        });

                                        hit.OnHit((thisHit, hitLocations, entity, hurtBoxes, hitBoxes) => {
                                            if (fgService.IsCurrentFGChar(entity) && entity != fgChar) {
                                                FightingGameCharacter opponent = (FightingGameCharacter)entity;

                                                // This exists to make characters hitting each other async
                                                fgService.Strike(entity, fgChar, thisHit, onHit: (hitAtSameTimeByAttackPriority, damage) => {
                                                    audioService.PlayOneShot(hitLocations[defaultHitBox], audioMap["hit"]);
                                                    PlayRandomGrunt(hitLocations[defaultHitBox]);

                                                    particleService.PlayOneShot(fgChar.position + fgChar.rigidFG.rotation * new Vector3(0, 1.0f, 1.0f) * 1.0f, "hit");

                                                    switch (opponent.GetGroundRelation()) {
                                                        case GroundRelation.GROUNDED:
                                                            opponent.ReceiveHit(thisHit.hitStun, false);
                                                            opponent.KnockBack(
                                                                thisHit.priority,
                                                                launch: false,
                                                                knockback: fgChar.rigidFG.rotation * Vector3.forward * 2.0f);
                                                            break;
                                                        case GroundRelation.AIRBORNE:
                                                            opponent.ReceiveBlocked(thisHit.hitStun, true);
                                                            opponent.KnockBack(
                                                                thisHit.priority,
                                                                launch: true,
                                                                knockback: fgChar.rigidFG.rotation * new Vector3(0.0f, 1.0f, 1.0f) * 0.7071f);
                                                            break;
                                                    }
                                                    opponent.ChangeHealth(damage); // damage includes combo scaling.

                                                }, onBlock: (hitAtSameTimeByAttackPriority, damage) => {
                                                    audioService.PlayOneShot(hitLocations[defaultHitBox], audioMap["block"]);

                                                    particleService.PlayOneShot(fgChar.position + fgChar.rigidFG.rotation * new Vector3(0, 1.0f, 1.0f) * 1.0f, "block");

                                                    // I may put all of these functions into the same call.
                                                    opponent.ReceiveBlocked(thisHit.blockStun, false);
                                                    opponent.KnockBack(
                                                        thisHit.priority,
                                                        launch: false,
                                                        knockback: fgChar.rigidFG.rotation * Vector3.forward * 0.5f);
                                                    opponent.ChangeHealth(damage); // damage is hit.blockDamage
                                                });
                                            }
                                        });

                                        hit.HitBox(defaultHitBox);
                                    })
                                .To(9)
                                .From(14)
                                    .ChainCancellable(true)
                                    .CancellableOnWhiff(true)
                                    .CounterHit(false)
                                .To(22);
                            }));
                        atkBuilder.CleanUp(ReturnToCrouch);
                    });

                    Attack atkOrt2AA = new Attack(atkBuilder => {
                        atkBuilder.Name("orthodox_2A,A");
                        atkBuilder.Requires(Orientation.ORTHODOX, GroundRelation.GROUNDED);
                        atkBuilder.Input("1|2|3", "A");
                        atkBuilder.AnimationState("2AA");
                        atkBuilder.InitCharState((CharacterStates.Attack)fgChar.State("attackGrounded"));
                        atkBuilder.Movement();
                        atkBuilder.MovementDetails(additive:false);
                        atkBuilder.Frames(
                            FrameUtil.CreateList(f => { f
                                .SpecialCancellable(true)
                                .ChainCancellable(false)
                                .CancellableOnWhiff(false)
                                .CounterHit(true)
                                .From(2)
                                    .Execute(() => {
                                        audioService.PlayOneShot(fgChar.GetSpeakPosition(), audioMap["attacksound_05"]);
                                    })
                                .To(3)
                                .From(10)
                                    .Hit(hit => {
                                        hit.HitDamage(800)
                                            .BlockDamage(0)
                                            .HitStun(22.0f)
                                            .BlockStun(20.0f)
                                            .ComboScaling(0.5f);

                                        hit.ValidBlock(BlockType.LOW, BlockType.HIGH);

                                        HitBox defaultHitBox = hitBoxService.Create(hb => {
                                            hb.Relative(fgChar.transform);
                                            hb.Point0(new Vector3(0, 0, 1.0f));
                                            hb.Point1(new Vector3(0, 0.8f, 1.3f));
                                            hb.Radius(0.25f);
                                            hb.InGameEntity(fgChar);
                                            hb.Validate((HitBox _, HitBox other) => false);
                                            hb.Validate((HitBox _, HurtBox other) => true);
                                        });

                                        hit.OnHit((thisHit, hitLocations, entity, hurtBoxes, hitBoxes) => {
                                            if (fgService.IsCurrentFGChar(entity) && entity != fgChar) {
                                                FightingGameCharacter opponent = (FightingGameCharacter)entity;

                                                // This exists to make characters hitting each other async
                                                fgService.Strike(entity, fgChar, thisHit, onHit: (hitAtSameTimeByAttackPriority, damage) => {
                                                    audioService.PlayOneShot(hitLocations[defaultHitBox], audioMap["hit"]);
                                                    PlayRandomGrunt(hitLocations[defaultHitBox]);

                                                    particleService.PlayOneShot(fgChar.position + fgChar.rigidFG.rotation * new Vector3(0, 1.0f, 1.0f) * 1.0f, "hit");

                                                    switch (opponent.GetGroundRelation()) {
                                                        case GroundRelation.GROUNDED:
                                                            opponent.ReceiveHit(thisHit.hitStun, false);
                                                            opponent.KnockBack(
                                                                thisHit.priority,
                                                                launch: false,
                                                                knockback: fgChar.rigidFG.rotation * Vector3.forward * 2.0f);
                                                            break;
                                                        case GroundRelation.AIRBORNE:
                                                            opponent.ReceiveBlocked(thisHit.hitStun, true);
                                                            opponent.KnockBack(
                                                                thisHit.priority,
                                                                launch: true,
                                                                knockback: fgChar.rigidFG.rotation * new Vector3(0.0f, 1.0f, 1.0f) * 0.7071f);
                                                            break;
                                                    }
                                                    opponent.ChangeHealth(damage); // damage includes combo scaling.

                                                }, onBlock: (hitAtSameTimeByAttackPriority, damage) => {
                                                    audioService.PlayOneShot(hitLocations[defaultHitBox], audioMap["block"]);

                                                    particleService.PlayOneShot(fgChar.position + fgChar.rigidFG.rotation * new Vector3(0, 1.0f, 1.0f) * 1.0f, "block");

                                                    // I may put all of these functions into the same call.
                                                    opponent.ReceiveBlocked(thisHit.blockStun, false);
                                                    opponent.KnockBack(
                                                        thisHit.priority,
                                                        launch: false,
                                                        knockback: fgChar.rigidFG.rotation * Vector3.forward * 0.5f);
                                                    opponent.ChangeHealth(damage); // damage is hit.blockDamage
                                                });
                                            }
                                        });

                                        hit.HitBox(defaultHitBox);
                                    })
                                .To(14)
                                .From(16)
                                    .CancellableOnWhiff(true)
                                    .ChainCancellable(true)
                                    .CounterHit(true)
                                .To(22);
                            }));
                        atkBuilder.CleanUp(ReturnToCrouch);
                    });

                    Attack atkOrt5B = new Attack(atkBuilder => {
                        atkBuilder.Name("orthodox_5B");
                        atkBuilder.Requires(Orientation.ORTHODOX, GroundRelation.GROUNDED);
                        atkBuilder.Input("4|5", "B");
                        atkBuilder.AnimationState("5B");
                        atkBuilder.InitCharState((CharacterStates.Attack)fgChar.State("attackGrounded"));
                        atkBuilder.Movement(zMoveCb: animationCurveMap["orth_5B.z"].Evaluate);
                        atkBuilder.MovementDetails(additive:false);
                        atkBuilder.Frames(
                            FrameUtil.CreateList(fl => {
                                fl.SpecialCancellable(true);
                                fl.CancellableOnWhiff(false);
                                fl.CounterHit(true);
                                fl.From(2);
                                    fl.Execute(() => {
                                        audioService.PlayOneShot(fgChar.GetSpeakPosition(), audioMap["attacksound_02"]);
                                    });
                                fl.To(3);
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

                                        HitBox defaultHitBox = hitBoxService.Create(hb => {
                                            hb.Relative(fgChar.transform);
                                            hb.Point0(new Vector3(x: 0, y: 0, z: 0.2f));
                                            hb.Point1(new Vector3(x: 0, y: 0.8f, z: 0.5f));
                                            hb.Radius(0.4f);
                                            hb.InGameEntity(fgChar);
                                            hb.Validate((HitBox _, HurtBox other) => true);
                                        });

                                        hit.OnHit((thisHit, hitLocations, entity, hurtBoxes, hitBoxes) => {
                                            if (fgService.IsCurrentFGChar(entity) && entity != fgChar) {
                                                FightingGameCharacter opponent = (FightingGameCharacter)entity;

                                                // This exists to make characters hitting each other async
                                                fgService.Strike(entity, fgChar, thisHit, onHit: (hitAtSameTimeByAttackPriority, damage) => {
                                                    audioService.PlayOneShot(hitLocations[defaultHitBox], audioMap["hit"]);
                                                    PlayRandomGrunt(hitLocations[defaultHitBox]);

                                                    particleService.PlayOneShot(fgChar.position + fgChar.rigidFG.rotation * new Vector3(0, 1.0f, 1.0f) * 1.0f, "hit");

                                                    switch (opponent.GetGroundRelation()) {
                                                        case GroundRelation.GROUNDED:
                                                            opponent.ReceiveHit(thisHit.hitStun, false);
                                                            opponent.KnockBack(
                                                                thisHit.priority,
                                                                launch: false,
                                                                knockback: fgChar.rigidFG.rotation * Vector3.forward * 2.0f);
                                                            break;
                                                        case GroundRelation.AIRBORNE:
                                                            opponent.ReceiveBlocked(thisHit.hitStun, true);
                                                            opponent.KnockBack(
                                                                thisHit.priority,
                                                                launch: true,
                                                                knockback: fgChar.rigidFG.rotation * new Vector3(0.0f, 1.0f, 1.0f) * 0.7071f);
                                                            break;
                                                    }
                                                    opponent.ChangeHealth(damage); // damage includes combo scaling.

                                                }, onBlock: (hitAtSameTimeByAttackPriority, damage) => {
                                                    audioService.PlayOneShot(hitLocations[defaultHitBox], audioMap["block"]);

                                                    particleService.PlayOneShot(fgChar.position + fgChar.rigidFG.rotation * new Vector3(0, 1.0f, 1.0f) * 1.0f, "block");

                                                    // I may put all of these functions into the same call.
                                                    opponent.ReceiveBlocked(thisHit.blockStun, false);
                                                    opponent.KnockBack(
                                                        thisHit.priority,
                                                        launch: false,
                                                        knockback: fgChar.rigidFG.rotation * Vector3.forward * 0.5f);
                                                    opponent.ChangeHealth(damage); // damage is hit.blockDamage
                                                });
                                            }
                                        });

                                        hit.HitBox(defaultHitBox);
                                        hit.HitBox(hitBoxService.Create(hb => {
                                            hb.Relative(fgChar.transform);
                                            hb.Point0(new Vector3(x: 1.2f, y: 0.5f, z: 0.0f));
                                            hb.Point1(new Vector3(x: 0.8f, y: 0.5f, z: 0.6f));
                                            hb.Radius(0.2f);
                                            hb.InGameEntity(fgChar);
                                        }));
                                        hit.HitBox(hitBoxService.Create(hb => {
                                            hb.Relative(fgChar.transform);
                                            hb.Point0(new Vector3(x: 0.8f, y: 0.5f, z: 0.6f));
                                            hb.Point1(new Vector3(x: 0.0f, y: 0.5f, z: 1.2f));
                                            hb.Radius(0.2f);
                                            hb.InGameEntity(fgChar);
                                        }));
                                        hit.HitBox(hitBoxService.Create(hb => {
                                            hb.Relative(fgChar.transform);
                                            hb.Point0(new Vector3(x: 0.0f, y: 0.5f, z: 1.2f));
                                            hb.Point1(new Vector3(x: -0.8f, y: 0.5f, z: 0.6f));
                                            hb.Radius(0.2f);
                                            hb.InGameEntity(fgChar);
                                        }));
                                        hit.HitBox(hitBoxService.Create(hb => {
                                            hb.Relative(fgChar.transform);
                                            hb.Point0(new Vector3(x: -0.8f, y: 0.5f, z: 0.6f));
                                            hb.Point1(new Vector3(x: -1.2f, y: 0.5f, z: 0.0f));
                                            hb.Radius(0.2f);
                                            hb.InGameEntity(fgChar);
                                        }));
                                    });
                                fl.To(17);
                                fl.From(20);
                                    fl.CounterHit(false);
                                    fl.ChainCancellable(true);
                                    fl.CancellableOnWhiff(true);
                                fl.To(30);
                            }));
                        atkBuilder.CleanUp(ReturnToStand);
                    });

                    Attack atkOrt5BB = new Attack(atkBuilder => {
                        atkBuilder.Name("orthodox_5B,B");
                        atkBuilder.Requires(Orientation.ORTHODOX, GroundRelation.GROUNDED);
                        atkBuilder.Input("4|5", "B");
                        atkBuilder.AnimationState("5BB");
                        atkBuilder.InitCharState((CharacterStates.Attack)fgChar.State("attackGrounded"));
                        atkBuilder.Movement(zMoveCb: animationCurveMap["orth_5BB.z"].Evaluate);
                        atkBuilder.MovementDetails(additive:false);
                        atkBuilder.Frames(
                            FrameUtil.CreateList(fl => {
                                fl.SpecialCancellable(true);
                                fl.CancellableOnWhiff(false);
                                fl.CounterHit(true);
                                fl.From(2);
                                    fl.Execute(() => {
                                        audioService.PlayOneShot(fgChar.GetSpeakPosition(), audioMap["attacksound_03"]);
                                    });
                                fl.To(3);
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

                                        HitBox defaultHitBox = hitBoxService.Create(hb => {
                                            hb.Relative(fgChar.transform);
                                            hb.Point0(new Vector3(0, 0, 1.0f));
                                            hb.Point1(new Vector3(0, 0.8f, 0.7f));
                                            hb.Radius(0.25f);
                                            hb.InGameEntity(fgChar);
                                            hb.Validate((HitBox _, HitBox other) => false);
                                            hb.Validate((HitBox _, HurtBox other) => true);
                                        });

                                        hit.OnHit((thisHit, hitLocations, entity, hurtBoxes, hitBoxes) => {
                                            if (fgService.IsCurrentFGChar(entity) && entity != fgChar) {
                                                FightingGameCharacter opponent = (FightingGameCharacter)entity;

                                                // This exists to make characters hitting each other async
                                                fgService.Strike(entity, fgChar, thisHit, onHit: (hitAtSameTimeByAttackPriority, damage) => {
                                                    audioService.PlayOneShot(hitLocations[defaultHitBox], audioMap["hit"]);
                                                    PlayRandomGrunt(hitLocations[defaultHitBox]);

                                                    particleService.PlayOneShot(fgChar.position + fgChar.rigidFG.rotation * new Vector3(0, 1.0f, 1.0f) * 1.0f, "hit");

                                                    switch (opponent.GetGroundRelation()) {
                                                        case GroundRelation.GROUNDED:
                                                            opponent.ReceiveHit(thisHit.hitStun, false);
                                                            opponent.KnockBack(
                                                                thisHit.priority,
                                                                launch: false,
                                                                knockback: fgChar.rigidFG.rotation * Vector3.forward * 2.0f);
                                                            break;
                                                        case GroundRelation.AIRBORNE:
                                                            opponent.ReceiveBlocked(thisHit.hitStun, true);
                                                            opponent.KnockBack(
                                                                thisHit.priority,
                                                                launch: true,
                                                                knockback: fgChar.rigidFG.rotation * new Vector3(0.0f, 1.0f, 1.0f) * 0.7071f);
                                                            break;
                                                    }
                                                    opponent.ChangeHealth(damage); // damage includes combo scaling.

                                                }, onBlock: (hitAtSameTimeByAttackPriority, damage) => {
                                                    audioService.PlayOneShot(hitLocations[defaultHitBox], audioMap["block"]);

                                                    particleService.PlayOneShot(fgChar.position + fgChar.rigidFG.rotation * new Vector3(0, 1.0f, 1.0f) * 1.0f, "block");

                                                    // I may put all of these functions into the same call.
                                                    opponent.ReceiveBlocked(thisHit.blockStun, false);
                                                    opponent.KnockBack(
                                                        thisHit.priority,
                                                        launch: false,
                                                        knockback: fgChar.rigidFG.rotation * Vector3.forward * 0.5f);
                                                    opponent.ChangeHealth(damage); // damage is hit.blockDamage
                                                });
                                            }
                                        });

                                        hit.HitBox(defaultHitBox);
                                    });
                                fl.To(16);
                                fl.From(20);
                                    fl.CounterHit(false);
                                    fl.ChainCancellable(true);
                                    fl.CancellableOnWhiff(true);
                                fl.To(30);
                            }));
                        atkBuilder.CleanUp(ReturnToStand);
                    });

                    Attack atkOrt5BBB = new Attack(atkBuilder => {
                        atkBuilder.Name("orthodox_5B,B,B");
                        atkBuilder.Requires(Orientation.ORTHODOX, GroundRelation.GROUNDED);
                        atkBuilder.Input("4|5", "B");
                        atkBuilder.AnimationState("5BBB");
                        atkBuilder.InitCharState((CharacterStates.Attack)fgChar.State("attackGrounded"));
                        atkBuilder.Movement(zMoveCb: animationCurveMap["orth_5BBB.z"].Evaluate);
                        atkBuilder.MovementDetails(additive:false);
                        atkBuilder.Frames(
                            FrameUtil.CreateList(fl => {
                                fl.SpecialCancellable(true);
                                fl.CancellableOnWhiff(false);
                                fl.CounterHit(true);
                                fl.From(2);
                                    fl.Execute(() => {
                                        audioService.PlayOneShot(fgChar.GetSpeakPosition(), audioMap["attacksound_01"]);
                                    });
                                fl.To(3);
                                fl.From(15);
                                    fl.Hit(hit => {
                                        hit.HitDamage(1000);
                                        hit.BlockDamage(0);
                                        hit.HitStun(30.0f);
                                        hit.BlockStun(12.0f);
                                        hit.ComboScaling(1.0f);
                                        hit.Priority(AttackPriority.LightAttack);

                                        HitBox defaultHitBox = hitBoxService.Create(hb => {
                                            hb.Relative(fgChar.transform);
                                            hb.Point0(new Vector3(-0.3f, 0.8f, 0.1f));
                                            hb.Point1(new Vector3(-0.3f, 1.0f, 1.3f));
                                            hb.Radius(0.25f);
                                            hb.InGameEntity(fgChar);
                                            hb.Validate((HitBox _, HitBox other) => false);
                                            hb.Validate((HitBox _, HurtBox other) => true);
                                        });

                                        hit.OnHit((thisHit, hitLocations, entity, hurtBoxes, hitBoxes) => {
                                            if (fgService.IsCurrentFGChar(entity) && entity != fgChar) {
                                                FightingGameCharacter opponent = (FightingGameCharacter)entity;

                                                // This exists to make characters hitting each other async
                                                fgService.Strike(entity, fgChar, thisHit, onHit: (hitAtSameTimeByAttackPriority, damage) => {
                                                    audioService.PlayOneShot(hitLocations[defaultHitBox], audioMap["hit"]);
                                                    PlayRandomGrunt(hitLocations[defaultHitBox]);

                                                    particleService.PlayOneShot(fgChar.position + fgChar.rigidFG.rotation * new Vector3(0, 1.0f, 1.0f) * 1.0f, "hit");

                                                    switch (opponent.GetGroundRelation()) {
                                                        case GroundRelation.GROUNDED:
                                                            opponent.ReceiveHit(thisHit.hitStun, false);
                                                            opponent.KnockBack(
                                                                thisHit.priority,
                                                                launch: false,
                                                                knockback: fgChar.rigidFG.rotation * Vector3.forward * 2.0f);
                                                            break;
                                                        case GroundRelation.AIRBORNE:
                                                            opponent.ReceiveBlocked(thisHit.hitStun, true);
                                                            opponent.KnockBack(
                                                                thisHit.priority,
                                                                launch: true,
                                                                knockback: fgChar.rigidFG.rotation * new Vector3(0.0f, 1.0f, 1.0f) * 0.7071f);
                                                            break;
                                                    }
                                                    opponent.ChangeHealth(damage); // damage includes combo scaling.

                                                }, onBlock: (hitAtSameTimeByAttackPriority, damage) => {
                                                    audioService.PlayOneShot(hitLocations[defaultHitBox], audioMap["block"]);

                                                    particleService.PlayOneShot(fgChar.position + fgChar.rigidFG.rotation * new Vector3(0, 1.0f, 1.0f) * 1.0f, "block");

                                                    // I may put all of these functions into the same call.
                                                    opponent.ReceiveBlocked(thisHit.blockStun, false);
                                                    opponent.KnockBack(
                                                        thisHit.priority,
                                                        launch: false,
                                                        knockback: fgChar.rigidFG.rotation * Vector3.forward * 0.5f);
                                                    opponent.ChangeHealth(damage); // damage is hit.blockDamage
                                                });
                                            }
                                        });

                                        hit.HitBox(defaultHitBox);
                                    });
                                fl.To(19);
                                fl.From(35);
                                    fl.CounterHit(false);
                                    fl.ChainCancellable(true);
                                    fl.CancellableOnWhiff(true);
                                fl.To(47);
                            }));
                        atkBuilder.CleanUp(ReturnToStand);
                    });

                    Attack atkOrt5BBBB = new Attack(atkBuilder => {
                        atkBuilder.Name("orthodox_5B,B,B,B");
                        atkBuilder.Requires(Orientation.ORTHODOX, GroundRelation.GROUNDED);
                        atkBuilder.Input("4|5", "B");
                        atkBuilder.AnimationState("5BBBB");
                        atkBuilder.InitCharState((CharacterStates.Attack)fgChar.State("attackGrounded"));
                        atkBuilder.Movement(zMoveCb: animationCurveMap["orth_5BBBB.z"].Evaluate);
                        atkBuilder.MovementDetails(additive:false);
                        atkBuilder.Frames(
                            FrameUtil.CreateList(fl => {
                                fl.SpecialCancellable(true);
                                fl.CancellableOnWhiff(false);
                                fl.CounterHit(true);
                                fl.From(2);
                                    fl.Execute(() => {
                                        audioService.PlayOneShot(fgChar.GetSpeakPosition(), audioMap["attacksound_02"]);
                                    });
                                fl.To(3);
                                fl.From(9);
                                    fl.Hit(hit => {
                                        hit.HitDamage(1000);
                                        hit.BlockDamage(0);
                                        hit.HitStun(30.0f);
                                        hit.BlockStun(12.0f);
                                        hit.ComboScaling(1.0f);
                                        hit.Priority(AttackPriority.LightAttack);

                                        HitBox defaultHitBox = hitBoxService.Create(hb => {
                                            hb.Relative(fgChar.transform);
                                            hb.Point0(new Vector3(0.3f, 0.8f, 0.1f));
                                            hb.Point1(new Vector3(0.3f, 1.0f, 1.3f));
                                            hb.Radius(0.25f);
                                            hb.InGameEntity(fgChar);
                                            hb.Validate((HitBox _, HitBox other) => false);
                                            hb.Validate((HitBox _, HurtBox other) => true);
                                        });

                                        hit.OnHit((thisHit, hitLocations, entity, hurtBoxes, hitBoxes) => {
                                            if (fgService.IsCurrentFGChar(entity) && entity != fgChar) {
                                                FightingGameCharacter opponent = (FightingGameCharacter)entity;

                                                // This exists to make characters hitting each other async
                                                fgService.Strike(entity, fgChar, thisHit, onHit: (hitAtSameTimeByAttackPriority, damage) => {
                                                    audioService.PlayOneShot(hitLocations[defaultHitBox], audioMap["hit"]);
                                                    PlayRandomGrunt(hitLocations[defaultHitBox]);

                                                    particleService.PlayOneShot(fgChar.position + fgChar.rigidFG.rotation * new Vector3(0, 1.0f, 1.0f) * 1.0f, "hit");

                                                    switch (opponent.GetGroundRelation()) {
                                                        case GroundRelation.GROUNDED:
                                                            opponent.ReceiveHit(thisHit.hitStun, false);
                                                            opponent.KnockBack(
                                                                thisHit.priority,
                                                                launch: false,
                                                                knockback: fgChar.rigidFG.rotation * Vector3.forward * 2.0f);
                                                            break;
                                                        case GroundRelation.AIRBORNE:
                                                            opponent.ReceiveBlocked(thisHit.hitStun, true);
                                                            opponent.KnockBack(
                                                                thisHit.priority,
                                                                launch: true,
                                                                knockback: fgChar.rigidFG.rotation * new Vector3(0.0f, 1.0f, 1.0f) * 0.7071f);
                                                            break;
                                                    }
                                                    opponent.ChangeHealth(damage); // damage includes combo scaling.

                                                }, onBlock: (hitAtSameTimeByAttackPriority, damage) => {
                                                    audioService.PlayOneShot(hitLocations[defaultHitBox], audioMap["block"]);

                                                    particleService.PlayOneShot(fgChar.position + fgChar.rigidFG.rotation * new Vector3(0, 1.0f, 1.0f) * 1.0f, "block");

                                                    // I may put all of these functions into the same call.
                                                    opponent.ReceiveBlocked(thisHit.blockStun, false);
                                                    opponent.KnockBack(
                                                        thisHit.priority,
                                                        launch: false,
                                                        knockback: fgChar.rigidFG.rotation * Vector3.forward * 0.5f);
                                                    opponent.ChangeHealth(damage); // damage is hit.blockDamage
                                                });
                                            }
                                        });

                                        hit.HitBox(defaultHitBox);
                                    });
                                fl.To(10);
                                fl.From(20);
                                    fl.CounterHit(false);
                                    fl.ChainCancellable(true);
                                    fl.CancellableOnWhiff(true);
                                fl.To(35);
                            }));
                        atkBuilder.CleanUp(ReturnToStand);
                    });

                    Attack atkOrt2B = new Attack(atkBuilder => {
                        atkBuilder.Name("orthodox_2B");
                        atkBuilder.Requires(Orientation.ORTHODOX, GroundRelation.GROUNDED);
                        atkBuilder.Input("1|2|3", "B");
                        atkBuilder.AnimationState("2B");
                        atkBuilder.InitCharState((CharacterStates.Attack)fgChar.State("attackGrounded"));
                        atkBuilder.Movement(zMoveCb: animationCurveMap["orth_2B.z"].Evaluate);
                        atkBuilder.MovementDetails(additive: false);
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

                                    hit.ValidBlock(BlockType.LOW);

                                    HitBox defaultHitBox = hitBoxService.Create(hb => {
                                        hb.Relative(fgChar.transform);
                                        hb.Point0(new Vector3(x: 0, y: 0.2f, z: 0.0f));
                                        hb.Point1(new Vector3(x: 0, y: 0.4f, z: 1.1f));
                                        hb.Radius(0.4f);
                                        hb.InGameEntity(fgChar);
                                        hb.Validate((HitBox _, HurtBox other) => true);
                                    });

                                    hit.OnHit((thisHit, hitLocations, entity, hurtBoxes, hitBoxes) => {
                                        if (fgService.IsCurrentFGChar(entity) && entity != fgChar) {
                                            FightingGameCharacter opponent = (FightingGameCharacter)entity;

                                            // This exists to make characters hitting each other async
                                            fgService.Strike(entity, fgChar, thisHit, onHit: (hitAtSameTimeByAttackPriority, damage) => {
                                                audioService.PlayOneShot(hitLocations[defaultHitBox], audioMap["hit"]);
                                                PlayRandomGrunt(hitLocations[defaultHitBox]);

                                                particleService.PlayOneShot(fgChar.position + fgChar.rigidFG.rotation * new Vector3(0, 1.0f, 1.0f) * 1.0f, "hit");

                                                switch (opponent.GetGroundRelation()) {
                                                    case GroundRelation.GROUNDED:
                                                        opponent.ReceiveHit(thisHit.hitStun, false);
                                                        opponent.KnockBack(
                                                            thisHit.priority,
                                                            launch: false,
                                                            knockback: fgChar.rigidFG.rotation * Vector3.forward * 2.0f);
                                                        break;
                                                    case GroundRelation.AIRBORNE:
                                                        opponent.ReceiveBlocked(thisHit.hitStun, true);
                                                        opponent.KnockBack(
                                                            thisHit.priority,
                                                            launch: true,
                                                            knockback: fgChar.rigidFG.rotation * new Vector3(0.0f, 1.0f, 1.0f) * 0.7071f);
                                                        break;
                                                }
                                                opponent.ChangeHealth(damage); // damage includes combo scaling.

                                            }, onBlock: (hitAtSameTimeByAttackPriority, damage) => {
                                                audioService.PlayOneShot(hitLocations[defaultHitBox], audioMap["block"]);

                                                particleService.PlayOneShot(fgChar.position + fgChar.rigidFG.rotation * new Vector3(0, 1.0f, 1.0f) * 1.0f, "block");

                                                // I may put all of these functions into the same call.
                                                opponent.ReceiveBlocked(thisHit.blockStun, false);
                                                opponent.KnockBack(
                                                    thisHit.priority,
                                                    launch: false,
                                                    knockback: fgChar.rigidFG.rotation * Vector3.forward * 0.5f);
                                                opponent.ChangeHealth(damage); // damage is hit.blockDamage
                                            });
                                        }
                                    });

                                    hit.HitBox(defaultHitBox);
                                });
                                fl.To(17);
                                fl.From(20);
                                fl.CounterHit(false);
                                fl.ChainCancellable(true);
                                fl.CancellableOnWhiff(true);
                                fl.To(50);
                            }));
                        atkBuilder.CleanUp(ReturnToStand);
                    });

                    Attack atkOrt2BB = new Attack(atkBuilder => {
                        atkBuilder.Name("orthodox_2B,B");
                        atkBuilder.Requires(Orientation.ORTHODOX, GroundRelation.GROUNDED);
                        atkBuilder.Input("1|2|3", "B");
                        atkBuilder.AnimationState("2BB");
                        atkBuilder.InitCharState((CharacterStates.Attack)fgChar.State("attackGrounded"));
                        atkBuilder.Movement(zMoveCb: animationCurveMap["orth_2BB.z"].Evaluate);
                        atkBuilder.MovementDetails(additive: false);
                        atkBuilder.Frames(
                            FrameUtil.CreateList(fl => {
                                fl.SpecialCancellable(true);
                                fl.CancellableOnWhiff(false);
                                fl.CounterHit(true);
                                fl.From(1);
                                fl.ChainCancellable(false);
                                fl.From(2);
                                fl.Execute(() => {
                                    audioService.PlayOneShot(fgChar.GetSpeakPosition(), audioMap["attacksound_01"]);
                                });
                                fl.To(3);
                                fl.From(13);
                                fl.Hit(hit => {
                                    hit.HitDamage(1000);
                                    hit.BlockDamage(0);
                                    hit.HitStun(32.0f);
                                    hit.BlockStun(12.0f);
                                    hit.ComboScaling(1.0f);
                                    hit.Priority(AttackPriority.LightAttack);

                                    HitBox defaultHitBox = hitBoxService.Create(hb => {
                                        hb.Relative(fgChar.transform);
                                        hb.Point0(new Vector3(x: -0.2f, y: 0, z: 0.2f));
                                        hb.Point1(new Vector3(x: -0.15f, y: 0.8f, z: 0.5f));
                                        hb.Radius(0.3f);
                                        hb.InGameEntity(fgChar);
                                        hb.Validate((HitBox _, HurtBox other) => true);
                                    });

                                    hit.OnHit((thisHit, hitLocations, entity, hurtBoxes, hitBoxes) => {
                                        if (fgService.IsCurrentFGChar(entity) && entity != fgChar) {
                                            FightingGameCharacter opponent = (FightingGameCharacter)entity;

                                            // This exists to make characters hitting each other async
                                            fgService.Strike(entity, fgChar, thisHit, onHit: (hitAtSameTimeByAttackPriority, damage) => {
                                                audioService.PlayOneShot(hitLocations[defaultHitBox], audioMap["hit"]);
                                                PlayRandomGrunt(hitLocations[defaultHitBox]);

                                                particleService.PlayOneShot(fgChar.position + fgChar.rigidFG.rotation * new Vector3(0, 1.0f, 1.0f) * 1.0f, "hit");

                                                switch (opponent.GetGroundRelation()) {
                                                    case GroundRelation.GROUNDED:
                                                        opponent.ReceiveHit(thisHit.hitStun, false);
                                                        opponent.KnockBack(
                                                            thisHit.priority,
                                                            launch: false,
                                                            knockback: fgChar.rigidFG.rotation * Vector3.forward * 2.0f);
                                                        break;
                                                    case GroundRelation.AIRBORNE:
                                                        opponent.ReceiveBlocked(thisHit.hitStun, true);
                                                        opponent.KnockBack(
                                                            thisHit.priority,
                                                            launch: true,
                                                            knockback: fgChar.rigidFG.rotation * new Vector3(0.0f, 1.0f, 1.0f) * 0.7071f);
                                                        break;
                                                }
                                                opponent.ChangeHealth(damage); // damage includes combo scaling.

                                            }, onBlock: (hitAtSameTimeByAttackPriority, damage) => {
                                                audioService.PlayOneShot(hitLocations[defaultHitBox], audioMap["block"]);

                                                particleService.PlayOneShot(fgChar.position + fgChar.rigidFG.rotation * new Vector3(0, 1.0f, 1.0f) * 1.0f, "block");

                                                // I may put all of these functions into the same call.
                                                opponent.ReceiveBlocked(thisHit.blockStun, false);
                                                opponent.KnockBack(
                                                    thisHit.priority,
                                                    launch: false,
                                                    knockback: fgChar.rigidFG.rotation * Vector3.forward * 0.5f);
                                                opponent.ChangeHealth(damage); // damage is hit.blockDamage
                                            });
                                        }
                                    });

                                    hit.HitBox(defaultHitBox);
                                });
                                fl.To(17);
                                fl.From(20);
                                fl.CounterHit(false);
                                fl.ChainCancellable(true);
                                fl.CancellableOnWhiff(true);
                                fl.To(40);
                            }));
                        atkBuilder.CleanUp(ReturnToStand);
                    });

                    Attack atkOrt2BBB = new Attack(atkBuilder => {
                        atkBuilder.Name("orthodox_2B,B,B");
                        atkBuilder.Requires(Orientation.ORTHODOX, GroundRelation.GROUNDED);
                        atkBuilder.Input("1|2|3", "B");
                        atkBuilder.AnimationState("2BBB");
                        atkBuilder.InitCharState((CharacterStates.Attack)fgChar.State("attackGrounded"));
                        atkBuilder.Movement(zMoveCb: animationCurveMap["orth_2BBB.z"].Evaluate);
                        atkBuilder.MovementDetails(additive: false);
                        atkBuilder.Frames(
                            FrameUtil.CreateList(fl => {
                                fl.SpecialCancellable(true);
                                fl.CancellableOnWhiff(false);
                                fl.CounterHit(true);
                                fl.From(1);
                                fl.ChainCancellable(false);
                                fl.From(2);
                                fl.Execute(() => {
                                    audioService.PlayOneShot(fgChar.GetSpeakPosition(), audioMap["attacksound_03"]);
                                });
                                fl.To(3);
                                fl.From(18);
                                fl.Hit(hit => {
                                    hit.HitDamage(1000);
                                    hit.BlockDamage(0);
                                    hit.HitStun(20.0f);
                                    hit.BlockStun(50.0f);
                                    hit.ComboScaling(1.0f);
                                    hit.Priority(AttackPriority.LightAttack);

                                    hit.ValidBlock(BlockType.HIGH);

                                    HitBox defaultHitBox = hitBoxService.Create(hb => {
                                        hb.Relative(fgChar.transform);
                                        hb.Point0(new Vector3(x: 0.2f, y: 0, z: 0.5f));
                                        hb.Point1(new Vector3(x: 0.2f, y: 0.8f, z: 1.0f));
                                        hb.Radius(0.3f);
                                        hb.InGameEntity(fgChar);
                                        hb.Validate((HitBox _, HurtBox other) => true);
                                    });

                                    hit.OnHit((thisHit, hitLocations, entity, hurtBoxes, hitBoxes) => {
                                        if (fgService.IsCurrentFGChar(entity) && entity != fgChar) {
                                            FightingGameCharacter opponent = (FightingGameCharacter)entity;

                                            // This exists to make characters hitting each other async
                                            fgService.Strike(entity, fgChar, thisHit, onHit: (hitAtSameTimeByAttackPriority, damage) => {
                                                audioService.PlayOneShot(hitLocations[defaultHitBox], audioMap["hit"]);
                                                PlayRandomGrunt(hitLocations[defaultHitBox]);

                                                particleService.PlayOneShot(fgChar.position + fgChar.rigidFG.rotation * new Vector3(0, 1.0f, 1.0f) * 1.0f, "hit");

                                                switch (opponent.GetGroundRelation()) {
                                                    case GroundRelation.GROUNDED:
                                                        opponent.ReceiveHit(thisHit.hitStun, false);
                                                        opponent.KnockBack(
                                                            thisHit.priority,
                                                            launch: false,
                                                            knockback: fgChar.rigidFG.rotation * Vector3.forward * 2.0f);
                                                        break;
                                                    case GroundRelation.AIRBORNE:
                                                        opponent.ReceiveBlocked(thisHit.hitStun, true);
                                                        opponent.KnockBack(
                                                            thisHit.priority,
                                                            launch: true,
                                                            knockback: fgChar.rigidFG.rotation * new Vector3(0.0f, 1.0f, 1.0f) * 0.7071f);
                                                        break;
                                                }
                                                opponent.ChangeHealth(damage); // damage includes combo scaling.

                                            }, onBlock: (hitAtSameTimeByAttackPriority, damage) => {
                                                audioService.PlayOneShot(hitLocations[defaultHitBox], audioMap["block"]);

                                                particleService.PlayOneShot(fgChar.position + fgChar.rigidFG.rotation * new Vector3(0, 1.0f, 1.0f) * 1.0f, "block");

                                                // I may put all of these functions into the same call.
                                                opponent.ReceiveBlocked(thisHit.blockStun, false);
                                                opponent.KnockBack(
                                                    thisHit.priority,
                                                    launch: false,
                                                    knockback: fgChar.rigidFG.rotation * Vector3.forward * 0.5f);
                                                opponent.ChangeHealth(damage); // damage is hit.blockDamage
                                            });
                                        }
                                    });

                                    hit.HitBox(defaultHitBox);
                                });
                                fl.To(20);
                                fl.From(21);
                                fl.CounterHit(false);
                                fl.ChainCancellable(true);
                                fl.CancellableOnWhiff(true);
                                fl.To(50);
                            }));
                        atkBuilder.CleanUp(ReturnToStand);
                    });

                    Attack atkJump5A = new Attack(atkBuilder => {
                        atkBuilder.Name("jump_5A");
                        atkBuilder.Requires(Orientation.ORTHODOX, GroundRelation.AIRBORNE);
                        atkBuilder.Input("A");
                        atkBuilder.AnimationState("j_A");
                        atkBuilder.InitCharState((CharacterStates.Attack)fgChar.State("attackAirborne"));
                        atkBuilder.Movement();
                        atkBuilder.MovementDetails(additive:true);
                        atkBuilder.FramesContinuous((localFrame, target) => {
                            if (localFrame >= 8.0f && localFrame <= 11.0f) {
                                float p = Mathf.InverseLerp(8.0f, 11.0f, localFrame);
                                fgChar.SetStandCollider(p * 0.5f * Vector3.up);
                            }
                            else if (localFrame > 11.0f) {
                                fgChar.SetStandCollider(0.5f * Vector3.up);
                            }
                        });
                        atkBuilder.Frames(
                            FrameUtil.CreateList(f => { f
                                .SpecialCancellable(true)
                                .ChainCancellable(true)
                                .CancellableOnWhiff(false)
                                .CounterHit(true)
                                .From(8)
                                    .Hit(hit => {
                                        hit.HitDamage(700)
                                            .BlockDamage(0)
                                            .HitStun(22.0f)
                                            .BlockStun(30.0f);

                                        hit.ValidBlock(BlockType.LOW, BlockType.HIGH);

                                        HitBox defaultHitBox = hitBoxService.Create(hb => {
                                            hb.Relative(fgChar.transform);
                                            hb.Point0(new Vector3(0, 0.2f, 0.5f));
                                            hb.Point1(new Vector3(0, 0.8f, 0.5f));
                                            hb.Radius(0.25f);
                                            hb.InGameEntity(fgChar);
                                            hb.Validate((HitBox _, HitBox other) => false);
                                            hb.Validate((HitBox _, HurtBox other) => true);
                                        });

                                        hit.OnHit((thisHit, hitLocations, entity, hurtBoxes, hitBoxes) => {
                                            if (fgService.IsCurrentFGChar(entity) && entity != fgChar) {
                                                FightingGameCharacter opponent = (FightingGameCharacter)entity;

                                                // This exists to make characters hitting each other async
                                                fgService.Strike(entity, fgChar, thisHit, onHit: (hitAtSameTimeByAttackPriority, damage) => {
                                                    audioService.PlayOneShot(hitLocations[defaultHitBox], audioMap["hit"]);
                                                    PlayRandomGrunt(hitLocations[defaultHitBox]);

                                                    particleService.PlayOneShot(fgChar.position + fgChar.rigidFG.rotation * new Vector3(0, 1.0f, 1.0f) * 1.0f, "hit");

                                                    switch (opponent.GetGroundRelation()) {
                                                        case GroundRelation.GROUNDED:
                                                            opponent.ReceiveHit(thisHit.hitStun, false);
                                                            opponent.KnockBack(
                                                                thisHit.priority,
                                                                launch: false,
                                                                knockback: fgChar.rigidFG.rotation * Vector3.forward * 2.0f);
                                                            break;
                                                        case GroundRelation.AIRBORNE:
                                                            opponent.ReceiveHit(thisHit.hitStun, true);
                                                            opponent.KnockBack(
                                                                thisHit.priority,
                                                                launch: true,
                                                                knockback: fgChar.rigidFG.rotation * new Vector3(0.0f, 1.0f, 1.0f) * 0.7071f);
                                                            break;
                                                    }
                                                    opponent.ChangeHealth(damage); // damage includes combo scaling.

                                                }, onBlock: (hitAtSameTimeByAttackPriority, damage) => {
                                                    audioService.PlayOneShot(hitLocations[defaultHitBox], audioMap["block"]);

                                                    particleService.PlayOneShot(fgChar.position + fgChar.rigidFG.rotation * new Vector3(0, 1.0f, 1.0f) * 1.0f, "block");

                                                    // I may put all of these functions into the same call.
                                                    opponent.ReceiveBlocked(thisHit.blockStun, false);
                                                    opponent.KnockBack(
                                                        thisHit.priority,
                                                        launch: false,
                                                        knockback: fgChar.rigidFG.rotation * Vector3.forward * 0.5f);
                                                    opponent.ChangeHealth(damage); // damage is hit.blockDamage
                                                });
                                            }
                                        });

                                        hit.HitBox(defaultHitBox);
                                    })
                                .To(11)
                                .From(13)
                                    .CounterHit(false)
                                    .ChainCancellable(true)
                                    .CancellableOnWhiff(true)
                                .To(23);
                            }));
                        atkBuilder.CleanUp(ReturnToAirborne);
                    });

                    Attack atkJump5AA = new Attack(atkBuilder => {
                        atkBuilder.Name("jump_5A,A");
                        atkBuilder.Requires(Orientation.ORTHODOX, GroundRelation.AIRBORNE);
                        atkBuilder.Input("A");
                        atkBuilder.AnimationState("j_AA");
                        atkBuilder.InitCharState((CharacterStates.Attack)fgChar.State("attackAirborne"));
                        atkBuilder.Movement();
                        atkBuilder.MovementDetails(additive:true);
                        atkBuilder.FramesContinuous((localFrame, target) => {
                            if (localFrame >= 8.0f && localFrame <= 11.0f) {
                                float p = Mathf.InverseLerp(8.0f, 11.0f, localFrame);
                                fgChar.SetStandCollider(p * 0.5f * Vector3.up);
                            }
                            else if (localFrame > 11.0f) {
                                fgChar.SetStandCollider(0.5f * Vector3.up);
                            }
                        });
                        atkBuilder.Frames(
                            FrameUtil.CreateList(f => { f
                                .SpecialCancellable(true)
                                .ChainCancellable(false)
                                .CancellableOnWhiff(false)
                                .CounterHit(true)
                                .From(2)
                                .Execute(() => {
                                    audioService.PlayOneShot(fgChar.GetSpeakPosition(), audioMap["attacksound_02"]);
                                })
                                .To(3)
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
                                            .HitStun(20.0f)
                                            .BlockStun(12.0f);

                                        hit.ValidBlock(BlockType.HIGH);

                                        HitBox defaultHitBox = hitBoxService.Create(hb => {
                                            hb.Relative(fgChar.transform);
                                            hb.Point0(new Vector3(0, 0, 1.0f));
                                            hb.Point1(new Vector3(0, 0.8f, 1.3f));
                                            hb.Radius(0.25f);
                                            hb.InGameEntity(fgChar);
                                            hb.Validate((HitBox _, HitBox other) => false);
                                            hb.Validate((HitBox _, HurtBox other) => true);
                                        });

                                        hit.OnHit((thisHit, hitLocations, entity, hurtBoxes, hitBoxes) => {
                                            if (fgService.IsCurrentFGChar(entity) && entity != fgChar) {
                                                FightingGameCharacter opponent = (FightingGameCharacter)entity;

                                                // This exists to make characters hitting each other async
                                                fgService.Strike(entity, fgChar, thisHit, onHit: (hitAtSameTimeByAttackPriority, damage) => {
                                                    audioService.PlayOneShot(hitLocations[defaultHitBox], audioMap["hit"]);
                                                    PlayRandomGrunt(hitLocations[defaultHitBox]);

                                                    particleService.PlayOneShot(fgChar.position + fgChar.rigidFG.rotation * new Vector3(0, 1.0f, 1.0f) * 1.0f, "hit");

                                                    switch (opponent.GetGroundRelation()) {
                                                        case GroundRelation.GROUNDED:
                                                            opponent.ReceiveHit(thisHit.hitStun, false);
                                                            opponent.KnockBack(
                                                                thisHit.priority,
                                                                launch: false,
                                                                knockback: fgChar.rigidFG.rotation * Vector3.forward * 2.0f);
                                                            break;
                                                        case GroundRelation.AIRBORNE:
                                                            opponent.ReceiveBlocked(thisHit.hitStun, true);
                                                            opponent.KnockBack(
                                                                thisHit.priority,
                                                                launch: true,
                                                                knockback: fgChar.rigidFG.rotation * new Vector3(0.0f, 1.0f, 1.0f) * 0.7071f);
                                                            break;
                                                    }
                                                    opponent.ChangeHealth(damage); // damage includes combo scaling.

                                                }, onBlock: (hitAtSameTimeByAttackPriority, damage) => {
                                                    audioService.PlayOneShot(hitLocations[defaultHitBox], audioMap["block"]);

                                                    particleService.PlayOneShot(fgChar.position + fgChar.rigidFG.rotation * new Vector3(0, 1.0f, 1.0f) * 1.0f, "block");

                                                    // I may put all of these functions into the same call.
                                                    opponent.ReceiveBlocked(thisHit.blockStun, false);
                                                    opponent.KnockBack(
                                                        thisHit.priority,
                                                        launch: false,
                                                        knockback: fgChar.rigidFG.rotation * Vector3.forward * 0.5f);
                                                    opponent.ChangeHealth(damage); // damage is hit.blockDamage
                                                });
                                            }
                                        });

                                        hit.HitBox(defaultHitBox);
                                    })
                                .To(13)
                                .From(16)
                                    .CounterHit(false)
                                    .ChainCancellable(true)
                                    .CancellableOnWhiff(true)
                                .To(30);
                            }));
                        atkBuilder.CleanUp(ReturnToAirborne);
                    });

                    Attack atkJump5AAA = new Attack(atkBuilder => {
                        atkBuilder.Name("jump_5A,A,A");
                        atkBuilder.Requires(Orientation.ORTHODOX, GroundRelation.AIRBORNE);
                        atkBuilder.Input("A");
                        atkBuilder.AnimationState("j_AAA");
                        atkBuilder.InitCharState((CharacterStates.Attack)fgChar.State("attackAirborne"));
                        atkBuilder.Movement(yMoveCb: animationCurveMap["jump_5AAA.y"].Evaluate, zMoveCb: animationCurveMap["jump_5AAA.z"].Evaluate);
                        atkBuilder.MovementDetails(additive:false);
                        atkBuilder.FramesContinuous((localFrame, target) => {
                            if (localFrame >= 8.0f && localFrame <= 11.0f) {
                                float p = Mathf.InverseLerp(8.0f, 11.0f, localFrame);
                                fgChar.SetStandCollider(p * 0.5f * Vector3.up);
                            }
                            else if (localFrame > 11.0f) {
                                fgChar.SetStandCollider(0.5f * Vector3.up);
                            }
                        });
                        atkBuilder.Frames(
                            FrameUtil.CreateList(f => { f
                                .SpecialCancellable(true)
                                .ChainCancellable(false)
                                .CancellableOnWhiff(false)
                                .CounterHit(true)
                                .From(2)
                                .Execute(() => {
                                    audioService.PlayOneShot(fgChar.GetSpeakPosition(), audioMap["attacksound_04"]);
                                })
                                .To(3)
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

                                        hit.ValidBlock(BlockType.HIGH);

                                        HitBox defaultHitBox = hitBoxService.Create(hb => {
                                            hb.Relative(fgChar.transform);
                                            hb.Point0(new Vector3(0, 0, 1.0f));
                                            hb.Point1(new Vector3(0, 0.8f, 1.3f));
                                            hb.Radius(0.25f);
                                            hb.InGameEntity(fgChar);
                                            hb.Validate((HitBox _, HitBox other) => true);
                                            hb.Validate((HitBox _, HurtBox other) => true);
                                        });

                                        hit.OnHit((thisHit, hitLocations, entity, hurtBoxes, hitBoxes) => {
                                            if (fgService.IsCurrentFGChar(entity) && entity != fgChar) {
                                                FightingGameCharacter opponent = (FightingGameCharacter)entity;

                                                // This exists to make characters hitting each other async
                                                fgService.Strike(entity, fgChar, thisHit, onHit: (hitAtSameTimeByAttackPriority, damage) => {
                                                    audioService.PlayOneShot(hitLocations[defaultHitBox], audioMap["hit"]);
                                                    PlayRandomGrunt(hitLocations[defaultHitBox]);

                                                    particleService.PlayOneShot(fgChar.position + fgChar.rigidFG.rotation * new Vector3(0, 1.0f, 1.0f) * 1.0f, "hit");

                                                    switch (opponent.GetGroundRelation()) {
                                                        case GroundRelation.GROUNDED:
                                                            opponent.ReceiveHit(thisHit.hitStun, false);
                                                            opponent.KnockBack(
                                                                thisHit.priority,
                                                                launch: false,
                                                                knockback: fgChar.rigidFG.rotation * Vector3.forward * 2.0f);
                                                            break;
                                                        case GroundRelation.AIRBORNE:
                                                            opponent.ReceiveBlocked(thisHit.hitStun, true);
                                                            opponent.KnockBack(
                                                                thisHit.priority,
                                                                launch: true,
                                                                knockback: fgChar.rigidFG.rotation * new Vector3(0.0f, 1.0f, 1.0f) * 0.7071f);
                                                            break;
                                                    }
                                                    opponent.ChangeHealth(damage); // damage includes combo scaling.

                                                }, onBlock: (hitAtSameTimeByAttackPriority, damage) => {
                                                    audioService.PlayOneShot(hitLocations[defaultHitBox], audioMap["block"]);

                                                    particleService.PlayOneShot(fgChar.position + fgChar.rigidFG.rotation * new Vector3(0, 1.0f, 1.0f) * 1.0f, "block");

                                                    // I may put all of these functions into the same call.
                                                    opponent.ReceiveBlocked(thisHit.blockStun, false);
                                                    opponent.KnockBack(
                                                        thisHit.priority,
                                                        launch: false,
                                                        knockback: fgChar.rigidFG.rotation * Vector3.forward * 0.5f);
                                                    opponent.ChangeHealth(damage); // damage is hit.blockDamage
                                                });
                                            }
                                        });

                                        hit.HitBox(defaultHitBox);
                                    })
                                .To(21)
                                    .CounterHit(false)
                                .To(40);
                            }));
                        atkBuilder.CleanUp(ReturnToAirborne);
                    });

                    Attack atkJump5B = new Attack(atkBuilder => {
                        atkBuilder.Name("jump_5B");
                        atkBuilder.Requires(Orientation.ORTHODOX, GroundRelation.AIRBORNE);
                        atkBuilder.Input("B");
                        atkBuilder.AnimationState("j_B");
                        atkBuilder.InitCharState((CharacterStates.Attack)fgChar.State("attackAirborne"));
                        atkBuilder.Movement();
                        atkBuilder.MovementDetails(additive:true);
                        atkBuilder.FramesContinuous((localFrame, target) => {
                            if (localFrame >= 8.0f && localFrame <= 25.0f) {
                                float p = Mathf.InverseLerp(8.0f, 25.0f, localFrame);
                                fgChar.SetStandCollider(p * 0.2f * Vector3.up);
                            }
                            else if (localFrame > 25.0f) {
                                fgChar.SetStandCollider(0.2f * Vector3.up);
                            }
                        });
                        atkBuilder.Frames(
                            FrameUtil.CreateList(f => { f
                                .SpecialCancellable(true)
                                .ChainCancellable(true)
                                .CancellableOnWhiff(false)
                                .CounterHit(true)
                                .From(2)
                                .Execute(() => {
                                    audioService.PlayOneShot(fgChar.GetSpeakPosition(), audioMap["attacksound_02"]);
                                })
                                .To(3)
                                .From(15)
                                    .Hit(hit => {
                                        hit.HitDamage(700)
                                            .BlockDamage(0)
                                            .HitStun(22.0f)
                                            .BlockStun(30.0f);

                                        hit.ValidBlock(BlockType.HIGH);

                                        HitBox defaultHitBox = hitBoxService.Create(hb => {
                                            hb.Relative(fgChar.transform);
                                            hb.Point0(new Vector3(0, 0.2f, 0.5f));
                                            hb.Point1(new Vector3(0, 0.8f, 0.5f));
                                            hb.Radius(0.25f);
                                            hb.InGameEntity(fgChar);
                                            hb.Validate((HitBox _, HitBox other) => false);
                                            hb.Validate((HitBox _, HurtBox other) => true);
                                        });

                                        hit.OnHit((thisHit, hitLocations, entity, hurtBoxes, hitBoxes) => {
                                            if (fgService.IsCurrentFGChar(entity) && entity != fgChar) {
                                                FightingGameCharacter opponent = (FightingGameCharacter)entity;

                                                // This exists to make characters hitting each other async
                                                fgService.Strike(entity, fgChar, thisHit, onHit: (hitAtSameTimeByAttackPriority, damage) => {
                                                    audioService.PlayOneShot(hitLocations[defaultHitBox], audioMap["hit"]);
                                                    PlayRandomGrunt(hitLocations[defaultHitBox]);

                                                    particleService.PlayOneShot(fgChar.position + fgChar.rigidFG.rotation * new Vector3(0, 1.0f, 1.0f) * 1.0f, "hit");

                                                    switch (opponent.GetGroundRelation()) {
                                                        case GroundRelation.GROUNDED:
                                                            opponent.ReceiveHit(thisHit.hitStun, false);
                                                            opponent.KnockBack(
                                                                thisHit.priority,
                                                                launch: false,
                                                                knockback: fgChar.rigidFG.rotation * Vector3.forward * 2.0f);
                                                            break;
                                                        case GroundRelation.AIRBORNE:
                                                            opponent.ReceiveHit(thisHit.hitStun, true);
                                                            opponent.KnockBack(
                                                                thisHit.priority,
                                                                launch: true,
                                                                knockback: fgChar.rigidFG.rotation * new Vector3(0.0f, 1.0f, 1.0f) * 0.7071f);
                                                            break;
                                                    }
                                                    opponent.ChangeHealth(damage); // damage includes combo scaling.

                                                }, onBlock: (hitAtSameTimeByAttackPriority, damage) => {
                                                    audioService.PlayOneShot(hitLocations[defaultHitBox], audioMap["block"]);

                                                    particleService.PlayOneShot(fgChar.position + fgChar.rigidFG.rotation * new Vector3(0, 1.0f, 1.0f) * 1.0f, "block");

                                                    // I may put all of these functions into the same call.
                                                    opponent.ReceiveBlocked(thisHit.blockStun, false);
                                                    opponent.KnockBack(
                                                        thisHit.priority,
                                                        launch: false,
                                                        knockback: fgChar.rigidFG.rotation * Vector3.forward * 0.5f);
                                                    opponent.ChangeHealth(damage); // damage is hit.blockDamage
                                                });
                                            }
                                        });

                                        hit.HitBox(defaultHitBox);
                                        hit.HitBox(hitBoxService.Create(hb => {
                                            hb.Relative(fgChar.transform);
                                            hb.Point0(new Vector3(x: 1.0f, y: 0.5f, z: 0.2f));
                                            hb.Point1(new Vector3(x: 0.8f, y: 0.5f, z: 0.6f));
                                            hb.Radius(0.3f);
                                            hb.InGameEntity(fgChar);
                                        }));
                                        hit.HitBox(hitBoxService.Create(hb => {
                                            hb.Relative(fgChar.transform);
                                            hb.Point0(new Vector3(x: 0.8f, y: 0.5f, z: 0.6f));
                                            hb.Point1(new Vector3(x: 0.0f, y: 0.5f, z: 1.0f));
                                            hb.Radius(0.3f);
                                            hb.InGameEntity(fgChar);
                                        }));
                                        hit.HitBox(hitBoxService.Create(hb => {
                                            hb.Relative(fgChar.transform);
                                            hb.Point0(new Vector3(x: 0.0f, y: 0.5f, z: 1.0f));
                                            hb.Point1(new Vector3(x: -0.8f, y: 0.5f, z: 0.6f));
                                            hb.Radius(0.3f);
                                            hb.InGameEntity(fgChar);
                                        }));
                                        hit.HitBox(hitBoxService.Create(hb => {
                                            hb.Relative(fgChar.transform);
                                            hb.Point0(new Vector3(x: -0.8f, y: 0.5f, z: 0.6f));
                                            hb.Point1(new Vector3(x: -1.0f, y: 0.5f, z: 0.2f));
                                            hb.Radius(0.3f);
                                            hb.InGameEntity(fgChar);
                                        }));
                                    })
                                .To(19)
                                .From(20)
                                    .CounterHit(false)
                                    .ChainCancellable(true)
                                    .CancellableOnWhiff(true)
                                .To(40);
                            }));
                        atkBuilder.CleanUp(ReturnToAirborne);
                    });

                    Attack atkJump5BB = new Attack(atkBuilder => {
                        atkBuilder.Name("jump_5B,B");
                        atkBuilder.Requires(Orientation.ORTHODOX, GroundRelation.AIRBORNE);
                        atkBuilder.Input("B");
                        atkBuilder.AnimationState("j_BB");
                        atkBuilder.InitCharState((CharacterStates.Attack)fgChar.State("attackAirborne"));
                        atkBuilder.Movement(yMoveCb: animationCurveMap["jump_5BB.y"].Evaluate);
                        atkBuilder.MovementDetails(additive:true);
                        atkBuilder.FramesContinuous((localFrame, target) => {
                            if (localFrame >= 12.0f && localFrame <= 19.0f) {
                                float p = Mathf.InverseLerp(12.0f, 19.0f, localFrame);
                                fgChar.SetStandCollider(p * 0.2f * Vector3.up);
                            }
                            else if (localFrame > 19.0f) {
                                fgChar.SetStandCollider(0.2f * Vector3.up);
                            }
                        });
                        atkBuilder.Frames(
                            FrameUtil.CreateList(f => { f
                                .SpecialCancellable(true)
                                .ChainCancellable(false)
                                .CancellableOnWhiff(false)
                                .CounterHit(true)
                                .From(2)
                                .Execute(() => {
                                    audioService.PlayOneShot(fgChar.GetSpeakPosition(), audioMap["attacksound_05"]);
                                })
                                .To(3)
                                .From(4)
                                    .Track((targetFG) => {
                                        Vector3 newTargetPos = TargetUtil.MoveTargetLimited(fgChar.position, targetFG.targetPos, targetFG.ActualTargetPos(), 20.0f);
                                            // for now, just let the setting of this value be instantaneous once per frame.
                                        fgChar.SetTarget(newTargetPos);
                                    })
                                .To(12)
                                .From(17)
                                    .Hit(hit => {
                                        hit.HitDamage(700)
                                            .BlockDamage(0)
                                            .HitStun(35.0f)
                                            .BlockStun(35.0f);

                                        hit.ValidBlock(BlockType.HIGH);

                                        HitBox defaultHitBox = hitBoxService.Create(hb => {
                                            hb.Relative(fgChar.transform);
                                            hb.Point0(new Vector3(0, -0.5f, 1.0f));
                                            hb.Point1(new Vector3(0, 0.8f, 0.0f));
                                            hb.Radius(0.25f);
                                            hb.InGameEntity(fgChar);
                                        });

                                        hit.OnHit((thisHit, hitLocations, entity, hurtBoxes, hitBoxes) => {
                                            if (fgService.IsCurrentFGChar(entity) && entity != fgChar) {
                                                FightingGameCharacter opponent = (FightingGameCharacter)entity;

                                                // This exists to make characters hitting each other async
                                                fgService.Strike(entity, fgChar, thisHit, onHit: (hitAtSameTimeByAttackPriority, damage) => {
                                                    audioService.PlayOneShot(hitLocations[defaultHitBox], audioMap["hit"]);
                                                    PlayRandomGrunt(hitLocations[defaultHitBox]);

                                                    particleService.PlayOneShot(fgChar.position + fgChar.rigidFG.rotation * new Vector3(0, 1.0f, 1.0f) * 1.0f, "hit");

                                                    switch (opponent.GetGroundRelation()) {
                                                        case GroundRelation.GROUNDED:
                                                            opponent.ReceiveHit(thisHit.hitStun, false);
                                                            opponent.KnockBack(
                                                                thisHit.priority,
                                                                launch: false,
                                                                knockback: fgChar.rigidFG.rotation * Vector3.forward * 2.0f);
                                                            break;
                                                        case GroundRelation.AIRBORNE:
                                                            opponent.ReceiveBlocked(thisHit.hitStun, true);
                                                            opponent.KnockBack(
                                                                thisHit.priority,
                                                                launch: true,
                                                                knockback: fgChar.rigidFG.rotation * new Vector3(0.0f, 1.0f, 1.0f) * 0.7071f);
                                                            break;
                                                    }
                                                    opponent.ChangeHealth(damage); // damage includes combo scaling.

                                                }, onBlock: (hitAtSameTimeByAttackPriority, damage) => {
                                                    audioService.PlayOneShot(hitLocations[defaultHitBox], audioMap["block"]);

                                                    particleService.PlayOneShot(fgChar.position + fgChar.rigidFG.rotation * new Vector3(0, 1.0f, 1.0f) * 1.0f, "block");

                                                    // I may put all of these functions into the same call.
                                                    opponent.ReceiveBlocked(thisHit.blockStun, false);
                                                    opponent.KnockBack(
                                                        thisHit.priority,
                                                        launch: false,
                                                        knockback: fgChar.rigidFG.rotation * Vector3.forward * 0.5f);
                                                    opponent.ChangeHealth(damage); // damage is hit.blockDamage
                                                });
                                            }
                                        });

                                        hit.HitBox(defaultHitBox);
                                    })
                                .To(19)
                                .From(19)
                                    .CounterHit(false)
                                    .ChainCancellable(true)
                                    .CancellableOnWhiff(true)
                                .To(40);
                            }));
                        atkBuilder.CleanUp(ReturnToAirborne);
                    });

                    Attack atkJump5C = new Attack(atkBuilder => {
                        atkBuilder.Name("jump_5C");
                        atkBuilder.Requires(Orientation.ORTHODOX, GroundRelation.AIRBORNE);
                        atkBuilder.Input("C");
                        atkBuilder.AnimationState("j_C");
                        atkBuilder.InitCharState((CharacterStates.Attack)fgChar.State("attackAirborne"));
                        atkBuilder.Movement();
                        atkBuilder.MovementDetails(additive:true);
                        atkBuilder.FramesContinuous((localFrame, target) => {
                            if (localFrame >= 15.0f && localFrame <= 20.0f) {
                                float p = Mathf.InverseLerp(15.0f, 20.0f, localFrame);
                                fgChar.SetStandCollider(p * 0.2f * Vector3.up);
                            }
                            else if (localFrame > 20.0f) {
                                fgChar.SetStandCollider(0.2f * Vector3.up);
                            }
                        });
                        atkBuilder.Frames(
                            FrameUtil.CreateList(f => { f
                                .SpecialCancellable(true)
                                .ChainCancellable(false)
                                .CancellableOnWhiff(false)
                                .CounterHit(true)
                                .From(2)
                                .Execute(() => {
                                    audioService.PlayOneShot(fgChar.GetSpeakPosition(), audioMap["attacksound_02"]);
                                })
                                .To(3)
                                .From(4)
                                    .Track((targetFG) => {
                                        Vector3 newTargetPos = TargetUtil.MoveTargetLimited(fgChar.position, targetFG.targetPos, targetFG.ActualTargetPos(), 20.0f);
                                            // for now, just let the setting of this value be instantaneous once per frame.
                                        fgChar.SetTarget(newTargetPos);
                                    })
                                .To(12)
                                .From(15)
                                    .Hit(hit => {
                                        hit.HitDamage(2000)
                                            .BlockDamage(0)
                                            .HitStun(40.0f)
                                            .BlockStun(30.0f)
                                            .Priority(AttackPriority.HeavyAttack)
                                            .ComboScaling(1.0f);

                                        hit.ValidBlock(BlockType.HIGH);

                                        HitBox defaultHitBox = hitBoxService.Create(hb => {
                                            hb.Relative(fgChar.transform);
                                            hb.Point0(new Vector3(0, -0.2f, 0.4f));
                                            hb.Point1(new Vector3(0, 0.8f, 0.4f));
                                            hb.Radius(0.25f);
                                            hb.InGameEntity(fgChar);
                                            hb.Validate((HitBox _, HitBox other) => true);
                                            hb.Validate((HitBox _, HurtBox other) => true);
                                        });

                                        hit.OnHit((thisHit, hitLocations, entity, hurtBoxes, hitBoxes) => {
                                            if (fgService.IsCurrentFGChar(entity) && entity != fgChar) {
                                                FightingGameCharacter opponent = (FightingGameCharacter)entity;

                                                // This exists to make characters hitting each other async
                                                fgService.Strike(entity, fgChar, thisHit, onHit: (hitAtSameTimeByAttackPriority, damage) => {
                                                    audioService.PlayOneShot(hitLocations[defaultHitBox], audioMap["hit"]);
                                                    PlayRandomGrunt(hitLocations[defaultHitBox]);

                                                    particleService.PlayOneShot(fgChar.position + fgChar.rigidFG.rotation * new Vector3(0, 1.0f, 1.0f) * 1.0f, "hit");

                                                    switch (opponent.GetGroundRelation()) {
                                                        case GroundRelation.GROUNDED:
                                                            opponent.ReceiveHit(thisHit.hitStun, false);
                                                            opponent.KnockBack(
                                                                thisHit.priority,
                                                                launch: false,
                                                                knockback: fgChar.rigidFG.rotation * Vector3.forward * 2.0f);
                                                            break;
                                                        case GroundRelation.AIRBORNE:
                                                            opponent.ReceiveBlocked(thisHit.hitStun, true);
                                                            opponent.KnockBack(
                                                                thisHit.priority,
                                                                launch: true,
                                                                knockback: fgChar.rigidFG.rotation * new Vector3(0.0f, 1.0f, 1.0f) * 0.7071f);
                                                            break;
                                                    }
                                                    opponent.ChangeHealth(damage); // damage includes combo scaling.

                                                }, onBlock: (hitAtSameTimeByAttackPriority, damage) => {
                                                    audioService.PlayOneShot(hitLocations[defaultHitBox], audioMap["block"]);

                                                    particleService.PlayOneShot(fgChar.position + fgChar.rigidFG.rotation * new Vector3(0, 1.0f, 1.0f) * 1.0f, "block");

                                                    // I may put all of these functions into the same call.
                                                    opponent.ReceiveBlocked(thisHit.blockStun, false);
                                                    opponent.KnockBack(
                                                        thisHit.priority,
                                                        launch: false,
                                                        knockback: fgChar.rigidFG.rotation * Vector3.forward * 0.5f);
                                                    opponent.ChangeHealth(damage); // damage is hit.blockDamage
                                                });
                                            }
                                        });

                                        hit.HitBox(defaultHitBox);
                                    })
                                .To(21)
                                    .CounterHit(false)
                                .To(56);
                            }));
                        atkBuilder.CleanUp(ReturnToAirborne);
                    });

                    Attack atkOrt5C = new Attack(atkBuilder => {
                        atkBuilder.Name("orthodox_5C");
                        atkBuilder.Requires(Orientation.ORTHODOX, GroundRelation.GROUNDED);
                        atkBuilder.Input("4|5|6", "C");
                        atkBuilder.AnimationState("5C");
                        atkBuilder.InitCharState((CharacterStates.Attack)fgChar.State("attackGrounded"));
                        atkBuilder.Movement();
                        atkBuilder.MovementDetails(additive: false);
                        atkBuilder.Frames(
                            FrameUtil.CreateList(fl => {
                                fl.SpecialCancellable(true);
                                fl.CancellableOnWhiff(false);
                                fl.CounterHit(true);
                                fl.From(2);
                                fl.Execute(() => {
                                    audioService.PlayOneShot(fgChar.GetSpeakPosition(), audioMap["attacksound_01"]);
                                });
                                fl.To(3);
                                fl.From(18);
                                fl.Hit(hit => {
                                    hit.HitDamage(1200);
                                    hit.BlockDamage(0);
                                    hit.HitStun(32.0f);
                                    hit.BlockStun(20.0f);
                                    hit.ComboScaling(1.0f);
                                    hit.Priority(AttackPriority.HeavyAttack);

                                    HitBox defaultHitBox = hitBoxService.Create(hb => {
                                        hb.Relative(fgChar.transform);
                                        hb.Point0(new Vector3(-0.35f, 0.5f, 0.7f));
                                        hb.Point1(new Vector3(0.35f, 0.5f, 0.7f));
                                        hb.Radius(0.25f);
                                        hb.InGameEntity(fgChar);
                                        hb.Validate((HitBox _, HitBox other) => false);
                                        hb.Validate((HitBox _, HurtBox other) => true);
                                    });

                                    hit.OnHit((thisHit, hitLocations, entity, hurtBoxes, hitBoxes) => {
                                        if (fgService.IsCurrentFGChar(entity) && entity != fgChar) {
                                            FightingGameCharacter opponent = (FightingGameCharacter)entity;

                                            // This exists to make characters hitting each other async
                                            fgService.Strike(entity, fgChar, thisHit, onHit: (hitAtSameTimeByAttackPriority, damage) => {
                                                audioService.PlayOneShot(hitLocations[defaultHitBox], audioMap["hit"]);
                                                PlayRandomGrunt(hitLocations[defaultHitBox]);

                                                particleService.PlayOneShot(fgChar.position + fgChar.rigidFG.rotation * new Vector3(0, 1.0f, 1.0f) * 1.0f, "hit");

                                                switch (opponent.GetGroundRelation()) {
                                                    case GroundRelation.GROUNDED:
                                                        opponent.ReceiveHit(thisHit.hitStun, false);
                                                        opponent.KnockBack(
                                                            thisHit.priority,
                                                            launch: false,
                                                            knockback: fgChar.rigidFG.rotation * Vector3.forward * 2.0f);
                                                        break;
                                                    case GroundRelation.AIRBORNE:
                                                        opponent.ReceiveBlocked(thisHit.hitStun, true);
                                                        opponent.KnockBack(
                                                            thisHit.priority,
                                                            launch: true,
                                                            knockback: fgChar.rigidFG.rotation * new Vector3(0.0f, 1.0f, 1.0f) * 0.7071f);
                                                        break;
                                                }
                                                opponent.ChangeHealth(damage); // damage includes combo scaling.

                                            }, onBlock: (hitAtSameTimeByAttackPriority, damage) => {
                                                audioService.PlayOneShot(hitLocations[defaultHitBox], audioMap["block"]);

                                                particleService.PlayOneShot(fgChar.position + fgChar.rigidFG.rotation * new Vector3(0, 1.0f, 1.0f) * 1.0f, "block");

                                                // I may put all of these functions into the same call.
                                                opponent.ReceiveBlocked(thisHit.blockStun, false);
                                                opponent.KnockBack(
                                                    thisHit.priority,
                                                    launch: false,
                                                    knockback: fgChar.rigidFG.rotation * Vector3.forward * 0.5f);
                                                opponent.ChangeHealth(damage); // damage is hit.blockDamage
                                            });
                                        }
                                    });

                                    hit.HitBox(defaultHitBox);
                                });
                                fl.To(20);
                                fl.From(20);
                                fl.CounterHit(false);
                                fl.ChainCancellable(true);
                                fl.To(50);
                            }));
                        atkBuilder.CleanUp(ReturnToStand);
                    });

                    Attack atkOrt1C = new Attack(atkBuilder => {
                        atkBuilder.Name("orthodox_1C");
                        atkBuilder.Requires(Orientation.ORTHODOX, GroundRelation.GROUNDED);
                        atkBuilder.Input("1", "C");
                        atkBuilder.AnimationState("1C");
                        atkBuilder.InitCharState((CharacterStates.Attack)fgChar.State("attackGrounded"));
                        atkBuilder.Movement(zMoveCb: animationCurveMap["orth_1C.z"].Evaluate);
                        atkBuilder.MovementDetails(additive:false);
                        atkBuilder.Frames(
                            FrameUtil.CreateList(fl => {
                                fl.SpecialCancellable(true);
                                fl.CancellableOnWhiff(false);
                                fl.CounterHit(true);
                                fl.From(2);
                                fl.Execute(() => {
                                    audioService.PlayOneShot(fgChar.GetSpeakPosition(), audioMap["attacksound_03"]);
                                });
                                fl.To(3);
                                fl.From(18);
                                    fl.Hit(hit => {
                                        hit.HitDamage(1000);
                                        hit.BlockDamage(0);
                                        hit.HitStun(30.0f);
                                        hit.BlockStun(12.0f);
                                        hit.ComboScaling(1.0f);
                                        hit.Priority(AttackPriority.LightAttack);

                                        hit.ValidBlock(BlockType.LOW);

                                        HitBox defaultHitBox = hitBoxService.Create(hb => {
                                            hb.Relative(fgChar.transform);
                                            hb.Point0(new Vector3(0, 0, 0.8f));
                                            hb.Point1(new Vector3(0, 0.2f, 1.0f));
                                            hb.Radius(0.25f);
                                            hb.InGameEntity(fgChar);
                                            hb.Validate((HitBox _, HitBox other) => false);
                                            hb.Validate((HitBox _, HurtBox other) => true);
                                        });

                                        hit.OnHit((thisHit, hitLocations, entity, hurtBoxes, hitBoxes) => {
                                            if (fgService.IsCurrentFGChar(entity) && entity != fgChar) {
                                                FightingGameCharacter opponent = (FightingGameCharacter)entity;

                                                // This exists to make characters hitting each other async
                                                fgService.Strike(entity, fgChar, thisHit, onHit: (hitAtSameTimeByAttackPriority, damage) => {
                                                    audioService.PlayOneShot(hitLocations[defaultHitBox], audioMap["hit"]);
                                                    PlayRandomGrunt(hitLocations[defaultHitBox]);

                                                    particleService.PlayOneShot(fgChar.position + fgChar.rigidFG.rotation * new Vector3(0, 1.0f, 1.0f) * 1.0f, "hit");

                                                    switch (opponent.GetGroundRelation()) {
                                                        case GroundRelation.GROUNDED:
                                                            opponent.ReceiveHit(thisHit.hitStun, false);
                                                            opponent.KnockBack(
                                                                thisHit.priority,
                                                                launch: false,
                                                                knockback: fgChar.rigidFG.rotation * Vector3.forward * 2.0f);
                                                            break;
                                                        case GroundRelation.AIRBORNE:
                                                            opponent.ReceiveBlocked(thisHit.hitStun, true);
                                                            opponent.KnockBack(
                                                                thisHit.priority,
                                                                launch: true,
                                                                knockback: fgChar.rigidFG.rotation * new Vector3(0.0f, 1.0f, 1.0f) * 0.7071f);
                                                            break;
                                                    }
                                                    opponent.ChangeHealth(damage); // damage includes combo scaling.

                                                }, onBlock: (hitAtSameTimeByAttackPriority, damage) => {
                                                    audioService.PlayOneShot(hitLocations[defaultHitBox], audioMap["block"]);

                                                    particleService.PlayOneShot(fgChar.position + fgChar.rigidFG.rotation * new Vector3(0, 1.0f, 1.0f) * 1.0f, "block");

                                                    // I may put all of these functions into the same call.
                                                    opponent.ReceiveBlocked(thisHit.blockStun, false);
                                                    opponent.KnockBack(
                                                        thisHit.priority,
                                                        launch: false,
                                                        knockback: fgChar.rigidFG.rotation * Vector3.forward * 0.5f);
                                                    opponent.ChangeHealth(damage); // damage is hit.blockDamage
                                                });
                                            }
                                        });

                                        hit.HitBox(defaultHitBox);
                                        hit.HitBox(hitBoxService.Create(hb => {
                                            hb.Relative(fgChar.transform);
                                            hb.Point0(new Vector3(x: 1.0f, y: 0.1f, z: 0.2f));
                                            hb.Point1(new Vector3(x: 0.8f, y: 0.1f, z: 0.6f));
                                            hb.Radius(0.3f);
                                            hb.InGameEntity(fgChar);
                                        }));
                                        hit.HitBox(hitBoxService.Create(hb => {
                                            hb.Relative(fgChar.transform);
                                            hb.Point0(new Vector3(x: 0.8f, y: 0.1f, z: 0.6f));
                                            hb.Point1(new Vector3(x: 0.0f, y: 0.1f, z: 1.0f));
                                            hb.Radius(0.3f);
                                            hb.InGameEntity(fgChar);
                                        }));
                                        hit.HitBox(hitBoxService.Create(hb => {
                                            hb.Relative(fgChar.transform);
                                            hb.Point0(new Vector3(x: 0.0f, y: 0.1f, z: 1.0f));
                                            hb.Point1(new Vector3(x: -0.8f, y: 0.1f, z: 0.6f));
                                            hb.Radius(0.3f);
                                            hb.InGameEntity(fgChar);
                                        }));
                                        hit.HitBox(hitBoxService.Create(hb => {
                                            hb.Relative(fgChar.transform);
                                            hb.Point0(new Vector3(x: -0.8f, y: 0.1f, z: 0.6f));
                                            hb.Point1(new Vector3(x: -1.0f, y: 0.1f, z: 0.2f));
                                            hb.Radius(0.3f);
                                            hb.InGameEntity(fgChar);
                                        }));
                                    });
                                fl.To(25);
                                fl.From(25);
                                    fl.CounterHit(false);
                                    fl.ChainCancellable(true);
                                fl.To(50);
                            }));
                        atkBuilder.CleanUp(ReturnToStand);
                    });

                    Attack atkOrt2C = new Attack(atkBuilder => {
                        atkBuilder.Name("orthodox_2C");
                        atkBuilder.Requires(Orientation.ORTHODOX, GroundRelation.GROUNDED);
                        atkBuilder.Input("2", "C");
                        atkBuilder.AnimationState("2C");
                        atkBuilder.InitCharState((CharacterStates.Attack)fgChar.State("attackGrounded"));
                        atkBuilder.Movement();
                        atkBuilder.MovementDetails(additive:false);
                        atkBuilder.Frames(
                            FrameUtil.CreateList(fl => {
                                fl.SpecialCancellable(true);
                                fl.CancellableOnWhiff(false);
                                fl.CounterHit(true);
                                fl.From(2);
                                fl.Execute(() => {
                                    audioService.PlayOneShot(fgChar.GetSpeakPosition(), audioMap["attacksound_01"]);
                                });
                                fl.To(3);
                                fl.From(18);
                                    fl.Hit(hit => {
                                        hit.HitDamage(1000);
                                        hit.BlockDamage(0);
                                        hit.HitStun(30.0f);
                                        hit.BlockStun(12.0f);
                                        hit.ComboScaling(1.0f);
                                        hit.Priority(AttackPriority.LightAttack);

                                        HitBox defaultHitBox = hitBoxService.Create(hb => {
                                            hb.Relative(fgChar.transform);
                                            hb.Point0(new Vector3(-0.35f, 0.5f, 0.7f));
                                            hb.Point1(new Vector3(0.35f, 0.5f, 0.7f));
                                            hb.Radius(0.25f);
                                            hb.InGameEntity(fgChar);
                                            hb.Validate((HitBox _, HitBox other) => false);
                                            hb.Validate((HitBox _, HurtBox other) => true);
                                        });

                                        hit.OnHit((thisHit, hitLocations, entity, hurtBoxes, hitBoxes) => {
                                            if (fgService.IsCurrentFGChar(entity) && entity != fgChar) {
                                                FightingGameCharacter opponent = (FightingGameCharacter)entity;

                                                // This exists to make characters hitting each other async
                                                fgService.Strike(entity, fgChar, thisHit, onHit: (hitAtSameTimeByAttackPriority, damage) => {
                                                    audioService.PlayOneShot(hitLocations[defaultHitBox], audioMap["hit"]);
                                                    PlayRandomGrunt(hitLocations[defaultHitBox]);

                                                    particleService.PlayOneShot(fgChar.position + fgChar.rigidFG.rotation * new Vector3(0, 1.0f, 1.0f) * 1.0f, "hit");

                                                    switch (opponent.GetGroundRelation()) {
                                                        case GroundRelation.GROUNDED:
                                                            opponent.ReceiveHit(thisHit.hitStun, false);
                                                            opponent.KnockBack(
                                                                thisHit.priority,
                                                                launch: false,
                                                                knockback: fgChar.rigidFG.rotation * Vector3.forward * 2.0f);
                                                            break;
                                                        case GroundRelation.AIRBORNE:
                                                            opponent.ReceiveBlocked(thisHit.hitStun, true);
                                                            opponent.KnockBack(
                                                                thisHit.priority,
                                                                launch: true,
                                                                knockback: fgChar.rigidFG.rotation * new Vector3(0.0f, 1.0f, 1.0f) * 0.7071f);
                                                            break;
                                                    }
                                                    opponent.ChangeHealth(damage); // damage includes combo scaling.

                                                }, onBlock: (hitAtSameTimeByAttackPriority, damage) => {
                                                    audioService.PlayOneShot(hitLocations[defaultHitBox], audioMap["block"]);

                                                    particleService.PlayOneShot(fgChar.position + fgChar.rigidFG.rotation * new Vector3(0, 1.0f, 1.0f) * 1.0f, "block");

                                                    // I may put all of these functions into the same call.
                                                    opponent.ReceiveBlocked(thisHit.blockStun, false);
                                                    opponent.KnockBack(
                                                        thisHit.priority,
                                                        launch: false,
                                                        knockback: fgChar.rigidFG.rotation * Vector3.forward * 0.5f);
                                                    opponent.ChangeHealth(damage); // damage is hit.blockDamage
                                                });
                                            }
                                        });

                                        hit.HitBox(defaultHitBox);
                                    });
                                fl.To(20);
                                fl.From(20);
                                    fl.CounterHit(false);
                                    fl.ChainCancellable(true);
                                fl.To(50);
                            }));
                        atkBuilder.CleanUp(ReturnToStand);
                    });

                    Attack atkOrt6B = new Attack(atkBuilder => {
                        atkBuilder.Name("orthodox_6B");
                        atkBuilder.Requires(Orientation.ORTHODOX, GroundRelation.GROUNDED);
                        atkBuilder.Input("6", "B");
                        atkBuilder.AnimationState("6B");
                        atkBuilder.InitCharState((CharacterStates.Attack)fgChar.State("attackGrounded"));
                        atkBuilder.Movement(zMoveCb: animationCurveMap["orth_6B.z"].Evaluate);
                        atkBuilder.MovementDetails(additive:false);
                        atkBuilder.Frames(
                            FrameUtil.CreateList(fl => {
                                fl.SpecialCancellable(true);
                                fl.CancellableOnWhiff(false); // TODO: Change this to true when you fix the input buffer
                                fl.ChainCancellable(false);
                                fl.CounterHit(true);
                                fl.From(2);
                                fl.Execute(() => {
                                    audioService.PlayOneShot(fgChar.GetSpeakPosition(), audioMap["attacksound_01"]);
                                });
                                fl.To(3);
                                fl.From(30);
                                fl.Hit(hit => {
                                    hit.HitDamage(1600);
                                    hit.BlockDamage(0);
                                    hit.HitStun(45.0f);
                                    hit.BlockStun(30.0f);
                                    hit.ComboScaling(0.8f);
                                    hit.Priority(AttackPriority.HeavyAttack);

                                    hit.ValidBlock(BlockType.HIGH);

                                    HitBox defaultHitBox = hitBoxService.Create(hb => {
                                        hb.Relative(fgChar.transform);
                                        hb.Point0(new Vector3(0, 0, 1.0f));
                                        hb.Point1(new Vector3(0, 0.8f, 1.0f));
                                        hb.Radius(0.3f);
                                        hb.InGameEntity(fgChar);
                                        hb.Validate((HitBox _, HitBox other) => false);
                                        hb.Validate((HitBox _, HurtBox other) => true);
                                    });

                                    hit.OnHit((thisHit, hitLocations, entity, hurtBoxes, hitBoxes) => {
                                        if (fgService.IsCurrentFGChar(entity) && entity != fgChar) {
                                            FightingGameCharacter opponent = (FightingGameCharacter)entity;

                                            // This exists to make characters hitting each other async
                                            fgService.Strike(entity, fgChar, thisHit, onHit: (hitAtSameTimeByAttackPriority, damage) => {
                                                audioService.PlayOneShot(hitLocations[defaultHitBox], audioMap["hit"]);
                                                PlayRandomGrunt(hitLocations[defaultHitBox]);

                                                particleService.PlayOneShot(fgChar.position + fgChar.rigidFG.rotation * new Vector3(0, 1.0f, 1.0f) * 1.0f, "hit");

                                                switch (opponent.GetGroundRelation()) {
                                                    case GroundRelation.GROUNDED:
                                                        opponent.ReceiveHit(thisHit.hitStun, false);
                                                        opponent.KnockBack(
                                                            thisHit.priority,
                                                            launch: false,
                                                            knockback: fgChar.rigidFG.rotation * Vector3.forward * 2.0f);
                                                        break;
                                                    case GroundRelation.AIRBORNE:
                                                        opponent.ReceiveHit(thisHit.hitStun, true);
                                                        opponent.KnockBack(
                                                            thisHit.priority,
                                                            launch: true,
                                                            knockback: fgChar.rigidFG.rotation * new Vector3(0.0f, 1.0f, 1.0f) * 0.7071f);
                                                        break;
                                                }
                                                opponent.ChangeHealth(damage); // damage includes combo scaling.

                                            }, onBlock: (hitAtSameTimeByAttackPriority, damage) => {
                                                audioService.PlayOneShot(hitLocations[defaultHitBox], audioMap["block"]);

                                                particleService.PlayOneShot(fgChar.position + fgChar.rigidFG.rotation * new Vector3(0, 1.0f, 1.0f) * 1.0f, "block");

                                                // I may put all of these functions into the same call.
                                                opponent.ReceiveBlocked(thisHit.blockStun, false);
                                                opponent.KnockBack(
                                                    thisHit.priority,
                                                    launch: false,
                                                    knockback: fgChar.rigidFG.rotation * Vector3.forward * 0.5f);
                                                opponent.ChangeHealth(damage); // damage is hit.blockDamage
                                            });
                                        }
                                    });

                                    hit.HitBox(defaultHitBox);
                                });
                                fl.To(32);
                                fl.From(32);
                                fl.CounterHit(false);
                                fl.ChainCancellable(false);
                                fl.To(60);
                            }));
                        atkBuilder.CleanUp(ReturnToStand);
                    });

                    Attack atkOrt6A = new Attack(atkBuilder => {
                        atkBuilder.Name("orthodox_6A");
                        atkBuilder.Requires(Orientation.ORTHODOX, GroundRelation.GROUNDED);
                        atkBuilder.Input("6", "A");
                        atkBuilder.AnimationState("6A");
                        atkBuilder.InitCharState((CharacterStates.Attack)fgChar.State("attackGrounded"));
                        atkBuilder.Movement(zMoveCb: animationCurveMap["orth_6A.z"].Evaluate);
                        atkBuilder.MovementDetails(additive: false);
                        atkBuilder.Frames(
                            FrameUtil.CreateList(fl => {
                                fl.SpecialCancellable(true);
                                fl.CancellableOnWhiff(false); // TODO: Change this to true when you fix the input buffer
                                fl.ChainCancellable(false);
                                fl.CounterHit(true);
                                fl.From(15);
                                fl.Hit(hit => {
                                    hit.HitDamage(800);
                                    hit.BlockDamage(0);
                                    hit.HitStun(24.0f);
                                    hit.BlockStun(16.0f);
                                    hit.ComboScaling(1.0f);
                                    hit.Priority(AttackPriority.LightAttack);

                                    hit.ValidBlock(BlockType.HIGH);

                                    HitBox defaultHitBox = hitBoxService.Create(hb => {
                                        hb.Relative(fgChar.transform);
                                        hb.Point0(new Vector3(0, 0, 0.3f));
                                        hb.Point1(new Vector3(0, 0.8f, 1.0f));
                                        hb.Radius(0.3f);
                                        hb.InGameEntity(fgChar);
                                        hb.Validate((HitBox _, HitBox other) => false);
                                        hb.Validate((HitBox _, HurtBox other) => true);
                                    });

                                    hit.OnHit((thisHit, hitLocations, entity, hurtBoxes, hitBoxes) => {
                                        if (fgService.IsCurrentFGChar(entity) && entity != fgChar) {
                                            FightingGameCharacter opponent = (FightingGameCharacter)entity;

                                            // This exists to make characters hitting each other async
                                            fgService.Strike(entity, fgChar, thisHit, onHit: (hitAtSameTimeByAttackPriority, damage) => {
                                                audioService.PlayOneShot(hitLocations[defaultHitBox], audioMap["hit"]);
                                                PlayRandomGrunt(hitLocations[defaultHitBox]);

                                                particleService.PlayOneShot(fgChar.position + fgChar.rigidFG.rotation * new Vector3(0, 1.0f, 1.0f) * 1.0f, "hit");

                                                switch (opponent.GetGroundRelation()) {
                                                    case GroundRelation.GROUNDED:
                                                        opponent.ReceiveHit(thisHit.hitStun, false);
                                                        opponent.KnockBack(
                                                            thisHit.priority,
                                                            launch: false,
                                                            knockback: fgChar.rigidFG.rotation * Vector3.forward * 2.0f);
                                                        break;
                                                    case GroundRelation.AIRBORNE:
                                                        opponent.ReceiveHit(thisHit.hitStun, true);
                                                        opponent.KnockBack(
                                                            thisHit.priority,
                                                            launch: true,
                                                            knockback: fgChar.rigidFG.rotation * new Vector3(0.0f, 1.0f, 1.0f) * 0.7071f);
                                                        break;
                                                }
                                                opponent.ChangeHealth(damage); // damage includes combo scaling.

                                            }, onBlock: (hitAtSameTimeByAttackPriority, damage) => {
                                                audioService.PlayOneShot(hitLocations[defaultHitBox], audioMap["block"]);

                                                particleService.PlayOneShot(fgChar.position + fgChar.rigidFG.rotation * new Vector3(0, 1.0f, 1.0f) * 1.0f, "block");

                                                // I may put all of these functions into the same call.
                                                opponent.ReceiveBlocked(thisHit.blockStun, false);
                                                opponent.KnockBack(
                                                    thisHit.priority,
                                                    launch: false,
                                                    knockback: fgChar.rigidFG.rotation * Vector3.forward * 0.5f);
                                                opponent.ChangeHealth(damage); // damage is hit.blockDamage
                                            });
                                        }
                                    });

                                    hit.HitBox(defaultHitBox);
                                });
                                fl.To(18);
                                fl.From(17);
                                fl.CounterHit(false);
                                fl.ChainCancellable(false);
                                fl.To(36);
                            }));
                        atkBuilder.CleanUp(ReturnToStand);
                    });

                    Attack atkOrt214A = new Attack(atkBuilder => {
                        atkBuilder.Name("orthodox_214A");
                        atkBuilder.Requires(Orientation.ORTHODOX, GroundRelation.GROUNDED);
                        atkBuilder.Input("214", "A");
                        atkBuilder.AnimationState("214L");
                        atkBuilder.InitCharState((CharacterStates.Attack)fgChar.State("attackGrounded"));
                        atkBuilder.Movement(zMoveCb: animationCurveMap["orth_214A.z"].Evaluate);
                        atkBuilder.MovementDetails(additive: false);
                        atkBuilder.Frames(
                            FrameUtil.CreateList(fl => {
                                fl.SpecialCancellable(true);
                                fl.CancellableOnWhiff(false); // TODO: Change this to true when you fix the input buffer
                                fl.ChainCancellable(false);
                                fl.CounterHit(true);
                                fl.From(2);
                                fl.Execute(() => {
                                    audioService.PlayOneShot(fgChar.GetSpeakPosition(), audioMap["attacksound_05"]);
                                });
                                fl.To(3);
                                fl.From(16);
                                fl.Hit(hit => {
                                    hit.HitDamage(800);
                                    hit.BlockDamage(0);
                                    hit.HitStun(24.0f);
                                    hit.BlockStun(16.0f);
                                    hit.ComboScaling(1.0f);
                                    hit.Priority(AttackPriority.HeavyAttack);

                                    HitBox defaultHitBox = hitBoxService.Create(hb => {
                                        hb.Relative(fgChar.transform);
                                        hb.Point0(new Vector3(0.2f, 0.7f, 0.2f));
                                        hb.Point1(new Vector3(0.2f, 1.2f, 0.3f));
                                        hb.Radius(0.2f);
                                        hb.InGameEntity(fgChar);
                                    });

                                    hit.OnHit((thisHit, hitLocations, entity, hurtBoxes, hitBoxes) => {
                                        if (fgService.IsCurrentFGChar(entity) && entity != fgChar) {
                                            FightingGameCharacter opponent = (FightingGameCharacter)entity;

                                            // This exists to make characters hitting each other async
                                            fgService.Strike(entity, fgChar, thisHit, onHit: (hitAtSameTimeByAttackPriority, damage) => {
                                                audioService.PlayOneShot(hitLocations[defaultHitBox], audioMap["hit"]);
                                                PlayRandomGrunt(hitLocations[defaultHitBox]);

                                                particleService.PlayOneShot(fgChar.position + fgChar.rigidFG.rotation * new Vector3(0, 1.0f, 1.0f) * 1.0f, "hit");

                                                opponent.ReceiveHit(thisHit.hitStun, true);
                                                opponent.KnockBack(
                                                    thisHit.priority,
                                                    launch: true,
                                                    knockback: fgChar.rigidFG.rotation * new Vector3(0.0f, 0.6f, 0.2f) * 0.7071f);
                                                opponent.ChangeHealth(damage); // damage includes combo scaling.

                                            }, onBlock: (hitAtSameTimeByAttackPriority, damage) => {
                                                audioService.PlayOneShot(hitLocations[defaultHitBox], audioMap["block"]);

                                                particleService.PlayOneShot(fgChar.position + fgChar.rigidFG.rotation * new Vector3(0, 1.0f, 1.0f) * 1.0f, "block");

                                                // I may put all of these functions into the same call.
                                                opponent.ReceiveBlocked(thisHit.blockStun, false);
                                                opponent.KnockBack(
                                                    thisHit.priority,
                                                    launch: false,
                                                    knockback: fgChar.rigidFG.rotation * Vector3.forward * 0.5f);
                                                opponent.ChangeHealth(damage); // damage is hit.blockDamage
                                            });
                                        }
                                    });

                                    hit.HitBox(defaultHitBox);
                                });
                                fl.To(18);
                                fl.From(17);
                                fl.CounterHit(false);
                                fl.ChainCancellable(false);
                                fl.From(28);
                                fl.Hit(hit => {
                                    hit.HitDamage(800);
                                    hit.BlockDamage(0);
                                    hit.HitStun(40.0f);
                                    hit.BlockStun(20.0f);
                                    hit.ComboScaling(1.0f);
                                    hit.Priority(AttackPriority.HeavyAttack);

                                    HitBox defaultHitBox = hitBoxService.Create(hb => {
                                        hb.Relative(fgChar.transform);
                                        hb.Point0(new Vector3(-0.2f, 0.1f, 0.4f));
                                        hb.Point1(new Vector3(-0.2f, 0.6f, 0.5f));
                                        hb.Radius(0.3f);
                                        hb.InGameEntity(fgChar);
                                    });

                                    hit.OnHit((thisHit, hitLocations, entity, hurtBoxes, hitBoxes) => {
                                        if (fgService.IsCurrentFGChar(entity) && entity != fgChar) {
                                            FightingGameCharacter opponent = (FightingGameCharacter)entity;

                                            // This exists to make characters hitting each other async
                                            fgService.Strike(entity, fgChar, thisHit, onHit: (hitAtSameTimeByAttackPriority, damage) => {
                                                audioService.PlayOneShot(hitLocations[defaultHitBox], audioMap["hit"]);
                                                PlayRandomGrunt(hitLocations[defaultHitBox]);

                                                particleService.PlayOneShot(fgChar.position + fgChar.rigidFG.rotation * new Vector3(0, 1.0f, 1.0f) * 1.0f, "hit");

                                                opponent.ReceiveHit(thisHit.hitStun, true);
                                                opponent.KnockBack(
                                                    thisHit.priority,
                                                    launch: true,
                                                    knockback: fgChar.rigidFG.rotation * new Vector3(0.0f, 4.5f, 1.0f));
                                                opponent.ChangeHealth(damage); // damage includes combo scaling.

                                            }, onBlock: (hitAtSameTimeByAttackPriority, damage) => {
                                                audioService.PlayOneShot(hitLocations[defaultHitBox], audioMap["block"]);

                                                particleService.PlayOneShot(fgChar.position + fgChar.rigidFG.rotation * new Vector3(0, 1.0f, 1.0f) * 1.0f, "block");

                                                // I may put all of these functions into the same call.
                                                opponent.ReceiveBlocked(thisHit.blockStun, false);
                                                opponent.KnockBack(
                                                    thisHit.priority,
                                                    launch: false,
                                                    knockback: fgChar.rigidFG.rotation * Vector3.forward * 2.0f);
                                                opponent.ChangeHealth(damage); // damage is hit.blockDamage
                                            });
                                        }
                                    });

                                    hit.HitBox(defaultHitBox);
                                });
                                fl.To(30);
                                fl.ChainCancellable(false);
                                fl.To(50);
                            }));
                        atkBuilder.CleanUp(ReturnToStand);
                    });

                    Attack atkOrt214B = new Attack(atkBuilder => {
                        atkBuilder.Name("orthodox_214B");
                        atkBuilder.Requires(Orientation.ORTHODOX, GroundRelation.GROUNDED);
                        atkBuilder.Input("214", "B");
                        atkBuilder.AnimationState("214M");
                        atkBuilder.InitCharState((CharacterStates.Attack)fgChar.State("attackGrounded"));
                        atkBuilder.Movement(zMoveCb: animationCurveMap["orth_214B.z"].Evaluate);
                        atkBuilder.MovementDetails(additive: false);
                        atkBuilder.Frames(
                            FrameUtil.CreateList(fl => {
                                fl.SpecialCancellable(true);
                                fl.CancellableOnWhiff(false); // TODO: Change this to true when you fix the input buffer
                                fl.ChainCancellable(false);
                                fl.CounterHit(true);
                                fl.From(2);
                                fl.Execute(() => {
                                    audioService.PlayOneShot(fgChar.GetSpeakPosition(), audioMap["attacksound_05"]);
                                });
                                fl.To(3);
                                fl.From(26);
                                fl.Hit(hit => {
                                    hit.HitDamage(800);
                                    hit.BlockDamage(0);
                                    hit.HitStun(30.0f);
                                    hit.BlockStun(32.0f);
                                    hit.ComboScaling(1.0f);
                                    hit.Priority(AttackPriority.HeavyAttack);

                                    HitBox defaultHitBox = hitBoxService.Create(hb => {
                                        hb.Relative(fgChar.transform);
                                        hb.Point0(new Vector3(0.2f, 0.7f, 0.2f));
                                        hb.Point1(new Vector3(0.2f, 1.2f, 0.3f));
                                        hb.Radius(0.2f);
                                        hb.InGameEntity(fgChar);
                                    });

                                    hit.OnHit((thisHit, hitLocations, entity, hurtBoxes, hitBoxes) => {
                                        if (fgService.IsCurrentFGChar(entity) && entity != fgChar) {
                                            FightingGameCharacter opponent = (FightingGameCharacter)entity;

                                            // This exists to make characters hitting each other async
                                            fgService.Strike(entity, fgChar, thisHit, onHit: (hitAtSameTimeByAttackPriority, damage) => {
                                                audioService.PlayOneShot(hitLocations[defaultHitBox], audioMap["hit"]);
                                                PlayRandomGrunt(hitLocations[defaultHitBox]);

                                                particleService.PlayOneShot(fgChar.position + fgChar.rigidFG.rotation * new Vector3(0, 1.0f, 1.0f) * 1.0f, "hit");

                                                opponent.ReceiveHit(thisHit.hitStun, true);
                                                opponent.KnockBack(
                                                    thisHit.priority,
                                                    launch: true,
                                                    knockback: fgChar.rigidFG.rotation * new Vector3(0.0f, 0.6f, 0.2f) * 0.7071f);
                                                opponent.ChangeHealth(damage); // damage includes combo scaling.

                                            }, onBlock: (hitAtSameTimeByAttackPriority, damage) => {
                                                audioService.PlayOneShot(hitLocations[defaultHitBox], audioMap["block"]);

                                                particleService.PlayOneShot(fgChar.position + fgChar.rigidFG.rotation * new Vector3(0, 1.0f, 1.0f) * 1.0f, "block");

                                                // I may put all of these functions into the same call.
                                                opponent.ReceiveBlocked(thisHit.blockStun, false);
                                                opponent.KnockBack(
                                                    thisHit.priority,
                                                    launch: false,
                                                    knockback: fgChar.rigidFG.rotation * Vector3.forward * 0.5f);
                                                opponent.ChangeHealth(damage); // damage is hit.blockDamage
                                            });
                                        }
                                    });

                                    hit.HitBox(defaultHitBox);
                                });
                                fl.To(40);
                                fl.ChainCancellable(false);
                                fl.To(50);
                            }));
                        atkBuilder.CleanUp(ReturnToStand);
                    });

                    Attack atkOrt214C = new Attack(atkBuilder => {
                        atkBuilder.Name("orthodox_214C");
                        atkBuilder.Requires(Orientation.ORTHODOX, GroundRelation.GROUNDED);
                        atkBuilder.Input("214", "C");
                        atkBuilder.AnimationState("214H");
                        atkBuilder.InitCharState((CharacterStates.Attack)fgChar.State("attackGrounded"));
                        atkBuilder.Movement(zMoveCb: animationCurveMap["orth_214C.z"].Evaluate);
                        atkBuilder.MovementDetails(additive: false);
                        atkBuilder.Frames(
                            FrameUtil.CreateList(fl => {
                                fl.SpecialCancellable(true);
                                fl.CancellableOnWhiff(false); // TODO: Change this to true when you fix the input buffer
                                fl.ChainCancellable(false);
                                fl.CounterHit(true);
                                fl.From(2);
                                fl.Execute(() => {
                                    audioService.PlayOneShot(fgChar.GetSpeakPosition(), audioMap["attacksound_04"]);
                                });
                                fl.To(3);
                                fl.From(37);
                                fl.Hit(hit => {
                                    hit.HitDamage(800);
                                    hit.BlockDamage(0);
                                    hit.HitStun(30.0f);
                                    hit.BlockStun(32.0f);
                                    hit.ComboScaling(1.0f);
                                    hit.Priority(AttackPriority.HeavyAttack);

                                    hit.ValidBlock(BlockType.HIGH);

                                    HitBox defaultHitBox = hitBoxService.Create(hb => {
                                        hb.Relative(fgChar.transform);
                                        hb.Point0(new Vector3(-0.2f, 0.2f, 1.0f));
                                        hb.Point1(new Vector3(-0.2f, 0.2f, 1.0f));
                                        hb.Radius(0.2f);
                                        hb.InGameEntity(fgChar);
                                    });

                                    hit.OnHit((thisHit, hitLocations, entity, hurtBoxes, hitBoxes) => {
                                        if (fgService.IsCurrentFGChar(entity) && entity != fgChar) {
                                            FightingGameCharacter opponent = (FightingGameCharacter)entity;

                                            // This exists to make characters hitting each other async
                                            fgService.Strike(entity, fgChar, thisHit, onHit: (hitAtSameTimeByAttackPriority, damage) => {
                                                audioService.PlayOneShot(hitLocations[defaultHitBox], audioMap["hit"]);
                                                PlayRandomGrunt(hitLocations[defaultHitBox]);

                                                particleService.PlayOneShot(fgChar.position + fgChar.rigidFG.rotation * new Vector3(0, 1.0f, 1.0f) * 1.0f, "hit");

                                                opponent.ReceiveHit(thisHit.hitStun, true);
                                                opponent.KnockBack(
                                                    thisHit.priority,
                                                    launch: true,
                                                    knockback: fgChar.rigidFG.rotation * new Vector3(0.0f, 0.6f, 0.2f) * 0.7071f);
                                                opponent.ChangeHealth(damage); // damage includes combo scaling.

                                            }, onBlock: (hitAtSameTimeByAttackPriority, damage) => {
                                                audioService.PlayOneShot(hitLocations[defaultHitBox], audioMap["block"]);

                                                particleService.PlayOneShot(fgChar.position + fgChar.rigidFG.rotation * new Vector3(0, 1.0f, 1.0f) * 1.0f, "block");

                                                // I may put all of these functions into the same call.
                                                opponent.ReceiveBlocked(thisHit.blockStun, false);
                                                opponent.KnockBack(
                                                    thisHit.priority,
                                                    launch: false,
                                                    knockback: fgChar.rigidFG.rotation * Vector3.forward * 0.5f);
                                                opponent.ChangeHealth(damage); // damage is hit.blockDamage
                                            });
                                        }
                                    });

                                    hit.HitBox(defaultHitBox);
                                });
                                fl.To(39);
                                fl.ChainCancellable(false);
                                fl.To(55);
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

                    charBuilder.Attack(throwNormal, (charState, currAttack, prevAttacks) => { // prevAttacks is a List<Attack>
                        if (!validGroundedAttackStates.Contains(charState)) {
                            return false;
                        }
                        if (currAttack == null) {
                            return true;
                        }
                        return false;
                    });
                    charBuilder.Attack(atkOrt5A, (charState, currAttack, prevAttacks) => {
                        if (!validGroundedAttackStates.Contains(charState)) {
                            return false;
                        }
                        if (currAttack == null ||
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
                    charBuilder.Attack(atkOrt6A, (charState, currAttack, prevAttacks) => {
                        if (!validGroundedAttackStates.Contains(charState)) {
                            return false;
                        }
                        if (currAttack == null ||
                            currAttack == atkOrt2A ||
                            currAttack == atkOrt2AA ||
                            currAttack == atkOrt5A) {
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
                            currAttack == atkOrt5AAA ||
                            currAttack == atkOrt2B ||
                            currAttack == atkOrt2BB ||
                            currAttack == atkOrt2BBB) {
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
                    charBuilder.Attack(atkOrt6B, (charState, currAttack, prevAttacks) => {
                        if (!validGroundedAttackStates.Contains(charState)) {
                            return false;
                        }
                        if (currAttack == null) {
                            return true;
                        }
                        return false;
                    });
                    charBuilder.Attack(atkOrt2B, (charState, currAttack, prevAttacks) => {
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
                            currAttack == atkOrt5BBB) {
                            return true;
                        }
                        return false;
                    });
                    charBuilder.Attack(atkOrt2BB, (charState, currAttack, prevAttacks) => {
                        if (!validGroundedAttackStates.Contains(charState)) {
                            return false;
                        }
                        if (currAttack == atkOrt2B) {
                            return true;
                        }
                        return false;
                    });
                    charBuilder.Attack(atkOrt2BBB, (charState, currAttack, prevAttacks) => {
                        if (!validGroundedAttackStates.Contains(charState)) {
                            return false;
                        }
                        if (currAttack == atkOrt2BB) {
                            return true;
                        }
                        return false;
                    });
                    charBuilder.Attack(atkOrt5C, (charState, currAttack, prevAttacks) => {
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
                            currAttack == atkOrt2B ||
                            currAttack == atkOrt2BB ||
                            currAttack == atkOrt2BBB) {
                            return true;
                        }
                        return false;
                    });
                    charBuilder.Attack(atkOrt1C, (charState, currAttack, prevAttacks) => {
                        if (!validGroundedAttackStates.Contains(charState)) {
                            return false;
                        }
                        if (currAttack == null) {
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
                            currAttack == atkOrt5C) {
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
                    charBuilder.Attack(atkJump5B, (charState, currAttack, prevAttacks) => {
                        if (!validAirborneAttackStates.Contains(charState)) {
                            return false;
                        }
                        if (currAttack == null ||
                            currAttack == atkJump5A ||
                            currAttack == atkJump5AA) {
                            return true;
                        }
                        return false;
                    });
                    charBuilder.Attack(atkJump5BB, (charState, currAttack, prevAttacks) => {
                        if (!validAirborneAttackStates.Contains(charState)) {
                            return false;
                        }
                        if (currAttack == atkJump5B) {
                            return true;
                        }
                        return false;
                    });
                    charBuilder.Attack(atkJump5C, (charState, currAttack, prevAttacks) => {
                        if (!validAirborneAttackStates.Contains(charState)) {
                            return false;
                        }
                        if (currAttack == null ||
                            currAttack == atkJump5A ||
                            currAttack == atkJump5AA ||
                            currAttack == atkJump5B ||
                            currAttack == atkJump5BB) {
                            return true;
                        }
                        return false;
                    });
                    charBuilder.Attack(atkOrt214A, (charState, currAttack, prevAttacks) => {
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
                            currAttack == atkOrt6B ||
                            currAttack == atkOrt2B ||
                            currAttack == atkOrt2BB ||
                            currAttack == atkOrt2BBB ||
                            currAttack == atkOrt5C ||
                            currAttack == atkOrt2C) {
                            return true;
                        }
                        return false;
                    });
                    charBuilder.Attack(atkOrt214B, (charState, currAttack, prevAttacks) => {
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
                            currAttack == atkOrt6B ||
                            currAttack == atkOrt2B ||
                            currAttack == atkOrt2BB ||
                            currAttack == atkOrt2BBB ||
                            currAttack == atkOrt5C ||
                            currAttack == atkOrt2C) {
                            return true;
                        }
                        return false;
                    });
                    charBuilder.Attack(atkOrt214C, (charState, currAttack, prevAttacks) => {
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
                            currAttack == atkOrt6B ||
                            currAttack == atkOrt2B ||
                            currAttack == atkOrt2BB ||
                            currAttack == atkOrt2BBB ||
                            currAttack == atkOrt5C ||
                            currAttack == atkOrt2C) {
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

                private void PlayRandomGrunt(Vector3 location) {
                    int index = (int)(UnityEngine.Random.value * 5.0f) + 1;
                    audioService.PlayOneShot(location, audioMap["grunt_0" + index]);
                }
            }
        }
    }
}