using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class HouseManager : MonoBehaviour {
    
    public GameObject firePrefab;
    public Sprite[] smokeSprites;
    public float smokeTime;
    public GameObject[] buildings;
    public int[] houseCosts;

    public static Dictionary<int, House> houses;
    public static List<int> burningHouses;

    private static HouseManager instance;

    public enum HouseType { HOUSE, APARTMENT, MANSION, STORE, DONUT, BANK, PLOT, LOCKED }

    void Awake() {
        instance = this;
        houses = new Dictionary<int, House>();
        burningHouses = new List<int>();
    }

    // Returns the time for which a building should be smoking
    public static float GetSmokeTime() {
        return instance.smokeTime;
    }

    // Returns the smoke sprites
    public static Sprite[] GetSmokeSprites() {
        return instance.smokeSprites;
    }

    // Returns the fire prefab for the houses when they'e burning
    public static GameObject GetFirePrefab() {
        return instance.firePrefab;
    }

    // Builds the given house, assumes that you've already checked that there's enough money
    public static void Build(Vector3 position, HouseType type) {
        Instantiate(instance.buildings[(int)type], position, Quaternion.identity);
    }

    // Returns whether or not you have enough money to build the given house
    public static bool CanBuild(HouseType type) {
        int money = GameManager.GetMoney();
        if (!ContentManager.IsBuildingUnlocked(type)) return false;
        return money >= instance.houseCosts[(int)type];
    }

    // Returns the cost of the given house type
    public static int GetCost(HouseType t) {
        return instance.houseCosts[(int) t];
    }

    // Adds a house to the list of burning houses
    public static void AddBurningHouse(int index) {
        burningHouses.Add(index);
    }

    // Removes a house from the list of burning houses
    public static void RemoveBurningHouse(int index) {
        burningHouses.Remove(index);
    }

    // Adds a house to the dictionary of houses
    public static void AddHouse(House h) {
        if (houses.ContainsKey(h.gridIndex)) {
            houses[h.gridIndex] = h;
        } else {
            houses.Add(h.gridIndex, h);
        }
    }

    // Removes a house from the dictionary of houses and the list of burning houses
    public static void RemoveHouse(House h) {
        houses.Remove(h.gridIndex);
        burningHouses.Remove(h.gridIndex);
    }

    // Returns whether or not any houses are not burning
    public static bool AnyHousesNotBurning() {
        return houses.Count != burningHouses.Count;
    }

    // Returns whether or not there is free space in front of any house
    public static bool AnyStallSpaceAnywhere() {
        foreach(House h in houses.Values) {
            if(h.numStalled < House.MAX_STALL) return true;
        }
        return false;
    }

    // Updates the mansion sprinklers
    public static void UpdateSprinklers() {
        foreach (House h in houses.Values) {
            h.SprinklersOn(isAdjacentToMansion(h));
        }
    }

    // Returns whether or not the given house is adjacent to a mansion (includes houses on diagonal)
    public static bool isAdjacentToMansion(House h) {
        foreach (House g in AdjacentHouses( h.X() , h.Y() )) {
            if (g.type == HouseType.MANSION) return true;
        }
        return false;
    }

    // Gets the houses adjacent to the given house (includes houses on diagonal)
    private static List<House> AdjacentHouses(int x, int y) {
        List<House> adjacent = new List<House>();
        int[] adjacentIndexes = GetAdjacentIndexes(x,y);
        for (int i = 0; i < adjacentIndexes.Length; i++) {
            if (houses.ContainsKey(adjacentIndexes[i])) {
                adjacent.Add(houses[adjacentIndexes[i]]);
            }
        }
        return adjacent;
    }
    
    // Returns an array of indeces adjacent to the given coordinates (includes coordinates on diagonal)
    private static int[] GetAdjacentIndexes(int x, int y) {
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

    // Returns a random house index that isn't burning
    public static int GetRandomNotBurningHouse() {
        if(burningHouses.Count == 0) {
            return houses.Keys.ElementAt(Random.Range(0, houses.Count));
        } else {
            int goalIndex = burningHouses[0];
            while(burningHouses.Contains(goalIndex)) {
                goalIndex = houses.Keys.ElementAt(Random.Range(0, houses.Count));
            }
            return goalIndex;
        }
    }

    // Returns a random house index
    public static int GetRandomHouse() {
        return houses.Keys.ElementAt(Random.Range(0, houses.Count));
    }
}