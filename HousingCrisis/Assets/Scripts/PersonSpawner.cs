using UnityEngine;
using System.Collections;

public class PersonSpawner : MonoBehaviour {

	public float spawnChance;
	private int spawnCounter = 0; 

	public GameObject manPrefab;

	void Start () {
	
	}
	
	void Update () {
		spawnCounter++;
		if (spawnCounter == 60)
		{
			spawnCounter = 0;
			if (spawnChance > Random.value) {
				manPrefab.transform.position = transform.position + Person.positionOffset;
				Instantiate(manPrefab);
			}
		}
	}

}
