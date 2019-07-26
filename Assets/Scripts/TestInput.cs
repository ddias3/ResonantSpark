using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInput : MonoBehaviour {

    public FightingGameCharacter charMovement;

    public InputBuffer inputBuffer;

    private int horizontalInput = 0;
    private int verticalInput = 0;

    // Update is called once per frame
    void Update() {
        horizontalInput = 0;
        verticalInput = 0;

        if (Input.GetKey(KeyCode.W)) {
            //charMovement.MoveAction(CharacterMovement.CharacterMovementEnum.SideStepChest);
            verticalInput += 1;
        }

        if (Input.GetKey(KeyCode.S)) {
            //charMovement.MoveAction(CharacterMovement.CharacterMovementEnum.SideStepSpine);
            verticalInput += -1;
        }

        if (Input.GetKey(KeyCode.A)) {
            //charMovement.MoveAction(CharacterMovement.CharacterMovementEnum.DashBackward);
            horizontalInput += -1;
        }

        if (Input.GetKey(KeyCode.D)) {
            //charMovement.MoveAction(CharacterMovement.CharacterMovementEnum.DashForward);
            horizontalInput += 1;
        }

        inputBuffer.AddInput((InputBuffer.FightingGameInputCodeDir) ((verticalInput + 1) * 3 + (horizontalInput + 1) + 1));
    }
}
