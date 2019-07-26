using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class DisablePlayer : MonoBehaviour {

    private ThirdPersonUserControl userControl;
    public void Start() {
        userControl = gameObject.GetComponent<ThirdPersonUserControl>();
    }

    public void Update() {
        if (Input.GetKeyDown(KeyCode.Backspace)) {
            userControl.enabled = !userControl.enabled;
        }
    }
}
