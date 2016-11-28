using UnityEngine;
using System.Collections.Generic;

public class House : MonoBehaviour {

    public int noticeThreshold;
    public int cost;

    private List<Person> allPeople;
    private Population population;
    private int[] gridPos;

    void Awake() {
        gridPos = new int[2] { (int)Mathf.Round(transform.position.x), (int)Mathf.Round(transform.position.y) };
        population = GameManager.GetPopulation();
    }

    public void Eat(Direction d) {
        allPeople = population.GetAllPeople();
        foreach(Person p in allPeople) {
            if(PersonInRange(p, d)) p.OnSeeHouse();
        }
    }

    private bool PersonInRange(Person p, Direction d) {
        int difY = p.Y() - gridPos[1];
        int difX = p.X() - gridPos[0];
        switch(d) {
            case Direction.WEST:
                return p.X() == gridPos[0] - 1 && 
                    PersonInRangeNorthSouth(difY, p);
            case Direction.EAST:
                return p.X() == gridPos[0] + 1 && 
                    PersonInRangeNorthSouth(difY, p);
            case Direction.NORTH:
                return p.Y() == gridPos[1] + 1 && 
                    PersonInRangeEastWest(difX, p);
            case Direction.SOUTH:
                return p.Y() == gridPos[1] - 1 &&
                    PersonInRangeEastWest(difX, p);
            default:
                return false;
        }
    }

    private bool PersonInRangeNorthSouth(int difY, Person p) {
        return Mathf.Abs(difY) < noticeThreshold &&
                    (difY < 0 && p.direction == Direction.NORTH ||
                    difY > 0 && p.direction == Direction.SOUTH);
    }

    private bool PersonInRangeEastWest(int difX, Person p) {
        return Mathf.Abs(difX) < noticeThreshold &&
                    (difX < 0 && p.direction == Direction.EAST ||
                    difX > 0 && p.direction == Direction.WEST);
    }
}
