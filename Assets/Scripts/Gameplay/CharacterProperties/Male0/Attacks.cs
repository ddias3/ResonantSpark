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
                                                ((FightingGameCharacter)hitInfo.hitEntity).ChangeHealth(1000);
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
                                    .HitBox(h => {
                                        h.Relative(fgChar.gameObject.transform);
                                        h.Point0(new Vector3(0, 0, 0));
                                        h.Point1(new Vector3(0, 1, 0));
                                        h.Radius(0.25f);
                                        h.Event("onHurtBox", (hitInfo) => {
                                            Debug.Log("Regular 5AA hit the enemy");
                                            audioService.PlayOneShot(hitInfo.position, audioMap["regular_5A"]);
                                            hitInfo.hitEntity.GetHitBy(hitInfo.hitBox);
                                            if (hitInfo.hitEntity != fgChar) {
                                                ((FightingGameCharacter)hitInfo.hitEntity).ChangeHealth(1000);
                                                hitInfo.hitEntity.GetHitBy(hitInfo.hitBox);
                                            }
                                        });
                                        h.Event("onHitBox", (hitInfo) => {
                                            Debug.Log("Regular 5AA hit another hitbox");
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

                    Attack atkReg2A = new Attack(atkBuilder => { atkBuilder
                        .Name("regular_2A")
                        .Orientation(Orientation.REGULAR)
                        .GroundRelation(GroundRelation.GROUNDED)
                        .Input(InputNotation._2A)
                        .AnimationState("regular_2A")
                        .Frames(
                            FrameUtil.CreateList(f => { f
                                .SpecialCancellable(true)
                                .From(1)
                                    .HitBox()
                                    .ChainCancellable(false)
                                .To(12)
                                .From(12)
                                    .HitBox(h => {
                                        h.Relative(fgChar.gameObject.transform);
                                        h.Point0(new Vector3(0, 0, 0));
                                        h.Point1(new Vector3(0, 1, 0));
                                        h.Radius(0.25f);
                                        h.Event("onHurtBox", (hitInfo) => {
                                            Debug.Log("Regular 2A hit the enemy");
                                            if (hitInfo.hitEntity != fgChar) {
                                                ((FightingGameCharacter)hitInfo.hitEntity).ChangeHealth(1000);
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

                    charBuilder
                        .Attack(atkReg5A, (charState, currAttack) => {
                            if (currAttack == null ||
                                currAttack == atkReg2A) {
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
                        .Attack(atkReg2A, (charState, currAttack) => {
                            if (currAttack == null ||
                                currAttack == atkReg5A) {
                                return true;
                            }
                            return false;
                        });

                    //charBuilder.Adapter("1.0", "1.2", new Version1_2Adapter());
                }
            }
        }
    }
}