using UnityEngine;
using UnityEngine.UI;

using ResonantSpark.Builder;
using ResonantSpark.Input;
using ResonantSpark.Service;
using ResonantSpark.Gameplay;

public class TestBuilder : MonoBehaviour {
    public BuildService buildService;
    public GameObject male0Builder0;
    public GameObject male0Builder1;

    public Text charVelocity;
    public InputBuffer inputBuffer;

    public void Start() {
        FightingGameCharacter char0 = buildService.Build(male0Builder0.GetComponent<ICharacterBuilder>());
        FightingGameCharacter char1 = buildService.Build(male0Builder1.GetComponent<ICharacterBuilder>());

        char0.charVelocity = charVelocity;
        char0.SetInputBuffer(inputBuffer);

        EventManager.TriggerEvent<ResonantSpark.Events.FightingGameCharsReady>();
    }
}
