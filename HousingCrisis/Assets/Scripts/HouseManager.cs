using UnityEngine;
using System.Collections.Generic;

public class HouseManager : MonoBehaviour {

    public GameObject house1;
    public GameObject house2;
    public GameObject office;
    public GameObject donut;
    public GameObject attack;
    public GameObject store;

    private static HouseManager instance;

    public enum HouseType { HOUSE, APARTMENT, BANK, DONUT, MANSION, STORE }

    void Awake() {
        instance = this;
    }

    public static void BuildHouse(Vector3 position, HouseType type, List<Direction> adjacentPaths) {
        House house = null;
        switch(type) {
            case HouseType.HOUSE:
                house = ((GameObject)Instantiate(instance.house1, position, Quaternion.identity)).GetComponent<House>();
                break;
            case HouseType.APARTMENT:
                house = ((GameObject) Instantiate(instance.house2, position, Quaternion.identity)).GetComponent<House>();
                break;
            case HouseType.BANK:
                house = ((GameObject) Instantiate(instance.office, position, Quaternion.identity)).GetComponent<Bank>();
                break;
            case HouseType.DONUT:
                house = ((GameObject) Instantiate(instance.donut, position, Quaternion.identity)).GetComponent<House>();
                break;
            case HouseType.MANSION:
                house = ((GameObject) Instantiate(instance.attack, position, Quaternion.identity)).GetComponent<House>();
                break;
            case HouseType.STORE:
                house = ((GameObject) Instantiate(instance.store, position, Quaternion.identity)).GetComponent<House>();
                break;
        }
        house.Buy();
        RemoveTriggers(adjacentPaths, house);
        AddHouseToGrid(position);
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

    public static void RemoveTriggers(List<Direction> adjacent, House house) {
        foreach(Transform t in house.transform) {
            if(!adjacent.Contains(t.GetComponent<EatingArea>().direction)) Destroy(t.gameObject);
        }
    }

    public static void AddHouseToGrid(Vector3 pos) {
        GridManager.houses.Add(GridManager.coordsToIndex(((int) Mathf.Round(pos.x)), ((int) Mathf.Round(pos.y))));
    }
}