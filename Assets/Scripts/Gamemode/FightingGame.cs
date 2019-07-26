using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightingGame : MonoBehaviour {

    public GameObject char0;
    public GameObject char1;

    public new GameObject camera;

    // Start is called before the first frame update
    void Start() {
        char0.GetComponent<FightingGameCharacter>().opponentCharacter = char1;
        char1.GetComponent<FightingGameCharacter>().opponentCharacter = char0;
    }

    // Update is called once per frame
    void Update() {
        
    }
}
