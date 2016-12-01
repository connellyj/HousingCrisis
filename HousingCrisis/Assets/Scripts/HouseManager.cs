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

    private static HouseManager instance;

    public enum HouseType { HOUSE, APARTMENT, BANK, DONUT, MANSION, STORE }

    void Awake() {
        instance = this;
        houses = new Dictionary<int, House>();
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
        AddHouse(position, house);
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

    public static void AddHouse(Vector3 pos, House h) {
        int idx = GridManager.CoordsToIndex(((int) Mathf.Round(pos.x)), ((int) Mathf.Round(pos.y)));
        GridManager.houses.Add(idx);
        houses.Add(idx, h);
    }

    public static void RemoveHouse(House house) {
        Vector3 pos = house.transform.position;
        int idx = GridManager.CoordsToIndex(((int) Mathf.Round(pos.x)), ((int) Mathf.Round(pos.y)));
        if(house.burnState > 0) {
            GridManager.burningHouses.Remove(idx);
        }else {
            GridManager.houses.Remove(idx);
        }
        houses.Remove(idx);
    }

    public static bool AnyStallSpaceAnywhere() {
        foreach(House h in houses.Values) {
            if(h.HasAvailableStallSpace()) return true;
        }
        return false;
    }
}