﻿using System;
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

                private FightingGameCharacter fgChar;

                public FightingGameCharacter CreateCharacter(AllServices services) {
                    GameObject character = Instantiate<GameObject>(male0Prefab, transform.position, transform.rotation);
                    character.name = male0Prefab.name;
                    fgChar = character.GetComponent<FightingGameCharacter>();

                    return fgChar;
                }

                public void Build(AllServices services) {
                    Male0Builder charBuilder = new Male0Builder(services);

                    Attacks attacks = ScriptableObject.CreateInstance<Attacks>(); //new Attacks();
                    Movement movement = ScriptableObject.CreateInstance<Movement>(); //new Movement();

                    charBuilder.Version("0.1");
                    attacks.Init(charBuilder, fgChar);
                    movement.Init(charBuilder, fgChar);

                    charBuilder.SetUpCharacter(fgChar);

                    Destroy(gameObject);
                }

                private class Male0Builder : CharacterPropertiesBuilder {

                    public Male0Builder(AllServices services) : base(services) {
                        // do nothing
                    }

                    public void SetUpCharacter(FightingGameCharacter fgChar) {
                        CharacterData charData = new CharacterData(this.maxHealth);

                        BuildAttacks(charData);
                        //BuildMovement(charData);

                        fgChar.Init(charData);
                    }
                }
            }
        }
    }
}