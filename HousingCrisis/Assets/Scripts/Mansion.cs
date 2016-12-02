using UnityEngine;

public class Mansion : House {

    protected override void Awake() {
        base.Awake();
        transform.position += new Vector3(0,0.1f,0);
    }

    /* 
    mansions give adjacent houses a bonus to health, show water drop in cornerwhen not burning
    boolean sprinkler tells any building whether it has a mansion nearby
    updates when a mansion is build or burns down.

    calculate adjacent mansions runs in HouseManager and sets the sprinkler boolean for each non-mansion building
    this occurs whenever a mansion is built or destoyed

    mansions always have the sprinkler property on
    */

}
