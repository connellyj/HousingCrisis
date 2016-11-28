using UnityEngine;
using System.Collections.Generic;

public class House : MonoBehaviour {

    public int noticeThreshold;
    public int cost;

    private List<Person> allPeople;
    private List<Person> toRemove;
    private Population population;
    private int[] gridPos;

    void Awake() {
        gridPos = new int[2] { (int)Mathf.Round(transform.position.x), (int)Mathf.Round(transform.position.y) };
        population = GameManager.GetPopulation();
    }

    public void Eat(Direction d) {
        toRemove = new List<Person>();
        allPeople = population.GetAllPeople();
        foreach(Person p in allPeople) {
            if(PersonInRange(p, Direction.NONE)) toRemove.Add(p);
            else if(PersonInRange(p, d)) p.OnSeeHouse();
        }
        foreach(Person p in toRemove) {
            allPeople.Remove(p);
            p.OnEaten();
        }
    }

    private bool PersonInRange(Person p, Direction d) {
        int difY = p.Y() - gridPos[1];
        int difX = p.X() - gridPos[0];
        switch(d) {
            case Direction.WEST:
                return (p.X() == gridPos[0] - 1 && PersonInRangeNorthSouth(difY, p)) ||
                    (p.direction == Direction.EAST && PersonInRangeSameY(difX, p));
            case Direction.EAST:
                return (p.X() == gridPos[0] + 1 && PersonInRangeNorthSouth(difY, p)) ||
                    (p.direction == Direction.WEST && PersonInRangeSameY(difX, p));
            case Direction.NORTH:
                return (p.Y() == gridPos[1] + 1 && PersonInRangeEastWest(difX, p)) ||
                    (p.direction == Direction.SOUTH && PersonInRangeSameX(difY, p));
            case Direction.SOUTH:
                return (p.Y() == gridPos[1] - 1 && PersonInRangeEastWest(difX, p)) ||
                    (p.direction == Direction.NORTH && PersonInRangeSameX(difY, p));
            case Direction.NONE:
                return Mathf.Abs(difY) <= 2 && Mathf.Abs(difX) <= 2;
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

    private bool PersonInRangeSameX(int difY, Person p) {
        return p.X() == gridPos[0] && Mathf.Abs(difY) < noticeThreshold;
    }

    private bool PersonInRangeSameY(int difX, Person p) {
        return p.Y() == gridPos[1] && Mathf.Abs(difX) < noticeThreshold;
    }
}
