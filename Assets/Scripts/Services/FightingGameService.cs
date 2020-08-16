using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

using ResonantSpark.Gameplay;
using ResonantSpark.Gamemode;
using ResonantSpark.Camera;
using ResonantSpark.Character;
using ResonantSpark.Utility;

namespace ResonantSpark {
    namespace Service {
        public enum ComboState : int {
            None,
            InCombo,
        }

        public struct CharacterVulnerability {
            public bool throwable;
            public bool strikable;
            public bool armored;
        }

        // A,B ent Strike each other
        // A,B ent Throw each other
        // A ent Strike B
        // A ent Throw B
        // A ent Strike B and B ent Throw A
        public enum ResolveInteractionType : int {
            StrikeEachOther,
            ThrowEachOther,
            Strike,
            Throw,
            StrikeWhileThrow,
            ThrowWhileStrike,
        }

        public class ResolveInteraction {
            public ResolveInteractionType type;
            public InGameEntity byEntity;
            public InGameEntity hitEntity;
            public object hitQueueObject;
        }

        public class FightingGameService : MonoBehaviour, IFightingGameService {

            public GameObject mapCamera;
            public Vector3 underLevel = new Vector3(0, -100, 0);
            public Transform spawnPoint;
            public float offset = 3.0f;
            public Transform outOfBounds;
            public Transform cameraStart;

            public List<Transform> levelBoundaries;

            private PlayerService playerService;
            private PersistenceService persistenceService;

            private List<InGameEntity> entities;
            private Dictionary<InGameEntity, bool> activeEntities;
            private Dictionary<InGameEntity, int> numComboHits;
            private Dictionary<InGameEntity, int> comboScalingIndex;

            private HashSet<BlockType> requiredBlocks;

            private Dictionary<InGameEntity, ComboState> prevComboState;

            private List<(InGameEntity hitEntity, InGameEntity byEntity, Hit hit, Action<AttackPriority, int> onHit, Action<AttackPriority, int> onBlock)> strikeQueue;
            private List<(InGameEntity hitEntity, InGameEntity byEntity, Hit hit, Action<AttackPriority> onGrabbed)> throwQueue;

            private Dictionary<InGameEntity, List<(InGameEntity hitEntity, InGameEntity byEntity, object hitQueueObj)>> resolveStrikeBy2;
            private Dictionary<InGameEntity, List<(InGameEntity hitEntity, InGameEntity byEntity, object hitQueueObj)>> resolveThrowBy2;
            private Dictionary<InGameEntity, InGameEntity> resolveStrikeBy;
            private Dictionary<InGameEntity, InGameEntity> resolveThrowBy;
            private List<ResolveInteraction> resolveInteractions;

            private Dictionary<InGameEntity, InGameEntity> entityGrabbingEntity;
            private Dictionary<InGameEntity, InGameEntity> entityGrabbingEntityContinue;
            private Dictionary<InGameEntity, bool> entityGrabbingEntityBreakable;
            private Dictionary<InGameEntity, bool> entityGrabbingEntityBreakableNextFrame;
            private List<InGameEntity> entityGrabbingDeleteQueue;

            private FrameEnforcer frame;

            private IGamemode gamemode;
            private IHookExpose gamemodeHooks;

            public void Start() {
                frame = GameObject.FindGameObjectWithTag("rspTime").GetComponent<FrameEnforcer>();
                frame.AddUpdate((int)FramePriority.ServiceFG, new System.Action<int>(FrameUpdate));
                frame.AddUpdate((int)FramePriority.LateService, new System.Action<int>(LateFrameUpdate));

                playerService = GetComponent<PlayerService>();
                persistenceService = GetComponent<PersistenceService>();

                entities = new List<InGameEntity>();
                activeEntities = new Dictionary<InGameEntity, bool>();
                numComboHits = new Dictionary<InGameEntity, int>();
                comboScalingIndex = new Dictionary<InGameEntity, int>();

                requiredBlocks = new HashSet<BlockType>();

                prevComboState = new Dictionary<InGameEntity, ComboState>();

                strikeQueue = new List<(InGameEntity hitEntity, InGameEntity byEntity, Hit hit, Action<AttackPriority, int> onHit, Action<AttackPriority, int> onBlock)>();
                throwQueue = new List<(InGameEntity hitEntity, InGameEntity byEntity, Hit hit, Action<AttackPriority> onGrabbed)>();
                resolveStrikeBy = new Dictionary<InGameEntity, InGameEntity>();
                resolveStrikeBy2 = new Dictionary<InGameEntity, List<(InGameEntity hitEntity, InGameEntity byEntity, object hitQueueObj)>>();
                resolveThrowBy = new Dictionary<InGameEntity, InGameEntity>();
                resolveThrowBy2 = new Dictionary<InGameEntity, List<(InGameEntity hitEntity, InGameEntity byEntity, object hitQueueObj)>>();
                resolveInteractions = new List<ResolveInteraction>();
                entityGrabbingEntity = new Dictionary<InGameEntity, InGameEntity>();
                entityGrabbingEntityContinue = new Dictionary<InGameEntity, InGameEntity>();
                entityGrabbingDeleteQueue = new List<InGameEntity>();
                entityGrabbingEntityBreakable = new Dictionary<InGameEntity, bool>();
                entityGrabbingEntityBreakableNextFrame = new Dictionary<InGameEntity, bool>();
                mapCamera.SetActive(false);

                EventManager.TriggerEvent<Events.ServiceReady, Type>(typeof(FightingGameService));
            }

            public void RegisterInGameEntity(InGameEntity entity) {
                entities.Add(entity);
                activeEntities.Add(entity, false);
                resolveStrikeBy2.Add(entity, new List<(InGameEntity hitEntity, InGameEntity byEntity, object hitQueueObj)>());
                resolveThrowBy2.Add(entity, new List<(InGameEntity hitEntity, InGameEntity byEntity, object hitQueueObj)>());
                numComboHits[entity] = 0;
                comboScalingIndex[entity] = 0;
                prevComboState.Add(entity, ComboState.None);
            }

            public void RemoveInGameEntity(InGameEntity entity) {
                entities.Remove(entity);
                activeEntities.Remove(entity);
                resolveStrikeBy2.Remove(entity);
                resolveThrowBy2.Remove(entity);
                numComboHits[entity] = 0;
                comboScalingIndex[entity] = 0;
                prevComboState.Remove(entity);
            }

            public void ActiveInGameEntity(InGameEntity entity, bool active) {
                activeEntities[entity] = active;
            }

            public void CreateGamemode() {
                GameObject newGameMode = GameObject.Instantiate(persistenceService.GetGamemode());
                newGameMode.name = "Gamemode";
                this.gamemode = newGameMode.GetComponent<IGamemode>();
                this.gamemodeHooks = newGameMode.GetComponent<IHookExpose>();

                this.gamemode.CreateDependencies(GetComponent<BuildService>().GetAllServices());
            }

            public void SetUpGamemode() {
                gamemode.SetUp();

                HookUpGamemode(gamemodeHooks.GetHooks());
            }

            private void HookUpGamemode(Dictionary<string, UnityEventBase> hooks) {
                HookReceive hookReceive = new HookReceive(hooks);
                hookReceive.HookIn("loadMainMenu", new UnityAction(LoadMainMenu));
            }

            public bool IsCurrentFGChar(InGameEntity entity) {
                return gamemode.IsCurrentFGChar(entity);
            }

            public void Strike(InGameEntity hitEntity, InGameEntity byEntity, Hit hit, Action<AttackPriority, int> onHit, Action<AttackPriority, int> onBlock) {
                Debug.LogFormat("{0} STRIKES {1}", hitEntity, byEntity);
                strikeQueue.Add((hitEntity, byEntity, hit, onHit, onBlock));
            }

            public void Throw(InGameEntity hitEntity, InGameEntity byEntity, Hit hit, Action<AttackPriority> onGrabbed) {
                Debug.LogFormat("{0} THROWS {1}", hitEntity, byEntity);
                throwQueue.Add((hitEntity, byEntity, hit, onGrabbed));
            }

            public void Grabs(InGameEntity byEntity, InGameEntity grabbedEntity, bool breakable) {
                entityGrabbingEntity[byEntity] = grabbedEntity;
                entityGrabbingEntityContinue[byEntity] = grabbedEntity;
                entityGrabbingEntityBreakable[byEntity] = breakable;

                byEntity.Attach(grabbedEntity);
                grabbedEntity.PredeterminedActions("grabbed");
            }

            public void Grabs(InGameEntity byEntity, InGameEntity grabbedEntity, bool breakable, Vector3 finalGrabbedPosition) {
                entityGrabbingEntity[byEntity] = grabbedEntity;
                entityGrabbingEntityContinue[byEntity] = grabbedEntity;
                entityGrabbingEntityBreakable[byEntity] = breakable;

                byEntity.Attach(grabbedEntity);
                grabbedEntity.PredeterminedActions("grabbed", finalGrabbedPosition);
            }

            public void MaintainsGrab(InGameEntity byEntity, InGameEntity grabbedEntity) {
                if (!entityGrabbingEntity.ContainsKey(byEntity)) {
                    Debug.LogError("Attempting to maintain a grab between 2 entities no longer grabbing");
                    Debug.LogErrorFormat("{0} continues grabbing {1}", byEntity, grabbedEntity);
                }
                entityGrabbingEntityContinue[byEntity] = grabbedEntity;
            }

            public void SetGrabBreakability(InGameEntity byEntity, bool breakable) {
                entityGrabbingEntityBreakableNextFrame[byEntity] = breakable;
            }

            public void BreakGrab(InGameEntity grabbedEntity) {
                InGameEntity byEntity = null;
                foreach (KeyValuePair<InGameEntity, InGameEntity> kvp in entityGrabbingEntity) {
                    if (kvp.Value == grabbedEntity) {
                        byEntity = kvp.Key;
                    }
                }

                if (byEntity == null) {
                    throw new Exception("Attempting to break a grab by an entity not being grabbed");
                }

                if (entityGrabbingEntityBreakable[byEntity]) {
                    byEntity.PredeterminedActions("grabBreak");
                    grabbedEntity.PredeterminedActions("grabBreak");
                }
            }

            public void ReleaseGrab(InGameEntity byEntity, InGameEntity grabbedEntity) {
                byEntity.Detach(grabbedEntity);
                //entityGrabbingEntity.Remove(byEntity);
                entityGrabbingDeleteQueue.Add(byEntity);
            }

            public void ReleaseUnmaintainedGrab() {
                foreach (KeyValuePair<InGameEntity, InGameEntity> kvp0 in entityGrabbingEntity) {
                    InGameEntity byEntity = kvp0.Key;
                    if (!entityGrabbingEntityContinue.ContainsKey(byEntity)) {
                        ReleaseGrab(kvp0.Key, kvp0.Value);
                    }
                }

                foreach (InGameEntity byEntity in entityGrabbingDeleteQueue) {
                    entityGrabbingEntity.Remove(byEntity);
                    entityGrabbingEntityBreakable.Remove(byEntity);
                }
                entityGrabbingDeleteQueue.Clear();
            }

            public void HitStunStart(FightingGameCharacter fgChar) {
                gamemode.OnHitStunStart(fgChar);
            }

            public void HitStunEnd(FightingGameCharacter fgChar) {
                gamemode.OnHitStunEnd(fgChar);
            }

            private void ResolveStrikesAndThrows() {
                if (strikeQueue.Count == 0 && throwQueue.Count == 0) {
                    return;
                }

                foreach (KeyValuePair<InGameEntity, bool> kvp in activeEntities) {
                    var active = kvp.Value;
                    if (active) {
                        resolveStrikeBy2[kvp.Key].Clear();
                        resolveThrowBy2[kvp.Key].Clear();
                    }
                }

                FindAllInteractions();
                Dictionary<InGameEntity, Dictionary<ResolveInteractionType, List<ResolveInteraction>>> finalResolveMap = CullInvalidInteractions();

                foreach (KeyValuePair<InGameEntity, Dictionary<ResolveInteractionType, List<ResolveInteraction>>> kvp0 in finalResolveMap) {
                    foreach (KeyValuePair<ResolveInteractionType, List<ResolveInteraction>> kvp1 in kvp0.Value) {
                        switch (kvp1.Key) {
                            case ResolveInteractionType.Strike:
                                InteractionStrike(kvp1.Value);
                                break;
                            case ResolveInteractionType.StrikeEachOther:
                                //InteractionStrikeEachOther(kvp1.Value);// Pretty sure I need to figure out the data between the 2 simultaneous strikes, even if I only call 1 at a time.
                                break;
                            case ResolveInteractionType.StrikeWhileThrow:
                                break;
                            case ResolveInteractionType.Throw:
                                InteractionThrow(kvp1.Value);
                                break;
                            case ResolveInteractionType.ThrowEachOther:
                                break;
                            case ResolveInteractionType.ThrowWhileStrike:
                                throw new InvalidOperationException("An entity may not throw another while it is getting struck");
                        }
                    }
                }

                strikeQueue.Clear();
                throwQueue.Clear();
            }

            private void FindAllInteractions() {
                resolveInteractions.Clear();

                for (int n = 0; n < strikeQueue.Count; ++n) {
                    InGameEntity hitEntity = strikeQueue[n].hitEntity;
                    InGameEntity byEntity = strikeQueue[n].byEntity;

                    resolveStrikeBy2[hitEntity].Add((hitEntity, byEntity, strikeQueue[n]));
                }

                for (int n = 0; n < throwQueue.Count; ++n) {
                    InGameEntity hitEntity = throwQueue[n].hitEntity;
                    InGameEntity byEntity = throwQueue[n].byEntity;

                    resolveThrowBy2[hitEntity].Add((hitEntity, byEntity, throwQueue[n]));
                }

                //strikeQueue.Clear();  // These can be cleared here.
                //throwQueue.Clear();

                foreach (KeyValuePair<InGameEntity, List<(InGameEntity hitEntity, InGameEntity byEntity, object hitQueueObj)>> kvp in resolveStrikeBy2) {
                    foreach ((InGameEntity hitEntity, InGameEntity byEntity, object hitQueueObj) strike in kvp.Value) {

                        if (kvp.Key != strike.hitEntity) {
                            throw new Exception("Hit Entities don't match in resolve map");
                        }

                        InGameEntity hitEntity = kvp.Key;
                        InGameEntity byEntity = strike.byEntity;

                        if (ActionOnEachOther(hitEntity, byEntity, resolveStrikeBy2)) {
                            resolveInteractions.Add(new ResolveInteraction {
                                type = ResolveInteractionType.StrikeEachOther,
                                byEntity = byEntity,
                                hitEntity = hitEntity,
                                hitQueueObject = strike.hitQueueObj,
                            });
                        }
                        else if (ActionOnEachOther(hitEntity, byEntity, resolveThrowBy2)) {
                            resolveInteractions.Add(new ResolveInteraction {
                                type = ResolveInteractionType.StrikeWhileThrow,
                                byEntity = byEntity,
                                hitEntity = hitEntity,
                                hitQueueObject = strike.hitQueueObj,
                            });
                        }
                        else {
                            resolveInteractions.Add(new ResolveInteraction {
                                type = ResolveInteractionType.Strike,
                                byEntity = byEntity,
                                hitEntity = hitEntity,
                                hitQueueObject = strike.hitQueueObj,
                            });
                        }
                    }
                }

                foreach (KeyValuePair<InGameEntity, List<(InGameEntity hitEntity, InGameEntity byEntity, object hitQueueObj)>> kvp in resolveThrowBy2) {
                    foreach ((InGameEntity hitEntity, InGameEntity byEntity, object hitQueueObj) @throw in kvp.Value) {

                        if (kvp.Key != @throw.hitEntity) {
                            throw new Exception("Hit Entities don't match in resolve map");
                        }

                        InGameEntity hitEntity = kvp.Key;
                        InGameEntity byEntity = @throw.byEntity;

                        if (ActionOnEachOther(hitEntity, byEntity, resolveThrowBy2)) {
                            resolveInteractions.Add(new ResolveInteraction {
                                type = ResolveInteractionType.ThrowEachOther,
                                byEntity = byEntity,
                                hitEntity = hitEntity,
                                hitQueueObject = @throw.hitQueueObj,
                            });
                        }
                        else if (ActionOnEachOther(hitEntity, byEntity, resolveStrikeBy2)) {
                            resolveInteractions.Add(new ResolveInteraction {
                                type = ResolveInteractionType.ThrowWhileStrike,
                                byEntity = byEntity,
                                hitEntity = hitEntity,
                                hitQueueObject = @throw.hitQueueObj,
                            });
                        }
                        else {
                            resolveInteractions.Add(new ResolveInteraction {
                                type = ResolveInteractionType.Throw,
                                byEntity = byEntity,
                                hitEntity = hitEntity,
                                hitQueueObject = @throw.hitQueueObj,
                            });
                        }
                    }
                }
            }

            private bool ActionOnEachOther(InGameEntity hitEntity, InGameEntity byEntity, Dictionary<InGameEntity, List<(InGameEntity hitEntity, InGameEntity byEntity, object hitQueueObj)>> resolveMap) {
                if (resolveMap.TryGetValue(byEntity, out List<(InGameEntity hitEntity, InGameEntity byEntity, object hitQueueObj)> byEntities)) {
                    foreach ((InGameEntity hitEntity, InGameEntity byEntity, object hitQueueObj) tuple in byEntities) {
                        if (hitEntity == tuple.byEntity) {
                            return true;
                        }
                    }
                }

                return false;
            }

            private Dictionary<InGameEntity, Dictionary<ResolveInteractionType, List<ResolveInteraction>>> CullInvalidInteractions() {
                Dictionary<InGameEntity, Dictionary<ResolveInteractionType, List<ResolveInteraction>>> finalResolveMap =
                    resolveInteractions
                        .GroupBy(interaction => interaction.hitEntity)
                        .ToDictionary(group0 => group0.Key, group0 => group0
                            .GroupBy(interaction => interaction.type)
                            .ToDictionary(group1 => group1.Key, group1 => group1
                                .ToList()));

                foreach (KeyValuePair<InGameEntity, Dictionary<ResolveInteractionType, List<ResolveInteraction>>> kvp in finalResolveMap) {
                    Dictionary<ResolveInteractionType, List<ResolveInteraction>> interactions = kvp.Value;

                    if (   interactions.ContainsKey(ResolveInteractionType.Strike)
                        || interactions.ContainsKey(ResolveInteractionType.StrikeEachOther)
                        || interactions.ContainsKey(ResolveInteractionType.StrikeWhileThrow)) {
                        if (interactions.ContainsKey(ResolveInteractionType.Throw))            interactions[ResolveInteractionType.Throw].Clear();
                        if (interactions.ContainsKey(ResolveInteractionType.ThrowWhileStrike)) interactions[ResolveInteractionType.ThrowWhileStrike].Clear();
                        if (interactions.ContainsKey(ResolveInteractionType.ThrowEachOther))   interactions[ResolveInteractionType.ThrowEachOther].Clear();
                    }
                }

                return finalResolveMap;
            }

            private void InteractionStrike(List<ResolveInteraction> interactions) {
                InGameEntity ent0 = interactions[0].hitEntity;

                if (IsCurrentFGChar(ent0)) {
                    FightingGameCharacter fgChar = (FightingGameCharacter)ent0;
                    if (fgChar.GetCharacterVulnerability().strikable) {
                        IEnumerable<(InGameEntity hitEntity, InGameEntity byEntity, Hit hit, Action<AttackPriority, int> onHit, Action<AttackPriority, int> onBlock)> relevantTuples = interactions
                            .Select(interaction => interaction.hitQueueObject)
                            .Cast<(InGameEntity hitEntity, InGameEntity byEntity, Hit hit, Action<AttackPriority, int> onHit, Action<AttackPriority, int> onBlock)>();
                        bool hitSuccessful = relevantTuples
                            .Any(strikeQueueObj => !fgChar.CheckBlockSuccess(strikeQueueObj.hit));

                        if (hitSuccessful) {
                            foreach ((InGameEntity hitEntity, InGameEntity byEntity, Hit hit, Action<AttackPriority, int> onHit, Action<AttackPriority, int> onBlock) tuple in relevantTuples) {
                                IncrementComboCounter(fgChar);
                                tuple.onHit?.Invoke(AttackPriority.None, GetComboScaleDamage(tuple.hitEntity, tuple.hit.comboScaling, tuple.hit.hitDamage));
                            }
                        }
                        else {
                            foreach ((InGameEntity hitEntity, InGameEntity byEntity, Hit hit, Action<AttackPriority, int> onHit, Action<AttackPriority, int> onBlock) tuple in relevantTuples) {
                                tuple.onBlock?.Invoke(AttackPriority.None, tuple.hit.blockDamage);
                            }
                        }
                    }
                }
                else {
                    IEnumerable<(InGameEntity hitEntity, InGameEntity byEntity, Hit hit, Action<AttackPriority, int> onHit, Action<AttackPriority, int> onBlock)> relevantTuples = interactions
                        .Select(interaction => interaction.hitQueueObject)
                        .Cast<(InGameEntity hitEntity, InGameEntity byEntity, Hit hit, Action<AttackPriority, int> onHit, Action<AttackPriority, int> onBlock)>();
                    foreach ((InGameEntity hitEntity, InGameEntity byEntity, Hit hit, Action<AttackPriority, int> onHit, Action<AttackPriority, int> onBlock) tuple in relevantTuples) {
                        //tuple.onHit?.Invoke(AttackPriority.None, GetComboScale(tuple.hitEntity, tuple.hit.comboScaling, tuple.hit.hitDamage));
                        // I think I'll have strikes on other entities not have combo scaling.
                        tuple.onHit?.Invoke(AttackPriority.None, GetComboScaleDamage(tuple.hitEntity, tuple.hit.comboScaling, tuple.hit.hitDamage));
                    }
                }
            }

            //private void InteractionStrikeEachOther(List<ResolveInteraction> interactions) {
            //    // old version, I think I don't want to call both callbacks at the same time anymore.
            //    //foreach (KeyValuePair<InGameEntity, InGameEntity> kvp in resolveStrikeBy) {
            //    //    var hitEntity = kvp.Key;
            //
            //    //    if (ActionOnEachOther(hitEntity, resolveStrikeBy2)) {
            //    //        // TODO: Fix this case just like the other one.
            //    //        InGameEntity ent0 = hitEntity;
            //    //        InGameEntity ent1 = resolveStrikeBy[hitEntity];
            //
            //    //        (InGameEntity hitEntity, InGameEntity byEntity, Hit hit, Action<AttackPriority, int> onHit, Action<AttackPriority, int> onBlock) info0 = strikeQueue.Single(tuple => tuple.hitEntity == ent0);
            //    //        (InGameEntity hitEntity, InGameEntity byEntity, Hit hit, Action<AttackPriority, int> onHit, Action<AttackPriority, int> onBlock) info1 = strikeQueue.Single(tuple => tuple.hitEntity == ent1);
            //
            //    //        if (IsCurrentFGChar(info0.hitEntity)) {
            //    //            InGameEntity fgChar = info0.hitEntity;
            //    //            int prevNumHits = numComboHits[fgChar];
            //    //            numComboHits[fgChar]++;
            //    //            gamemode.OnGameEntityNumHitsChange(fgChar, numComboHits[fgChar], prevNumHits);
            //
            //    //            comboScalingIndex[fgChar]++;
            //    //        }
            //    //        if (IsCurrentFGChar(info1.hitEntity)) {
            //    //            InGameEntity fgChar = info1.hitEntity;
            //    //            int prevNumHits = numComboHits[fgChar];
            //    //            numComboHits[fgChar]++;
            //    //            gamemode.OnGameEntityNumHitsChange(fgChar, numComboHits[fgChar], prevNumHits);
            //
            //    //            comboScalingIndex[fgChar]++;
            //    //        }
            //
            //    //        info0.onHit?.Invoke(info1.hit.priority, GetComboScaleDamage(info0.hitEntity, info0.hit.comboScaling, info0.hit.hitDamage));
            //    //        info1.onHit?.Invoke(info0.hit.priority, GetComboScaleDamage(info1.hitEntity, info1.hit.comboScaling, info1.hit.hitDamage));
            //    //    }
            //    //}
            //}
            
            private void InteractionThrow(List<ResolveInteraction> interactions) {
                InGameEntity ent0 = interactions[0].hitEntity;

                bool runCallback = false;
                if (IsCurrentFGChar(ent0)) {
                    FightingGameCharacter fgChar = (FightingGameCharacter)ent0;
                    if (fgChar.GetCharacterVulnerability().throwable) {
                        runCallback = true;
                    }
                }
                else {
                    runCallback = true;
                }

                if (runCallback) {
                    IEnumerable<(InGameEntity hitEntity, InGameEntity byEntity, Hit hit, Action<AttackPriority> onGrabbed)> relevantTuples = interactions
                            .Select(interaction => interaction.hitQueueObject)
                            .Cast<(InGameEntity hitEntity, InGameEntity byEntity, Hit hit, Action<AttackPriority> onGrabbed)>();

                    foreach ((InGameEntity hitEntity, InGameEntity byEntity, Hit hit, Action<AttackPriority> onGrabbed) tuple in relevantTuples) {
                        tuple.onGrabbed?.Invoke(AttackPriority.None);
                    }
                }
            }

            private void FrameUpdate(int frameIndex) {
                ResetComboCounter();
                ResolveStrikesAndThrows();
                ReleaseUnmaintainedGrab();

                entityGrabbingEntityContinue.Clear();
            }

            private void LateFrameUpdate(int frameIndex) {
                foreach (InGameEntity entity in entities) {
                    prevComboState[entity] = entity.GetComboState();
                }

                foreach (KeyValuePair<InGameEntity, bool> kvp in entityGrabbingEntityBreakableNextFrame) {
                    InGameEntity byEntity = kvp.Key;
                    entityGrabbingEntityBreakable[byEntity] = kvp.Value;
                }
                entityGrabbingEntityBreakableNextFrame.Clear();
            }

            private List<float> comboScaling = new List<float> { 1.0f, 0.8f, 0.65f, 0.5f, 0.45f, 0.4f, 0.35f, 0.3f, 0.25f };

            public int GetComboScaleDamage(InGameEntity hitEntity, float initComboScaling, int hitDamage) {
                if (numComboHits[hitEntity] == 0) {
                    for (int n = 0; n < comboScaling.Count; ++n) {
                        if (comboScaling[n] <= initComboScaling) {
                            comboScalingIndex[hitEntity] = n;
                        }
                    }
                }
                if (comboScalingIndex[hitEntity] >= comboScaling.Count) {
                    return (int)(hitDamage * comboScaling[comboScaling.Count - 1]);
                }
                else {
                    return (int)(hitDamage * comboScaling[comboScalingIndex[hitEntity]]);
                }
            }

            private void ResetComboCounter() {
                playerService.ForEach((fgChar, id) => {
                    if (fgChar.GetComboState() == ComboState.None && prevComboState[fgChar] == ComboState.InCombo) {
                        int prevNumHits = numComboHits[fgChar];
                        numComboHits[fgChar] = 0;
                        comboScalingIndex[fgChar] = 0;

                        gamemode.OnGameEntityNumHitsChange(fgChar, 0, prevNumHits);
                    }
                });
            }

            public void IncrementComboCounter(FightingGameCharacter fgChar) {
                numComboHits[fgChar]++;
                int prevNumHits = numComboHits[fgChar];
                comboScalingIndex[fgChar]++;

                gamemode.OnGameEntityNumHitsChange(fgChar, numComboHits[fgChar], prevNumHits);
            }

            public void LoadMainMenu() {
                Debug.Log("LoadMainMenu called");
                SceneManager.LoadScene("Scenes/Menu/MainMenu2");
            }

            public int GetMaxPlayers() {
                return gamemode.GetMaxPlayers();
            }

            public int GetNumHits(InGameEntity fgChar) {
                return numComboHits[fgChar];
            }

            public void DisableControl() {
                playerService.ForEach((fgChar, id) => {
                    fgChar.SetControlEnable(false);
                });
            }

            public void EnableControl() {
                playerService.ForEach((fgChar, id) => {
                    fgChar.SetControlEnable(true);
                });
            }

            public void OpeningCamera() {
                gamemode.CameraEnable(false);
                mapCamera.SetActive(true);
            }

            public void FightingGameCamera() {
                mapCamera.SetActive(false);
                gamemode.CameraEnable(true);
            }

            public Transform GetSpawnPoint() {
                return spawnPoint;
            }

            public float GetSpawnPointOffset() {
                return offset;
            }

            public Vector2 ScreenOrientation(FightingGameCharacter fgChar) {
                return gamemode.ScreenOrientation(fgChar);
            }

            public Transform GetCameraStart() {
                return cameraStart;
            }

            public List<Transform> GetLevelBoundaries() {
                return levelBoundaries;
            }

            public void SetGameTimeScaling(float scaling) {
                gamemode.SetGameTimeScaling(scaling);
            }

            public float GetGameTimeScaling() {
                return gamemode.GetGameTimeScaling();
            }
        }
    }
}