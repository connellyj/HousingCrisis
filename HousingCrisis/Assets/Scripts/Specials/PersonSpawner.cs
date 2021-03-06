﻿using UnityEngine;
using System.Collections;

public class PersonSpawner : MonoBehaviour {

    public float spawnChance;
    public float robberChance;
    public float soldierChance;
    public float policeChance;
    public float bankerChance;

	public GameObject manPrefab;
    public GameObject womanPrefab;
    public GameObject robberPrefab;
    public GameObject soldierPrefab;
    public GameObject policePrefab;
    public GameObject bankerPrefab;

    void Start() {
        StartCoroutine(SpawnPeople());
    }

    private IEnumerator SpawnPeople() {
        while(true) {
            yield return new WaitForSeconds(1);
            if(spawnChance > Random.value) {
                float rand = Random.value;
                if(rand < robberChance) {
                    if(HouseManager.houses.Count > 0 && HouseManager.AnyHousesNotBurning()) {
                        Instantiate(robberPrefab, transform.position + Person.positionOffset, Quaternion.identity);
                        continue;
                    }
                } else if(rand < policeChance + robberChance) {
                    if(HouseManager.houses.Count > 0) {
                        Instantiate(policePrefab, transform.position + Person.positionOffset, Quaternion.identity);
                        continue;
                    }
                } else if(rand < soldierChance + policeChance + robberChance) {
                    if(HouseManager.houses.Count > 0) {
                        Instantiate(soldierPrefab, transform.position + Person.positionOffset, Quaternion.identity);
                        continue;
                    }
                } else if(rand < bankerChance + soldierChance + policeChance + robberChance) {
                    Instantiate(bankerPrefab, transform.position + Person.positionOffset, Quaternion.identity);
                    continue;
                }
                SpawnPedestrian();
            }
        }
    }

    private void SpawnPedestrian() {
        float genderRoll = Random.value;
        if (genderRoll < .5) {
            Instantiate(manPrefab, transform.position + Person.positionOffset, Quaternion.identity);
        } else {
            Instantiate(womanPrefab, transform.position + Person.positionOffset, Quaternion.identity);
        }
    }

}
