using UnityEngine;
using System.Collections;

public class DifficultyManager : MonoBehaviour {

	public int progressCount = 0;
	public int wantedLevel = 0;
	public int wantedLevelCap = 3;
	private static int maxWantedLevel = 3;

	public int[] progressThreshholds = new int[maxWantedLevel];

	public float[] spawnChances = new float[maxWantedLevel + 1];
	public float[] robberChances = new float[maxWantedLevel + 1];
	public float[] policeChances = new float[maxWantedLevel + 1];
	public float[] soldierChances = new float[maxWantedLevel + 1];
	public float[] bankerChances = new float[maxWantedLevel + 1];

	private int frameCounter = 0;

	void Awake () {
		UpdateSpawnPoints();
	}

	public void ActivateSpawners()
	{
		for (int i = 0; i < transform.childCount; i++)
		{
			PersonSpawner spawner = transform.GetChild(i).GetComponent<PersonSpawner>();
			spawner.gameObject.SetActive(true);
		}
	}

	public void ChangeProgressCount(int amount)
	{
		progressCount += amount;
		if (wantedLevel < wantedLevelCap)
		{
			if (progressCount >= progressThreshholds[wantedLevel])
			{
				wantedLevel++;
				UpdateSpawnPoints();
			}
		}
	}

	private void UpdateSpawnPoints()
	{
		for (int i = 0; i < transform.childCount; i++)
		{
			PersonSpawner spawner = transform.GetChild(i).GetComponent<PersonSpawner>();
			spawner.spawnChance = spawnChances[wantedLevel];
			spawner.robberChance = robberChances[wantedLevel];
			spawner.policeChance = policeChances[wantedLevel];
			spawner.soldierChance = soldierChances[wantedLevel];
			spawner.bankerChance = bankerChances[wantedLevel];
		}
	}

	/*
	Difficulty Manager Plan

	Implementation:
		The DifficultyManager should ideally be the only thing the level designer need touch to create a HousingCrisis level
		(this means shifting the ContentManager from the misnamed LevelUI to the DifficultyManager)
		The DifficultyManager's carry's a public array which tells each spawnPoint what group they are in on Awake()
			spawnPoints then get their frequency information from the DifficultyManager on Start() depending on spawnGroup

			a spawnGroups corresponds to an array for each person type where each element is the spawnRate 
				of that type at wantedRating = index
		
		The DifficultyManager also has a set of wantedRating threshholds (# escaped to increase level)

	Design:
		When people escape they raise the player's wanted rating (displayed in the upper right)
		Each wanted rating (1-5 stars) spawns new and/or more enemies
		The beginning levels may have <5 wanted levels to keep things reasonable

		enemies are "unlocked" mid-level (whereas buildings are unlocked at the start)
		this enemy unlocking occurs purely through a pause + info popup + nonzero spawn chance
			triggered by a change in wanted level
		besides this one time unlock per enemy, wanted levels only affect the spawning rates of enemies and people
		higher wanted levels should be manageable if the player has enough buildings + skill
	*/
}
