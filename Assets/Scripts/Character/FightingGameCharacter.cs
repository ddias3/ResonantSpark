using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightingGameCharacter : MonoBehaviour {

    private GameObject otherChar;

    private CharacterMovement charMovement;

    public GameObject opponentCharacter {
        set { otherChar = value; }
    }

    // Start is called before the first frame update
    void Start() {
        charMovement = gameObject.GetComponent<CharacterMovement>();
    }

    void Update() {

    }

    public bool PerformAction(string action) {
        switch (action) {
            case "stepBackward":
                charMovement.MoveAction(CharacterMovement.CharacterMovementEnum.StepBackward, otherChar);
                break;
            case "stepForward":
                charMovement.MoveAction(CharacterMovement.CharacterMovementEnum.StepForward, otherChar);
                break;
            case "stepChest":
                charMovement.MoveAction(CharacterMovement.CharacterMovementEnum.StepChest, otherChar);
                break;
            case "stepSpine":
                charMovement.MoveAction(CharacterMovement.CharacterMovementEnum.StepSpine, otherChar);
                break;
            case "dashBackward":
                charMovement.MoveAction(CharacterMovement.CharacterMovementEnum.DashBackward, otherChar);
                break;
            case "dashForward":
                charMovement.MoveAction(CharacterMovement.CharacterMovementEnum.DashForward, otherChar);
                break;
            case "sideStepChest":
                charMovement.MoveAction(CharacterMovement.CharacterMovementEnum.SideStepChest, otherChar);
                break;
            case "sideStepSpine":
                charMovement.MoveAction(CharacterMovement.CharacterMovementEnum.SideStepSpine, otherChar);
                break;
        }
        return true;
    }

    public bool MoveAction(CharacterMovement.CharacterMovementEnum charMove) {
        return charMovement.MoveAction(charMove, otherChar);
    }
}
