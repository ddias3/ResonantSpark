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

            private HashSet<BlockType> nextHitValidBlocks;
            private List<BlockType> blockTypes;

            public void Awake() {
                FrameEnforcer frame = GameObject.FindGameObjectWithTag("rspTime").GetComponent<FrameEnforcer>();
                frame.AddUpdate((int)FramePriority.TrainingDummy, new Action<int>(FrameUpdate));

                nextHitValidBlocks = new HashSet<BlockType>();
                blockTypes = new List<BlockType>((BlockType[]) Enum.GetValues(typeof(BlockType)));
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
                            List<Hit> upComingHits = opponent.GetNextHitInCurrentAttack();
                            if (upComingHits != null) {
                                nextHitValidBlocks.Clear();
                                foreach (BlockType blockType in blockTypes) {
                                    bool validBlock = true;
                                    foreach (Hit hit in upComingHits) {
                                        if (!hit.validBlocks.Contains(blockType)) {
                                            validBlock = false;
                                            break;
                                        }
                                    }
                                    if (validBlock) {
                                        nextHitValidBlocks.Add(blockType);
                                    }
                                }

                                System.Text.StringBuilder toString = new System.Text.StringBuilder();
                                toString.Append("ValidBlocks(");
                                toString.Append(frameIndex);
                                toString.Append(") [");
                                foreach (BlockType block in nextHitValidBlocks) {
                                    toString.Append(block.ToString());
                                    toString.Append(", ");
                                }
                                toString.Append("]");
                                Debug.LogFormat(toString.ToString());

                                if (nextHitValidBlocks.Contains(BlockType.LOW)) {
                                    controller.SetInput(fgChar.MapRelativeToAbsolute(FightingGameInputCodeDir.DownBack));
                                }
                                else if (nextHitValidBlocks.Contains(BlockType.HIGH)) {
                                    controller.SetInput(fgChar.MapRelativeToAbsolute(FightingGameInputCodeDir.Back));
                                }
                                else {
                                    throw new Exception("Attack requires both high and low blocking");
                                }
                            }
                            else {
                                controller.SetInput(FightingGameAbsInputCodeDir.Neutral);
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