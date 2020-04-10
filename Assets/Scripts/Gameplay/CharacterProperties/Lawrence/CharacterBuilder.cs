using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Builder;
using ResonantSpark.Gameplay;
using ResonantSpark.Service;
using ResonantSpark.Character;
using ResonantSpark.Utility;

namespace ResonantSpark {
    namespace CharacterProperties {
        namespace Lawrence {
            public class CharacterBuilder : MonoBehaviour, ICharacterBuilder {

                public GameObject lawrencePrefab;

                public List<MaterialMapper> charColors;

                private AudioClipMap audioMap;
                private ParticleEffectMap particleMap;
                private AnimationCurveMap attackMovementMap;
                private ProjectileMap projectileMap;

                private FightingGameCharacter fgChar;

                public FightingGameCharacter CreateCharacter(AllServices services) {
                    GameObject character = Instantiate<GameObject>(lawrencePrefab, transform.position, transform.rotation);
                    character.name = lawrencePrefab.name;
                    fgChar = character.GetComponent<FightingGameCharacter>();

                    return fgChar;
                }

                public void Build(AllServices services) {
                    LawrenceBuilder charBuilder = new LawrenceBuilder(services);

                    audioMap = GetComponent<AudioClipMap>();
                    particleMap = GetComponent<ParticleEffectMap>();
                    projectileMap = GetComponent<ProjectileMap>();

                    Attacks attacks = new Attacks(services,
                        audioMap.BuildDictionary(),
                        particleMap.BuildDictionary(),
                        projectileMap.BuildDictionary());
                    Movement movement = new Movement(services);

                    charBuilder.Version("0.1");
                    attacks.Init(charBuilder, fgChar);
                    movement.Init(charBuilder, fgChar);

                    charBuilder.SetUpCharacter(charColors, fgChar);

                    //Destroy(gameObject);
                }

                private class LawrenceBuilder : CharacterPropertiesBuilder {

                    public LawrenceBuilder(AllServices services) : base(services) {
                        // do nothing
                    }

                    public void SetUpCharacter(List<MaterialMapper> charColors, FightingGameCharacter fgChar) {
                        CharacterData charData = ScriptableObject.CreateInstance<CharacterData>();
                        charData.Init(this.maxHealth);

                        BuildAttacks(charData);
                        //BuildMovement(charData);

                        fgChar.Init(charData);

                        fgChar.SetColorMapper(charColors[fgChar.id % 3]);
                    }
                }
            }
        }
    }
}