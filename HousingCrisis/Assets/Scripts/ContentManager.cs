﻿using UnityEngine;
using System.Collections;

public class ContentManager : MonoBehaviour {

	public bool houseUnlocked = true;
	public bool storeUnlocked;
	public bool apartmentUnlocked;
	public bool donutUnlocked;
	public bool mansionUnlocked;
	public bool bankUnlocked;

	public static bool houseUnlockedS = true;
	public static bool storeUnlockedS;
	public static bool apartmentUnlockedS;
	public static bool donutUnlockedS;
	public static bool mansionUnlockedS;
	public static bool bankUnlockedS;

	void Awake ()
	{
		houseUnlockedS = houseUnlocked;
		storeUnlockedS = storeUnlocked;
		apartmentUnlockedS = apartmentUnlocked;
		donutUnlockedS = donutUnlocked;
		mansionUnlockedS = mansionUnlocked;
		bankUnlockedS = bankUnlocked;
	}

	public static bool isBuildingUnlocked(HouseManager.HouseType type)
	{
		switch(type) {
			case HouseManager.HouseType.HOUSE:
				return houseUnlockedS;
			case HouseManager.HouseType.APARTMENT:
				return apartmentUnlockedS;
			case HouseManager.HouseType.MANSION:
				return mansionUnlockedS;
			case HouseManager.HouseType.STORE:
				return storeUnlockedS;
			case HouseManager.HouseType.DONUT:
				return donutUnlockedS;
			case HouseManager.HouseType.BANK:
				return bankUnlockedS;
			default:
				return false;
		}
	}

}