using UnityEngine;

public class Donut : House {

    protected override void Awake() {
        base.Awake();
    }

    protected override void ActivateAbility() {
        Debug.Log("DONUT ACTIVATED");
    }
}