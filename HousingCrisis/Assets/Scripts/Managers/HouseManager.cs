using UnityEngine;
using System.Collections.Generic;

public class HouseManager : MonoBehaviour {

    public GameObject house;
    public GameObject apartment;
    public GameObject bank;
    public GameObject donut;
    public GameObject mansion;
    public GameObject store;
    public GameObject firePrefab;

    public static Dictionary<int, House> houses;
    public static List<int> burningHouses;

    private static HouseManager instance;

    public enum HouseType { HOUSE, APARTMENT, MANSION, STORE, DONUT, BANK, PLOT, LOCKED }

    void Awake() {
        instance = this;
        houses = new Dictionary<int, House>();
        burningHouses = new List<int>();
    }

    public static GameObject GetFirePrefab() {
        return instance.firePrefab;
    }

    public static void Build(Vector3 position, HouseType type) {
        House house = null;
        switch(type) {
            case HouseType.HOUSE:
                house = ((GameObject)Instantiate(instance.house, position, Quaternion.identity)).GetComponent<House>();
                break;
            case HouseType.APARTMENT:
                house = ((GameObject) Instantiate(instance.apartment, position, Quaternion.identity)).GetComponent<Apartment>();
                break;
            case HouseType.BANK:
                house = ((GameObject) Instantiate(instance.bank, position, Quaternion.identity)).GetComponent<Store>();
                break;
            case HouseType.DONUT:
                house = ((GameObject) Instantiate(instance.donut, position, Quaternion.identity)).GetComponent<Store>();
                break;
            case HouseType.MANSION:
                house = ((GameObject) Instantiate(instance.mansion, position, Quaternion.identity)).GetComponent<Mansion>();
                break;
            case HouseType.STORE:
                house = ((GameObject) Instantiate(instance.store, position, Quaternion.identity)).GetComponent<Store>();
                break;
        }
        AddHouse(house);
    }

    public static bool CanBuild(HouseType type) {
        int money = GameManager.GetMoney();
        if (!ContentManager.isBuildingUnlocked(type)) return false;
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

    public static void UpdateSprinklers()
    {
        foreach (House h in houses.Values) {
            h.SprinklersOn(isAdjacentToMansion(h));
        }
    }

    public static bool isAdjacentToMansion(House h) //includes houses on diagonal
    {
        foreach (House g in adjacentHouses( h.X() , h.Y() ))
        {
            if (g.type == HouseType.MANSION) 
            {
                Debug.LogFormat("House at {0},{1} is adjacent to a Mansion", h.X(), h.Y());
                return true;
            }
        }
        return false;
    }

    private static List<House> adjacentHouses(int x, int y) //includes houses on diagonal
    {
        List<House> adjacent = new List<House>();
        int[] adjacentIndexes = GetAdjacentIndexes(x,y);
        for (int i = 0; i < adjacentIndexes.Length; i++)
        {
            if (houses.ContainsKey(adjacentIndexes[i]))
            {
                adjacent.Add(houses[adjacentIndexes[i]]);
            }
        }
        return adjacent;
    }

    private static int[] GetAdjacentIndexes(int x, int y)
    {
        int[] adjacentIndexes = new int[8];
        adjacentIndexes[0] = GridManager.CoordsToIndex(x,y+1);      // north
        adjacentIndexes[1] = GridManager.CoordsToIndex(x,y-1);      // south
        adjacentIndexes[2] = GridManager.CoordsToIndex(x+1,y);      // east
        adjacentIndexes[3] = GridManager.CoordsToIndex(x-1,y);      // west
        adjacentIndexes[4] = GridManager.CoordsToIndex(x+1,y+1);    // northeast
        adjacentIndexes[5] = GridManager.CoordsToIndex(x-1,y+1);    // northwest
        adjacentIndexes[6] = GridManager.CoordsToIndex(x+1,y-1);    // southeast
        adjacentIndexes[7] = GridManager.CoordsToIndex(x-1,y-1);    // southwest
        return adjacentIndexes;
    }

}