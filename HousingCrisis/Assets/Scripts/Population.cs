using System.Collections.Generic;
using UnityEngine;

public class Population : MonoBehaviour {
  
    private static List<Person> people;
    private static List<Person> toBeEaten;
    private static Person[] toBePulled;
    private static float[] closestPeople;
    private static int numEnumerations = 0;

    void Awake() {
        people = new List<Person>();
    }

    public static void AddPerson(Person p) {
        if(numEnumerations == 0) people.Add(p);
    }

    public static void RemovePerson(Person p) {
        if(numEnumerations == 0) people.Remove(p);
    }

    public static void ClearPeople() {
        if(numEnumerations == 0) people.Clear();
    }

    public static void AlertPeopleAffectedByEat(Direction d, int[] houseLocXY, float eatRadius, int noticeThreshold) {
        toBeEaten = new List<Person>();
        numEnumerations++;
        foreach(Person p in people) {
            if(PersonInRangeToEat(p, d, eatRadius, houseLocXY)) toBeEaten.Add(p);
            else if(PersonInRangeToSee(p, d, noticeThreshold, houseLocXY)) p.OnSeeHouse(GridManager.CoordsToIndex(houseLocXY[0], houseLocXY[1]));
        }
        numEnumerations--;
        foreach(Person p in toBeEaten) {
            people.Remove(p);
            p.OnEaten();
        }
    }

    private static bool PersonInRangeToEat(Person p, Direction d, float eatRadius, int[] gridPos) {
        Vector3 pos = p.transform.position;
        switch(d) {
            case Direction.WEST:
                return Mathf.Abs(pos.x - (gridPos[0] - 1)) <= eatRadius && Mathf.Abs(pos.y - gridPos[1]) <= eatRadius;
            case Direction.EAST:
                return Mathf.Abs(pos.x - (gridPos[0] + 1)) <= eatRadius && Mathf.Abs(pos.y - gridPos[1]) <= eatRadius;
            case Direction.NORTH:
                return Mathf.Abs(pos.x - gridPos[0]) <= eatRadius && Mathf.Abs(pos.y - (gridPos[1] + 1)) <= eatRadius;
            case Direction.SOUTH:
                return Mathf.Abs(pos.x - gridPos[0]) <= eatRadius && Mathf.Abs(pos.y - (gridPos[1] - 1)) <= eatRadius;
            default:
                return false;
        }
    }

    private static bool PersonInRangeToSee(Person p, Direction d, int noticeThreshold, int[] gridPos) {
        int difY = p.Y() - gridPos[1];
        int difX = p.X() - gridPos[0];
        switch(d) {
            case Direction.WEST:
                return (p.X() == gridPos[0] - 1 && PersonInRangeNorthSouth(difY, p, noticeThreshold)) ||
                    (p.direction == Direction.EAST && PersonInRangeSameY(difX, p, gridPos[1], noticeThreshold));
            case Direction.EAST:
                return (p.X() == gridPos[0] + 1 && PersonInRangeNorthSouth(difY, p, noticeThreshold)) ||
                    (p.direction == Direction.WEST && PersonInRangeSameY(difX, p, gridPos[1], noticeThreshold));
            case Direction.NORTH:
                return (p.Y() == gridPos[1] + 1 && PersonInRangeEastWest(difX, p, noticeThreshold)) ||
                    (p.direction == Direction.SOUTH && PersonInRangeSameX(difY, p, gridPos[0], noticeThreshold));
            case Direction.SOUTH:
                return (p.Y() == gridPos[1] - 1 && PersonInRangeEastWest(difX, p, noticeThreshold)) ||
                    (p.direction == Direction.NORTH && PersonInRangeSameX(difY, p, gridPos[0], noticeThreshold));
            default:
                return false;
        }
    }

    private static bool PersonInRangeNorthSouth(int difY, Person p, int noticeThreshold) {
        return Mathf.Abs(difY) < noticeThreshold &&
                    (difY < 0 && p.direction == Direction.NORTH ||
                    difY > 0 && p.direction == Direction.SOUTH);
    }

    private static bool PersonInRangeEastWest(int difX, Person p, int noticeThreshold) {
        return Mathf.Abs(difX) < noticeThreshold &&
                    (difX < 0 && p.direction == Direction.EAST ||
                    difX > 0 && p.direction == Direction.WEST);
    }

    private static bool PersonInRangeSameX(int difY, Person p, int xLoc, int noticeThreshold) {
        return p.X() == xLoc && Mathf.Abs(difY) < noticeThreshold;
    }

    private static bool PersonInRangeSameY(int difX, Person p, int yLoc, int noticeThreshold) {
        return p.Y() == yLoc && Mathf.Abs(difX) < noticeThreshold;
    }

    public static void AlertPeopleAffectedByStore(HouseManager.HouseType type, float alertRadius, int numPeople, Vector3 storePos) {
        if(numPeople == 0) return;
        toBePulled = new Person[numPeople];
        closestPeople = new float[numPeople];
        for(int i = 0; i < numPeople; i++) {
            closestPeople[i] = float.MaxValue;
        }

        if(type == HouseManager.HouseType.STORE) AlertPeopleAffectedNormalStore(alertRadius, storePos);
        else if(type == HouseManager.HouseType.DONUT) AlertPeopleAffectedSpecialStore(alertRadius, storePos, "PersonPolice");
        else if(type == HouseManager.HouseType.DONUT) AlertPeopleAffectedSpecialStore(alertRadius, storePos, "PersonBanker");
        else return;

        foreach(Person toPull in toBePulled) {
            if(toPull != null) toPull.OnStorePull(GridManager.CoordsToIndex((int)Mathf.Round(storePos.x), (int)Mathf.Round(storePos.y)));
        }
    }

    public static void AlertPeopleAffectedSpecialStore(float alertRadius, Vector3 storePos, string personType) {
        int greatestIndex = 0;
        numEnumerations++;
        foreach(Person p in people) {
            float dist = (p.transform.position - storePos).magnitude;
            if(dist < alertRadius && p.state != Person.PersonState.STALL) {
                if(toBePulled[greatestIndex] == null || 
                    (dist < closestPeople[greatestIndex] && p.tag == toBePulled[greatestIndex].tag) || 
                    (toBePulled[greatestIndex].tag != personType && p.tag == personType)) {
                    closestPeople[greatestIndex] = dist;
                    toBePulled[greatestIndex] = p;
                    for(int i = 0; i < closestPeople.Length; i++) {
                        if(toBePulled[i] == null) {
                            greatestIndex = i;
                            break;
                        }else if((closestPeople[greatestIndex] < closestPeople[i] && toBePulled[greatestIndex].tag == toBePulled[i].tag) ||
                            (toBePulled[greatestIndex].tag == personType && toBePulled[i].tag != personType)) {
                            greatestIndex = i;
                        }
                    }
                }
            }
        }
        numEnumerations--;
    }

    public static void AlertPeopleAffectedNormalStore(float alertRadius, Vector3 storePos) {
        int greatestIndex = 0;
        numEnumerations++;
        foreach(Person p in people) {
            float dist = (p.transform.position - storePos).magnitude;
            if(dist < alertRadius && p.state != Person.PersonState.STALL) {
                if(dist < closestPeople[greatestIndex]) {
                    closestPeople[greatestIndex] = dist;
                    toBePulled[greatestIndex] = p;
                    for(int i = 0; i < closestPeople.Length; i++) {
                        if(closestPeople[greatestIndex] < closestPeople[i]) {
                            greatestIndex = i;
                        }
                    }
                }
            }
        }
        numEnumerations--;
    }
}