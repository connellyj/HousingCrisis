using System.Collections.Generic;
using UnityEngine;

public class Population : MonoBehaviour {
  
    private static List<Person> people;
    private static int numEnumerations = 0;

    void Awake() {
        people = new List<Person>();
    }

    // If possible, adds the person to the population, otherwise destroys the person
    public static void AddPerson(Person p) {
        if(numEnumerations == 0) people.Add(p);
        else Destroy(p.gameObject);
    }

    // If possible, removes the person from the list
    // If not, there will be a null person in the list, which is handled elsewhere
    public static void RemovePerson(Person p) {
        if(numEnumerations == 0) people.Remove(p);
    }

    // Loops through all the people and alerts those affected by the house that ate
    public static void AlertPeopleAffectedByEat(Direction d, int hX, int hY) {
        List<Person> toBeEaten = new List<Person>();
        numEnumerations++;
        foreach(Person p in people) {
            if(p != null) {
                if(PersonInRangeToEat(p, d, hX, hY)) toBeEaten.Add(p);
                else if(PersonInRangeToSee(p, d, hX, hY)) p.OnSeeHouse(GridManager.CoordsToIndex(hX, hY));
            }
        }
        numEnumerations--;
        foreach(Person p in toBeEaten) {
            p.OnEaten();
        }
    }

    // Checks if the person is close enough to the house to be eaten by it
    private static bool PersonInRangeToEat(Person p, Direction d, int hX, int hY) {
        Vector3 pos = p.transform.position;
        switch(d) {
            case Direction.WEST:
                return Mathf.Abs(pos.x - (hX - 1)) <= House.eatRadius && Mathf.Abs(pos.y - hY) <= House.eatRadius;
            case Direction.EAST:
                return Mathf.Abs(pos.x - (hX + 1)) <= House.eatRadius && Mathf.Abs(pos.y - hY) <= House.eatRadius;
            case Direction.NORTH:
                return Mathf.Abs(pos.x - hX) <= House.eatRadius && Mathf.Abs(pos.y - (hY + 1)) <= House.eatRadius;
            case Direction.SOUTH:
                return Mathf.Abs(pos.x - hX) <= House.eatRadius && Mathf.Abs(pos.y - (hY - 1)) <= House.eatRadius;
            default:
                return false;
        }
    }

    // Checks to see if the person is close enough and facing the right direction to see the house
    private static bool PersonInRangeToSee(Person p, Direction d, int hX, int hY) {
        int difY = p.Y() - hY;
        int difX = p.X() - hX;
        switch(d) {
            case Direction.WEST:
                return (p.X() == hX - 1 && PersonInRangeNorthSouth(difY, p)) ||
                    (p.direction == Direction.EAST && PersonInRangeSameY(difX, p, hY));
            case Direction.EAST:
                return (p.X() == hX + 1 && PersonInRangeNorthSouth(difY, p) ||
                    (p.direction == Direction.WEST && PersonInRangeSameY(difX, p, hY)));
            case Direction.NORTH:
                return (p.Y() == hY + 1 && PersonInRangeEastWest(difX, p)) ||
                    (p.direction == Direction.SOUTH && PersonInRangeSameX(difY, p, hX));
            case Direction.SOUTH:
                return (p.Y() == hY - 1 && PersonInRangeEastWest(difX, p)) ||
                    (p.direction == Direction.NORTH && PersonInRangeSameX(difY, p, hX));
            default:
                return false;
        }
    }

    // Helper method to check if the person is facing the right way and it close enough
    private static bool PersonInRangeNorthSouth(int difY, Person p) {
        return Mathf.Abs(difY) < House.alertRadius &&
                    (difY < 0 && p.direction == Direction.NORTH ||
                    difY > 0 && p.direction == Direction.SOUTH);
    }

    // Helper method to check if the person is facing the right way and it close enough
    private static bool PersonInRangeEastWest(int difX, Person p) {
        return Mathf.Abs(difX) < House.alertRadius &&
                    (difX < 0 && p.direction == Direction.EAST ||
                    difX > 0 && p.direction == Direction.WEST);
    }

    // Helper method to check if the person is in the same column and close enough
    private static bool PersonInRangeSameX(int difY, Person p, int xLoc) {
        return p.X() == xLoc && Mathf.Abs(difY) < House.alertRadius;
    }

    // Helper method to check if the person is in the same row and close enough
    private static bool PersonInRangeSameY(int difX, Person p, int yLoc) {
        return p.Y() == yLoc && Mathf.Abs(difX) < House.alertRadius;
    }

    // Loops through all the people and alerts those attracted by the store
    public static void AlertPeopleAffectedByStore(HouseManager.HouseType type, int numPeople, Vector3 storePos, List<Person> stalledPeople) {
        if(numPeople == 0) return;
        Person[] toBePulled;
        if(type == HouseManager.HouseType.STORE) toBePulled = GetPeopleAffectedNormalStore(storePos, numPeople, stalledPeople);
        else if(type == HouseManager.HouseType.DONUT) toBePulled = GetPeopleAffectedSpecialStore(storePos, "PersonPolice", numPeople, stalledPeople);
        else if(type == HouseManager.HouseType.BANK) toBePulled = GetPeopleAffectedSpecialStore(storePos, "PersonBanker", numPeople, stalledPeople);
        else return;
        foreach(Person toPull in toBePulled) {
            if(toPull != null) toPull.OnStorePull(GridManager.CoordsToIndex((int)Mathf.Round(storePos.x), (int)Mathf.Round(storePos.y)));
        }
    }

    // Determines which people should be affected by the special stores (bank, donut)
    public static Person[] GetPeopleAffectedSpecialStore(Vector3 storePos, string personType, int numPeople, List<Person> stalledPeople) {
        Person[] toBePulled = new Person[numPeople];
        float[] closestPeople = new float[numPeople];
        for(int i = 0; i < numPeople; i++) {
            closestPeople[i] = House.alertRadius;
        }
        int greatestIndex = 0;
        numEnumerations++;
        foreach(Person p in people) {
            if(p != null) {
                float dist = (p.transform.position - storePos).magnitude;

                // if the person is within the alert radius and is not already stalled...
                if((p.tag == "Person" || p.tag == personType) && dist < House.alertRadius && p.state != Person.PersonState.STALL && !stalledPeople.Contains(p)) {
                    // if the greatestIndex person is null or...
                    if(toBePulled[greatestIndex] == null ||
                        // if the person is the same type of person as the greatestIndex person
                        // and the person is closer to the house than the greatestIndex person or...
                        (dist < closestPeople[greatestIndex] && p.tag == toBePulled[greatestIndex].tag) ||
                        // if the person is the special person type and the greatestIndex person is not
                        (toBePulled[greatestIndex].tag != personType && p.tag == personType)) {

                            // set the greatestIndex to the person and update greatestIndex
                            closestPeople[greatestIndex] = dist;
                            toBePulled[greatestIndex] = p;
                            for(int i = 0; i < closestPeople.Length; i++) {
                                if(toBePulled[i] == null) {
                                    greatestIndex = i;
                                    break;
                                } else if((closestPeople[greatestIndex] < closestPeople[i] && toBePulled[greatestIndex].tag == toBePulled[i].tag) ||
                                     (toBePulled[greatestIndex].tag == personType && toBePulled[i].tag != personType)) {
                                    greatestIndex = i;
                                }
                            }
                    }
                }
            }
        }
        numEnumerations--;
        return toBePulled;
    }

    // Determines which people should be affected by the normal store
    public static Person[] GetPeopleAffectedNormalStore(Vector3 storePos, int numPeople, List<Person> stalledPeople) {
        Person[] toBePulled = new Person[numPeople];
        float[] closestPeople = new float[numPeople];
        for(int i = 0; i < numPeople; i++) {
            closestPeople[i] = House.alertRadius;
        }
        int greatestIndex = 0;
        numEnumerations++;
        foreach(Person p in people) {
            if(p != null) {
                float dist = (p.transform.position - storePos).magnitude;
                if(p.tag == "Person" && p.state != Person.PersonState.STALL && dist < closestPeople[greatestIndex] && !stalledPeople.Contains(p)) {
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
        return toBePulled;
    }
}