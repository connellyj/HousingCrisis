using UnityEngine;
using System.Collections.Generic;

public class HouseManager : MonoBehaviour {

    public GameObject house;
    public GameObject apartment;
    public GameObject bank;
    public GameObject donut;
    public GameObject mansion;
    public GameObject store;

    public static Dictionary<int, House> houses;
    public static List<int> burningHouses;

    private static HouseManager instance;

    public enum HouseType { HOUSE, APARTMENT, BANK, DONUT, MANSION, STORE }

    void Awake() {
        instance = this;
        houses = new Dictionary<int, House>();
        burningHouses = new List<int>();
    }

    public static void BuildHouse(Vector3 position, HouseType type, List<Direction> adjacentPaths) {
        House house = null;
        switch(type) {
            case HouseType.HOUSE:
                house = ((GameObject)Instantiate(instance.house, position, Quaternion.identity)).GetComponent<House>();
                break;
            case HouseType.APARTMENT:
                house = ((GameObject) Instantiate(instance.apartment, position, Quaternion.identity)).GetComponent<House>();
                break;
            case HouseType.BANK:
                house = ((GameObject) Instantiate(instance.bank, position, Quaternion.identity)).GetComponent<Bank>();
                break;
            case HouseType.DONUT:
                house = ((GameObject) Instantiate(instance.donut, position, Quaternion.identity)).GetComponent<House>();
                break;
            case HouseType.MANSION:
                house = ((GameObject) Instantiate(instance.mansion, position, Quaternion.identity)).GetComponent<House>();
                break;
            case HouseType.STORE:
                house = ((GameObject) Instantiate(instance.store, position, Quaternion.identity)).GetComponent<House>();
                break;
        }
        house.Buy();
        house.RemoveTriggers(adjacentPaths);
        AddHouse(house);
    }

    public static bool CanBuildHouse(HouseType type) {
        int money = GameManager.GetMoney();
        switch(type) {
            case HouseType.HOUSE:
                return money >= House.cost;
            case HouseType.APARTMENT:
                return money >= House.cost;
            case HouseType.BANK:
                return money >= House.cost;
            case HouseType.DONUT:
                return money >= House.cost;
            case HouseType.MANSION:
                return money >= House.cost;
            case HouseType.STORE:
                return money >= House.cost;
            default:
                return false;
        }
    }

    public static void AddBurningHouse(House h) {
        burningHouses.Add(GridManager.CoordsToIndex(h.X(), h.Y()));
    }

    public static void RemoveBurningHouse(House h) {
        burningHouses.Remove(GridManager.CoordsToIndex(h.X(), h.Y()));
    }

    public static void AddHouse(House h) {
        int idx = GridManager.CoordsToIndex(h.X(), h.Y());
        houses.Add(idx, h);
    }

    public static void RemoveHouse(House house) {
        int idx = GridManager.CoordsToIndex(house.X(), house.Y());
        houses.Remove(idx);
        if(burningHouses.Contains(idx)) burningHouses.Remove(idx);
    }

    public static bool AnyStallSpaceAnywhere() {
        foreach(House h in houses.Values) {
            if(h.HasAvailableStallSpace()) return true;
        }
        return false;
    }

    public static bool AnyHousesNotBurning() {
        return houses.Count != burningHouses.Count;
    }
}