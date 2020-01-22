using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestHitBox : MonoBehaviour {

    private LayerMask hurtBox;
    private LayerMask hitBox;

    private void Awake() {
        hurtBox = LayerMask.NameToLayer("HurtBox");
        hitBox = LayerMask.NameToLayer("HitBox");
    }

    private void OnTriggerEnter(Collider other) {
        Debug.Log("On Trigger Enter Called from TestHitBox.cs");

        if (other.gameObject.layer == hurtBox) {
            Debug.Log("Hitbox hit a hurtbox");
        }
        else if (other.gameObject.layer == hitBox) {
            Debug.Log("Hitbox hit other hitbox");
        }
    }
}
