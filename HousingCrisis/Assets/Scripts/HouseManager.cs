using UnityEngine;
using System.Collections.Generic;

public class HouseManager : MonoBehaviour {

    public GameObject house1;
    public GameObject house2;
    public GameObject office;
    public GameObject donut;
    public GameObject attack;
    public GameObject store;

    private int house1Cost;
    private int house2Cost;
    private int officeCost;
    private int donutCost;
    private int attackCost;
    private int storeCost;

    private static HouseManager instance;

    public enum HouseType { HOUSE1, HOUSE2, OFFICE, DONUT, ATTACK, STORE }

    void Awake() {
        instance = this;
    }

    void Start() {
        house1Cost = instance.house1.GetComponent<House>().cost;
        house2Cost = instance.house2.GetComponent<House>().cost;
        officeCost = instance.office.GetComponent<House>().cost;
        donutCost = instance.donut.GetComponent<House>().cost;
        attackCost = instance.attack.GetComponent<House>().cost;
        storeCost = instance.store.GetComponent<House>().cost;
    }

    public static void BuildHouse(Vector3 position, HouseType type, List<Direction> adjacentPaths) {
        GameObject house = null;
        switch(type) {
            case HouseType.HOUSE1:
                house = Instantiate(instance.house1, position, Quaternion.identity) as GameObject;
                GameManager.UpdateMoney(-1 * instance.house1Cost);
                break;
            case HouseType.HOUSE2:
                house = Instantiate(instance.house2, position, Quaternion.identity) as GameObject;
                GameManager.UpdateMoney(-1 * instance.house2Cost);
                break;
            case HouseType.OFFICE:
                house = Instantiate(instance.office, position, Quaternion.identity) as GameObject;
                GameManager.UpdateMoney(-1 * instance.officeCost);
                break;
            case HouseType.DONUT:
                house = Instantiate(instance.donut, position, Quaternion.identity) as GameObject;
                GameManager.UpdateMoney(-1 * instance.donutCost);
                break;
            case HouseType.ATTACK:
                house = Instantiate(instance.attack, position, Quaternion.identity) as GameObject;
                GameManager.UpdateMoney(-1 * instance.attackCost);
                break;
            case HouseType.STORE:
                house = Instantiate(instance.store, position, Quaternion.identity) as GameObject;
                GameManager.UpdateMoney(-1 * instance.storeCost);
                break;
        }
        RemoveTriggers(adjacentPaths, house);
        AddHouseToGrid(position);
    }

    public static bool CanBuildHouse(HouseType type) {
        int money = GameManager.GetMoney();
        switch(type) {
            case HouseType.HOUSE1:
                return money >= instance.house1Cost;
            case HouseType.HOUSE2:
                return money >= instance.house2Cost;
            case HouseType.OFFICE:
                return money >= instance.officeCost;
            case HouseType.DONUT:
                return money >= instance.donutCost;
            case HouseType.ATTACK:
                return money >= instance.attackCost;
            case HouseType.STORE:
                return money >= instance.storeCost;
            default:
                return false;
        }
    }

    public static void RemoveTriggers(List<Direction> adjacent, GameObject house) {
        foreach(Transform t in house.transform) {
            if(!adjacent.Contains(t.GetComponent<EatingArea>().direction)) Destroy(t.gameObject);
        }
    }

    public static void AddHouseToGrid(Vector3 pos) {
        GridManager.houses.Add((GridManager.MAX_ROW - 1 - (int)Mathf.Round(pos.y)) * GridManager.MAX_COL + (int)Mathf.Round(pos.x));
    }
}