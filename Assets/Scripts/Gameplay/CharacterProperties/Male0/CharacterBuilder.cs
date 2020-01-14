using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Builder;
using ResonantSpark.Gameplay;
using ResonantSpark.Service;
using ResonantSpark.Character;

namespace ResonantSpark {
    namespace CharacterProperties {
        namespace Male0 {
            public class CharacterBuilder : MonoBehaviour, ICharacterBuilder {

                public GameObject male0Prefab;

                public FightingGameService fgService;
                public BuildService buildService;
                public AudioService audioService;

                private Male0Builder charBuilder;

                public GameObject CreateCharacter() {
                    charBuilder = new Male0Builder(
                        new AllServices()
                            .AddServiceAs<IBuildService>(buildService)
                            .AddServiceAs<IFightingGameService>(fgService)
                            .AddServiceAs<IAudioService>(audioService)
                            .AddServiceAs<IHitBoxService>(fgService)
                            .AddServiceAs<IProjectileService>(fgService));

                    Attacks attacks = new Attacks();
                    Movement movement = new Movement();

                    charBuilder.Version("0.1");
                    attacks.Init(charBuilder);
                    movement.Init(charBuilder);

                    GameObject character = Instantiate<GameObject>(male0Prefab, transform.position, transform.rotation);

                    FightingGameCharacter fgChar = charBuilder.SetUpCharacter(character);

                    Destroy(gameObject);

                    return character;
                }

                private class Male0Builder : CharacterPropertiesBuilder {

                    public Male0Builder(AllServices services) : base(services) {
                        // do nothing
                    }

                    public FightingGameCharacter SetUpCharacter(GameObject gameObject) {
                        CharacterData charData = new CharacterData();

                        BuildAttacks(charData);
                        //BuildMovement(charData);

                        FightingGameCharacter fgChar = gameObject.GetComponent<FightingGameCharacter>();

                        fgChar.Init();

                        return fgChar;
                    }
                }
            }
        }
    }
}