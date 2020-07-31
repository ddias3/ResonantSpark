using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.CharacterProperties;
using ResonantSpark.Character;
using ResonantSpark.Service;

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

            private IFightingGameService fgService;
            private FightingGameCharacter opponent;
            private List<Projectile> enemyProjectiles;

            private DummyBlock blockMode;

            private HashSet<BlockType> nextHitRequiredBlocks;
            private List<BlockType> validBlocks;

            public void Awake() {
                nextHitRequiredBlocks = new HashSet<BlockType>();
                validBlocks = new List<BlockType>();
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
                if (opponent.isAttacking) {
                    Attack attack = opponent.GetCurrentAttack();
                    attack.GetNextBlockRequirements(nextHitRequiredBlocks);

                    fgService.PopulateValidBlocking(validBlocks, nextHitRequiredBlocks);

                    if (validBlocks.Contains(BlockType.LOW)) {
                        //TODO: fgChar.SetInput(Factory.Get<DirectionCurrent>(FightingGameInputDir.DownBack));
                    }
                    else if (validBlocks.Contains(BlockType.HIGH)) {
                        //TODO: fgChar.SetInput(Factory.Get<DirectionCurrent>(FightingGameInputDir.Back));
                    }
                    else {
                        throw new Exception("Attack requires both high and low blocking");
                    }
                }
            }
        }
    }
}