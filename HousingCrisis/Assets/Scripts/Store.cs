using UnityEngine;

public class Store : House {

    protected override void Awake() {
        base.Awake();
    }

    protected override void ActivateAbility() {
        Debug.Log("STORE ACTIVATED");
    }
}