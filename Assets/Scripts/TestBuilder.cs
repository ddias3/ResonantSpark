using ResonantSpark.Builder;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBuilder : MonoBehaviour {
    public GameObject male0Builder;

    public void Start() {
        ICharacterBuilder builder = male0Builder.GetComponent<ICharacterBuilder>();
        builder.Init();
        builder.CreateCharacter();
    }
}
