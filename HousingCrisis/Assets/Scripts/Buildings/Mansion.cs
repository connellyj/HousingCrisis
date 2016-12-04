using UnityEngine;

public class Mansion : House {

	public static int sprinklerStrength = 100;

    protected override void Awake() {
        base.Awake();
        spriteWrapper.transform.position += new Vector3(0,0.1f,0);
        HouseManager.UpdateSprinklers();
    }

    protected override void Start() {
    	base.Start();
    	HouseManager.UpdateSprinklers();
        hasSprinklers = true;
        waterDrop.SetActive(true);
    }

    // Removes and destroys the mansion
    protected override void RemoveHouse() {
		HouseManager.RemoveHouse(this);
		HouseManager.UpdateSprinklers();
        Destroy(gameObject);
    }

    /* 
    mansions give adjacent houses a bonus to health, show water drop in cornerwhen not burning
    boolean sprinkler tells any building whether it has a mansion nearby
    updates when a mansion is build or burns down.

    calculate adjacent mansions runs in HouseManager and sets the sprinkler boolean for each non-mansion building
    	this occurs whenever a mansion is built or destoyed 
    	or another building is built (but then just for that building)

    sprinklers should lower the minimum totalDamage of buildings to 0 - Mansion.sprinklerStrength
    this is handled with an additional burnState = -1 which is indicated to the player with the waterDrop icon
    this should automatically adjust totalDamage dynamically when mansions are created/destroyed or other buildings are created in range
    */
}