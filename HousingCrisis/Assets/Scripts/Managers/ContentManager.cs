using UnityEngine;

public class ContentManager : MonoBehaviour {

	public bool houseUnlocked = true;
	public bool storeUnlocked;
	public bool apartmentUnlocked;
	public bool donutUnlocked;
	public bool mansionUnlocked;
	public bool bankUnlocked;
    public int startingMoney;
    public int peopleEatenToWin;

    private static ContentManager instance;

    void Awake () {
        instance = this;
	}

    // Returns how much the palyer starts the level with
    public static int StartingMoney() {
        return instance.startingMoney;
    }

    // Returns how much money is needed to win
    public static int PeopleEatenToWin() {
        return instance.peopleEatenToWin;
    }

    // Returns whether or not the given house type is unlocked
	public static bool IsBuildingUnlocked(HouseManager.HouseType type) {
		switch(type) {
			case HouseManager.HouseType.HOUSE:
				return instance.houseUnlocked;
			case HouseManager.HouseType.APARTMENT:
				return instance.apartmentUnlocked;
			case HouseManager.HouseType.MANSION:
				return instance.mansionUnlocked;
			case HouseManager.HouseType.STORE:
				return instance.storeUnlocked;
			case HouseManager.HouseType.DONUT:
				return instance.donutUnlocked;
			case HouseManager.HouseType.BANK:
				return instance.bankUnlocked;
			default:
				return false;
		}
	}

}
