using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityStandardAssets.Characters.ThirdPerson;

public class DisablePlayer : MonoBehaviour {

    private ThirdPersonUserControl userControl;
    public void Start() {
        userControl = gameObject.GetComponent<ThirdPersonUserControl>();
    }

    public void Update() {
        if (Keyboard.current.backspaceKey.wasPressedThisFrame) {
            userControl.enabled = !userControl.enabled;
        }
    }
}
