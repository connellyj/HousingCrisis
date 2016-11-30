using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class House : MonoBehaviour {

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

    private List<Person> allPeople;
    private List<Person> toRemove;

    protected int[] gridPos;
    protected float eatRadius = 0.4f;
    public bool isChewing = false;

    protected virtual void Awake() {
        if(cost == 0) cost = houseCost;
        gridPos = new int[2] { (int)Mathf.Round(transform.position.x), (int)Mathf.Round(transform.position.y) };
        spriteRenderer = spriteWrapper.GetComponent<SpriteRenderer>();
        ActivateAbility();
    }

    protected virtual void ActivateAbility() {
        return;
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
            if(!adjacent.Contains(t.GetComponent<EatingArea>().direction)) Destroy(t.gameObject);
        }
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
        for(int i = 0; i < 4; i++)
        {
            GameObject eatingArea = transform.GetChild(i).gameObject;
            eatingArea.SetActive(true);
        }
    }

    private void DisableEatingAreas()
    {
        for(int i = 0; i < 4; i++)
        {
            GameObject eatingArea = transform.GetChild(i).gameObject;
            eatingArea.SetActive(false);
        }
    }
}

