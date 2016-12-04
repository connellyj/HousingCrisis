using UnityEngine;

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

    private int spawnCounter = 0; 

	void Update () {
		spawnCounter++;
		if (spawnCounter == 60)
		{
			spawnCounter = 0;
			if (spawnChance > Random.value) {
                float rand = Random.value;
                if (rand < robberChance) {
                    if (HouseManager.houses.Count > 0 && HouseManager.AnyHousesNotBurning()) {
                        Instantiate(robberPrefab, transform.position + Person.positionOffset, Quaternion.identity);
                        return;
                    }
                } else if(rand < policeChance + robberChance) {
                    Instantiate(policePrefab, transform.position + Person.positionOffset, Quaternion.identity);
                    return;
                } else if(rand < soldierChance + policeChance + robberChance) {
                    if (HouseManager.houses.Count > 0) {
                        Instantiate(soldierPrefab, transform.position + Person.positionOffset, Quaternion.identity);
                        return;
                    }
                } else if(rand < bankerChance + soldierChance + policeChance + robberChance) {
                    Instantiate(bankerPrefab, transform.position + Person.positionOffset, Quaternion.identity);
                    return;
                }
                SpawnPedestrian();
                return;
            }
        }
    }

    private void SpawnPedestrian()
    {
        float genderRoll = Random.value;
        if (genderRoll < .5)
        {
            Instantiate(manPrefab, transform.position + Person.positionOffset, Quaternion.identity);
        } else {
            Instantiate(womanPrefab, transform.position + Person.positionOffset, Quaternion.identity);
        }
    }

}
