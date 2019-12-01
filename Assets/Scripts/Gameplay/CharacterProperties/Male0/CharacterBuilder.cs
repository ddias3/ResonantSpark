using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Builder;
using ResonantSpark.Character;
using ResonantSpark.Gameplay;

namespace ResonantSpark {
    namespace CharacterProperties {
        namespace Male0 {
            public class CharacterBuilder : MonoBehaviour {

                public GameObject modelAnimatorPrefab;
                public GameObject charStateMachine;

                private CharacterPropertiesBuilder charBuilder;

                public void Init() {
                    charBuilder = new Male0Builder();

                    Attacks attacks = new Attacks();
                    Movement movement = new Movement();

                    charBuilder.Version("1.0");
                    attacks.Init(charBuilder);
                    movement.Init(charBuilder);
                }

                public FightingGameCharacter CreateCharacter() {
                    return charBuilder.CreateCharacter();
                }

                private class Male0Builder : CharacterPropertiesBuilder {
                    public new ICharacterPropertiesBuilder Attack(Attack attack) {
                        return this;
                    }
                }
            }
        }
    }
}