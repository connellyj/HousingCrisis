using UnityEngine;

public class Mansion : House {

    protected override void Awake() {
        base.Awake();
    }

    protected override void ActivateAbility() {
        Debug.Log("MANSION ACTIVATED");
    }
}
