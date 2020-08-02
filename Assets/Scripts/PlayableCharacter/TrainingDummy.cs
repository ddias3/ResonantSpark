using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.CharacterProperties;
using ResonantSpark.Character;
using ResonantSpark.Service;
using ResonantSpark.Input;

namespace ResonantSpark {
    namespace Gameplay {
        public enum DummyBlock : int {
            None,
            Low,
            High,
            All,
        };

        public class TrainingDummy : MonoBehaviour {
            public FightingGameCharacter fgChar;

            private DummyInputController controller;

            private IFightingGameService fgService;
            private FightingGameCharacter opponent;
            private List<Projectile> enemyProjectiles;

            private DummyBlock blockMode;

            private HashSet<BlockType> nextHitRequiredBlocks;
            private List<BlockType> validBlocks;

            public void Awake() {
                FrameEnforcer frame = GameObject.FindGameObjectWithTag("rspTime").GetComponent<FrameEnforcer>();
                frame.AddUpdate((int)FramePriority.InputBuffer, new Action<int>(FrameUpdate));

                nextHitRequiredBlocks = new HashSet<BlockType>();
                validBlocks = new List<BlockType>();
                enemyProjectiles = new List<Projectile>();
            }

            public void Init(DummyInputController controller, IFightingGameService fgService, FightingGameCharacter opponent) {
                this.controller = controller;
                this.fgService = fgService;
                this.opponent = opponent;

                blockMode = DummyBlock.None;
            }

            public void SetBlockMode(string mode) {
                switch (mode) {
                    case "none":
                        blockMode = DummyBlock.None;
                        break;
                    case "all":
                        blockMode = DummyBlock.All;
                        break;
                    case "low":
                        blockMode = DummyBlock.Low;
                        break;
                    case "high":
                        blockMode = DummyBlock.High;
                        break;
                }
            }

            public void FrameUpdate(int frameIndex) {
                switch (blockMode) {
                    case DummyBlock.All:
                        if (opponent.isAttacking) {
                            Attack attack = opponent.GetCurrentAttack();
                            if (attack == null) return;

                            attack.GetNextBlockRequirements(nextHitRequiredBlocks);

                            fgService.PopulateValidBlocking(validBlocks, nextHitRequiredBlocks);
                            System.Text.StringBuilder toString = new System.Text.StringBuilder();
                            toString.Append("ValidBlocks(");
                            toString.Append(frameIndex);
                            toString.Append(") [");
                            for (int n = 0; n < validBlocks.Count; ++n) {
                                toString.Append(validBlocks[n].ToString());
                                toString.Append(", ");
                            }
                            toString.Append("]");
                            Debug.LogFormat(toString.ToString());

                            if (validBlocks.Contains(BlockType.LOW)) {
                                controller.SetInput(fgChar.MapRelativeToAbsolute(FightingGameInputCodeDir.DownBack));
                            }
                            else if (validBlocks.Contains(BlockType.HIGH)) {
                                controller.SetInput(fgChar.MapRelativeToAbsolute(FightingGameInputCodeDir.Back));
                            }
                            else {
                                throw new Exception("Attack requires both high and low blocking");
                            }
                        }
                        else {
                            controller.SetInput(FightingGameAbsInputCodeDir.Neutral);
                        }
                        break;
                    case DummyBlock.High:
                        if (opponent.isAttacking) {
                            controller.SetInput(fgChar.MapRelativeToAbsolute(FightingGameInputCodeDir.Back));
                        }
                        else {
                            controller.SetInput(FightingGameAbsInputCodeDir.Neutral);
                        }
                        break;
                    case DummyBlock.Low:
                        if (opponent.isAttacking) {
                            controller.SetInput(fgChar.MapRelativeToAbsolute(FightingGameInputCodeDir.DownBack));
                        }
                        else {
                            controller.SetInput(FightingGameAbsInputCodeDir.Neutral);
                        }
                        break;
                    case DummyBlock.None:
                        controller.SetInput(FightingGameAbsInputCodeDir.Neutral);
                        break;
                }
            }
        }
    }
}