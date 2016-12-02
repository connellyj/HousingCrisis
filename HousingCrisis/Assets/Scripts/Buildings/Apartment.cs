using UnityEngine;

public class Apartment : House {

    protected override void Awake() {
        base.Awake();
        spriteWrapper.transform.position += new Vector3(0,0.15f,0);
    }

    protected override float ChewTimeOffset() {
        return chewingTime / 2f;
    }
}