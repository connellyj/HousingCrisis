using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class House : MonoBehaviour {

    private static readonly int MAX_STALL = 3;

    public int houseCost;
    public int noticeThreshold;

    public static int cost = 0;

    public float chewingTime;

    // sprites and renderer
    SpriteRenderer spriteRenderer;
    public GameObject spriteWrapper;
    public Sprite defaultSprite;
    public Sprite eatingSprite;
    public Sprite[] chewingSprites = new Sprite[4];

    protected Person[] stalledPeople;
    protected Dictionary<int, Vector3[]> stalledPositions;

    protected int[] gridPos;
    protected float eatRadius = 0.5f;
    public bool isChewing = false;

    protected virtual void Awake() {
        if(cost == 0) cost = houseCost;
        gridPos = new int[2] { (int)Mathf.Round(transform.position.x), (int)Mathf.Round(transform.position.y) };
        spriteRenderer = spriteWrapper.GetComponent<SpriteRenderer>();
        stalledPeople = new Person[MAX_STALL];
        stalledPositions = new Dictionary<int, Vector3[]>();
    }

    private void CalculateStallPositions(List<Direction> adjPaths) {
        int houseIndex = GridManager.CoordsToIndex(gridPos[0], gridPos[1]);
        foreach(Direction d in adjPaths) {
            Vector3[] positions = new Vector3[MAX_STALL];
            switch(d) {
                case Direction.EAST:
                    for(int i = 0; i < MAX_STALL; i++) {
                        positions[i] = new Vector3(transform.position.x - 0.5f, transform.position.y + ((i - 1) * 0.3f));
                    }
                    stalledPositions.Add(houseIndex + 1, positions);
                    break;
                case Direction.WEST:
                    for(int i = 0; i < MAX_STALL; i++) {
                        positions[i] = new Vector3(transform.position.x + 0.5f, transform.position.y + ((i - 1) * 0.3f));
                    }
                    stalledPositions.Add(houseIndex - 1, positions);
                    break;
                case Direction.NORTH:
                    for(int i = 0; i < MAX_STALL; i++) {
                        positions[i] = new Vector3(transform.position.x + ((i - 1) * 0.3f), transform.position.y - 0.5f);
                    }
                    stalledPositions.Add(houseIndex - GridManager.MAX_COL, positions);
                    break;
                case Direction.SOUTH:
                    for(int i = 0; i < MAX_STALL; i++) {
                        positions[i] = new Vector3(transform.position.x + ((i - 1) * 0.3f), transform.position.y + 0.5f);
                    }
                    stalledPositions.Add(houseIndex + GridManager.MAX_COL, positions);
                    break;
            }
        }
    }

    public void Buy() {
        GameManager.UpdateMoney(-1 * houseCost);
    }

    public void Eat(Direction d) {
        if (!isChewing)
        {
            isChewing = true;
            DisableEatingAreas();
            StartCoroutine(EatAnimation(d));
            Population.AlertAffectedPeople(d, gridPos, eatRadius, noticeThreshold);
        }
    }

    public void RemoveTriggers(List<Direction> adjacent) {
        foreach(Transform t in transform) {
            EatingArea e = t.GetComponent<EatingArea>();
            if(e != null) {
                if(!adjacent.Contains(e.direction)) Destroy(t.gameObject);
            }
        }
        CalculateStallPositions(adjacent);
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
        EnableEatingAreas();
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

    private void EnableEatingAreas()
    {
        for(int i = 0; i < transform.childCount - 1; i++)
        {
            GameObject eatingArea = transform.GetChild(i).gameObject;
            eatingArea.SetActive(true);
        }
    }

    private void DisableEatingAreas()
    {
        for(int i = 0; i < transform.childCount - 1; i++)
        {
            GameObject eatingArea = transform.GetChild(i).gameObject;
            eatingArea.SetActive(false);
        }
    }

    public bool HasAvailableStallSpace() {
        return stalledPeople.Length < MAX_STALL;
    }

    public Vector3 AddStalledPerson(int x, int y) {
        for(int i = 0; i < MAX_STALL; i++) {
            if(stalledPeople[i] == null) {
                int idx = GridManager.CoordsToIndex(x, y);
                Vector3[] value;
                if(stalledPositions.TryGetValue(idx, out value)) return value[i];
            }
        }
        return Vector3.zero;
    }
}

