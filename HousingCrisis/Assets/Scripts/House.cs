using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class House : MonoBehaviour {

    public int noticeThreshold;
    public float chewingTime;
    public int cost;

    // sprites and renderer
    SpriteRenderer spriteRenderer;
    public GameObject spriteWrapper;
    public Sprite defaultSprite;
    public Sprite eatingSprite;
    public Sprite[] chewingSprites = new Sprite[4];

    private List<Person> allPeople;
    private List<Person> toRemove;
    private Population population;
    private int[] gridPos;
    private float eatRadius;
    private bool isChewing = false;

    void Start() {
        gridPos = new int[2] { (int)Mathf.Round(transform.position.x), (int)Mathf.Round(transform.position.y) };
        eatRadius = transform.GetChild(0).GetComponent<CircleCollider2D>().radius;
        population = GameManager.GetPopulation();
        spriteRenderer = spriteWrapper.GetComponent<SpriteRenderer>();
    }

    public void Eat(Direction d) {
        if (!isChewing)
        {
            isChewing = true;
            StartCoroutine(EatAnimation(d));
            toRemove = new List<Person>();
            allPeople = population.GetAllPeople();
            foreach(Person p in allPeople) {
                if(PersonInRangeToEat(p, d)) toRemove.Add(p);
                else if(PersonInRangeToSee(p, d)) p.OnSeeHouse();
            }
            foreach(Person p in toRemove) {
                allPeople.Remove(p);
                p.OnEaten();
            }
        }
    }

    private bool PersonInRangeToEat(Person p, Direction d) {
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

    private bool PersonInRangeToSee(Person p, Direction d) {
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

    private IEnumerator EatAnimation(Direction d)
    {
        Vector3 origin = spriteWrapper.transform.position;
        Vector3 v = GridManager.DirectionToVector(d) / 2;   
        spriteWrapper.transform.position += v;
        spriteRenderer.sprite = eatingSprite;
        yield return new WaitForSeconds(.5f);
        spriteWrapper.transform.position = origin;
        StartCoroutine(ChewAnimation());
        yield return new WaitForSeconds(chewingTime);
        isChewing = false;
        StopCoroutine("ChewingAnimation");
        spriteRenderer.sprite = defaultSprite;
    }

    private IEnumerator ChewAnimation()
    {
        int frameIndex = 0;
        float chewingFPS = 6f;
        while (isChewing) {
            spriteRenderer.sprite = chewingSprites[frameIndex];
            frameIndex = (frameIndex + 1) % chewingSprites.Length;
            yield return new WaitForSeconds(1f/chewingFPS);
        }
    }
}

