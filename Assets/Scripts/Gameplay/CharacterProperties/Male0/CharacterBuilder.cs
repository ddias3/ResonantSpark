using System;
using System.Collections.Generic;
using UnityEngine;

using ResonantSpark.Builder;
using ResonantSpark.Character;
using ResonantSpark.Gameplay;

namespace ResonantSpark {
    namespace CharacterProperties {
        namespace Male0 {
            public class CharacterBuilder : MonoBehaviour, ICharacterBuilder {

                public GameObject modelAnimatorPrefab;
                public RuntimeAnimatorController animatorController;
                public GameObject charStateMachine;

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
                    FightingGameCharacter fgChar = charBuilder.CreateCharacter(gameObject);
                    GameObject stateMachine = Instantiate<GameObject>(charStateMachine, gameObject.transform, false);
                    GameObject mesh = Instantiate<GameObject>(modelAnimatorPrefab, gameObject.transform, false);
                    mesh.GetComponent<Animator>().runtimeAnimatorController = animatorController;

                    Rigidbody rigidbody = gameObject.AddComponent<Rigidbody>();
                    CapsuleCollider positionCollider = gameObject.AddComponent<CapsuleCollider>();
                    CharacterStates.Init stmInit = gameObject.AddComponent<CharacterStates.Init>();
                    Destroy(this);
                    return gameObject;
                }

                private class Male0Builder : CharacterPropertiesBuilder {

                    public FightingGameCharacter CreateCharacter(GameObject gameObject) {
                        return gameObject.AddComponent<FightingGameCharacter>();
                    }
                }
            }
        }
    }
}