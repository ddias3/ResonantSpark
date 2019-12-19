using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Builder;
using ResonantSpark.Gameplay;

namespace ResonantSpark {
    namespace CharacterProperties {
        namespace Male0 {
            public class CharacterBuilder : MonoBehaviour, ICharacterBuilder {

                public GameObject male0Prefab;

                private Male0Builder charBuilder;

                public void Init() {
                    charBuilder = new Male0Builder();

                    Attacks attacks = new Attacks();
                    Movement movement = new Movement();

                    charBuilder.Version("1.0");
                    attacks.Init(charBuilder);
                    movement.Init(charBuilder);
                }

                public GameObject CreateCharacter() {
                    GameObject character = Instantiate<GameObject>(male0Prefab, transform.position, transform.rotation);

                    //FightingGameCharacter fgChar = charBuilder.CreateCharacter(character);

                    Destroy(gameObject);

                    return character;
                }

                private class Male0Builder : CharacterPropertiesBuilder {

                    public FightingGameCharacter CreateCharacter(GameObject gameObject) {
                        FightingGameCharacter fgChar = gameObject.AddComponent<FightingGameCharacter>();

                        fgChar.Init();

                        return fgChar;
                    }
                }
            }
        }
    }
}