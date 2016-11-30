using UnityEngine;

public class Bank : House {

    protected override void Awake() {
        base.Awake();
    }

    protected override void ActivateAbility() {
        Debug.Log("BANK ACTIVATED");
    }
}
