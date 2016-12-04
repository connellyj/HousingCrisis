﻿using UnityEngine;

public class PersonSpawner : MonoBehaviour {

	public float spawnChance;
    public float robberChance;
    public float soldierChance;
    public float policeChance;
    public float bankerChance;
	private int spawnCounter = 0; 

	public GameObject manPrefab;
    public GameObject womanPrefab;
    public GameObject robberPrefab;
    public GameObject soldierPrefab;
    public GameObject policePrefab;
    public GameObject bankerPrefab;
	
    // Spawns people randomly based on provided chances
	void Update () {
		spawnCounter++;
		if (spawnCounter == 60)
		{
			spawnCounter = 0;
			if (spawnChance > Random.value) {
                float rand = Random.value;
                if(rand < policeChance) {
                    Instantiate(policePrefab, transform.position + Person.positionOffset, Quaternion.identity);
                } else if(rand < policeChance + bankerChance) {
                    Instantiate(bankerPrefab, transform.position + Person.positionOffset, Quaternion.identity);
                } else if(rand < soldierChance + policeChance + bankerChance && HouseManager.houses.Count > 0) {
                    Instantiate(soldierPrefab, transform.position + Person.positionOffset, Quaternion.identity);
                } else if(rand < soldierChance + robberChance + policeChance + bankerChance && HouseManager.houses.Count > 0 && HouseManager.AnyHousesNotBurning()) {
                    Instantiate(robberPrefab, transform.position + Person.positionOffset, Quaternion.identity);
                } else {
                    float genderRoll = Random.value;
                    if (genderRoll < .5)
                    {
                        Instantiate(manPrefab, transform.position + Person.positionOffset, Quaternion.identity);
                    } else {
                        Instantiate(womanPrefab, transform.position + Person.positionOffset, Quaternion.identity);
                    }
                }
			}
		}
	}
}
