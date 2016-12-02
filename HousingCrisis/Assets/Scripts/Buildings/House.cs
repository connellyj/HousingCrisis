using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class House : Builder {

    // static info
    public static readonly int cost = 10;
    protected static readonly int MAX_STALL = 3;
    protected static readonly int alertRadius = 5;
    protected static readonly int healingPerTap = 20;
    protected static readonly float chewingTime = 5;
    protected static readonly float attritionDPS = 20f;

    // sprites and renderer
    protected SpriteRenderer spriteRenderer;
    public GameObject spriteWrapper;
    public Sprite defaultSprite;
    public Sprite eatingSprite;
    public Sprite[] chewingSprites = new Sprite[4];

    // stalled people info
    protected Person[] stalledPeople;
    protected Dictionary<int, Vector3[]> stalledPositions;
    protected int numStalled = 0;

    // house info
    protected int[] gridPos;
    protected float eatRadius = 0.5f;
    protected bool isChewing = false;

    // fire info
    protected int burnState = 0;
    private int totalDamage = 0;
    private List<GameObject> fires = new List<GameObject>();
    private Vector3 fireOffset = new Vector3(0,0.4f,0);
    private int minDamage = 0;

    private bool hasSprinklers;

    private GameObject firePrefab;
    private GameObject waterDrop;

    protected virtual void Awake() {
        gridPos = new int[2] { (int)Mathf.Round(transform.position.x), (int)Mathf.Round(transform.position.y) };
        spriteRenderer = spriteWrapper.GetComponent<SpriteRenderer>();
        stalledPeople = new Person[MAX_STALL];
        stalledPositions = new Dictionary<int, Vector3[]>();
    }
    
    protected virtual void Start() {
        firePrefab = HouseManager.GetFirePrefab();
        SetUpSprinklers();
    }

    private void SetUpSprinklers()
    {
        bool adjacentToMansion = HouseManager.isAdjacentToMansion(this);
        waterDrop = transform.GetChild(4).gameObject;
        waterDrop.SetActive(adjacentToMansion);
        SprinklersOn(adjacentToMansion);
    }

    void OnMouseDown() 
    {
        if (burnState <= 0)
        {
            // display build options
            BuildMenu.Open(this);
        } else {
            // put out fire
            HealHouse(healingPerTap);
        }
    }

    private void CalculateStallPositions(List<Direction> adjPaths) {
        int houseIndex = GridManager.CoordsToIndex(gridPos[0], gridPos[1]);
        foreach(Direction d in adjPaths) {
            Vector3[] positions = new Vector3[MAX_STALL];
            switch(d) {
                case Direction.EAST:
                    for(int i = 0; i < MAX_STALL; i++) {
                        positions[i] = new Vector3(transform.position.x + 0.6f, transform.position.y + ((i - 1) * 0.4f));
                    }
                    stalledPositions.Add(houseIndex + 1, positions);
                    break;
                case Direction.WEST:
                    for(int i = 0; i < MAX_STALL; i++) {
                        positions[i] = new Vector3(transform.position.x - 0.6f, transform.position.y + ((i - 1) * 0.4f));
                    }
                    stalledPositions.Add(houseIndex - 1, positions);
                    break;
                case Direction.NORTH:
                    for(int i = 0; i < MAX_STALL; i++) {
                        positions[i] = new Vector3(transform.position.x + ((i - 1) * 0.4f), transform.position.y + 0.6f);
                    }
                    stalledPositions.Add(houseIndex - GridManager.MAX_COL, positions);
                    break;
                case Direction.SOUTH:
                    for(int i = 0; i < MAX_STALL; i++) {
                        positions[i] = new Vector3(transform.position.x + ((i - 1) * 0.4f), transform.position.y - 0.6f);
                    }
                    stalledPositions.Add(houseIndex + GridManager.MAX_COL, positions);
                    break;
            }
        }
    }

    public void Buy() {
        GameManager.UpdateMoney(-1 * cost);
    }

    public bool CanEat()
    {
        return (!isChewing) && (burnState <= 0);
    }

    public virtual void Eat(Direction d) {
        if (CanEat())
        {
            isChewing = true;
            DisableEatingAreas();
            StartCoroutine(EatAnimation(d));
            Population.AlertPeopleAffectedByEat(d, gridPos, eatRadius, alertRadius);
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

    protected virtual IEnumerator EatAnimation(Direction d)
    {
        Vector3 origin = spriteWrapper.transform.position;
        Vector3 v = GridManager.DirectionToVector(d) / 2;   
        spriteWrapper.transform.position += v;
        spriteRenderer.sprite = eatingSprite;
        yield return new WaitForSeconds(.5f);
        spriteWrapper.transform.position = origin;
        StartCoroutine(ChewAnimation());
        yield return new WaitForSeconds(chewingTime - ChewTimeOffset());
        isChewing = false;
        StopCoroutine("ChewingAnimation");
        spriteRenderer.sprite = defaultSprite;
        EnableEatingAreas();
    }

    protected IEnumerator ChewAnimation()
    {
        int frameIndex = 0;
        float chewingFPS = 6f;
        while (isChewing) {
            spriteRenderer.sprite = chewingSprites[frameIndex];
            frameIndex = (frameIndex + 1) % chewingSprites.Length;
            yield return new WaitForSeconds(1f/chewingFPS);
        }
    }

    protected void EnableEatingAreas()
    {
        if (CanEat()) 
        {   
            for(int i = 0; i < transform.childCount - 1; i++)
            {
                GameObject eatingArea = transform.GetChild(i).gameObject;
                eatingArea.SetActive(true);
            }
        }
    }

    protected void DisableEatingAreas()
    {
        if (!CanEat())
        {
            for(int i = 0; i < transform.childCount - 1; i++)
            {
                GameObject eatingArea = transform.GetChild(i).gameObject;
                eatingArea.SetActive(false);
            }
        }
    }

    public void RobHouse(int minRobberDamage)
    {
        if (totalDamage < 100)
        {
            int damageToBurn = 100 - totalDamage;
            if (damageToBurn > 100) damageToBurn = 100;
            DamageHouse(damageToBurn);
        } else {
            DamageHouse(minRobberDamage);
        }
    }

    public void DamageHouse(int damage)
    {
        totalDamage += damage;
        OnDamageOrHeal();
    }

    public void HealHouse(int damageHealed)
    {
        totalDamage -= damageHealed;
        if (totalDamage < 100) {
            totalDamage = minDamage;
        }
        OnDamageOrHeal();
    }

    private void OnDamageOrHeal()
    {
        if (DidBurnStateChange())
        {
            int oldState = burnState;
            burnState = CorrectBurnState();
            if (burnState > 3) 
            {
                RemoveHouse();
            } else if (oldState > 0) {
                StartBurning();
            } else if (burnState <= 0) {
                StopBurning();
            }
            UpdateFires();
        }
    }

    private bool DidBurnStateChange()
    {
        int correctBurnState = CorrectBurnState();
        return correctBurnState != burnState;
    }

    private int CorrectBurnState()
    {
        int correctState = (int)Math.Floor(totalDamage / 100f);
        if (correctState <= 0)
        {
            correctState = hasSprinklers ? -1 : 0;
        }
        return correctState;
    }

    private void StartBurning() 
    {
        DisableEatingAreas();
        HouseManager.AddBurningHouse(this);
        StartCoroutine(BurnDown());
    }

    private void StopBurning() 
    {
        EnableEatingAreas();
        HouseManager.RemoveBurningHouse(this);
        StopCoroutine("BurnDown");
    }

    private IEnumerator BurnDown()
    {
        while (burnState > 0)
        {
            DamageHouse(1);
            yield return new WaitForSeconds(1f / attritionDPS);
        }
    }

    private void UpdateFires()
    {
        RemoveAllFires();
        waterDrop.SetActive(false);
        switch(burnState) {
            case -1:
                waterDrop.SetActive(true);
                break;
            case 1:
                AddFireWithOffset(Vector3.zero);
                break;
            case 2:
                AddFireWithOffset(Vector3.left / 8);
                AddFireWithOffset(Vector3.right / 8);
                break;
            case 3:
                AddFireWithOffset(Vector3.left / 4);
                AddFireWithOffset(Vector3.zero);
                AddFireWithOffset(Vector3.right / 4);
                break;
            default: break;
        }
    }

    private void AddFireWithOffset(Vector3 individualOffset)
    {
        Vector3 firePosition = transform.position + individualOffset + fireOffset;
        GameObject fire = (GameObject)Instantiate(firePrefab, firePosition, Quaternion.Euler(0,0,270));
        fires.Add(fire);
    }

    private void RemoveAllFires()
    {
        for (int i = 0; i < fires.Count; i++)
        {
            GameObject fire = fires[i];
            Destroy(fire);
        }
        fires.Clear();
    }

    public override void OnBuild()
    {
        RemoveHouse();
    }

    protected virtual void RemoveHouse() {
        HouseManager.RemoveHouse(this);
        Destroy(gameObject);
    }

    public bool HasAvailableStallSpace() {
        return numStalled < MAX_STALL;
    }

    public Vector3 AddStalledPerson(Person p) {
        numStalled++;
        for(int i = 0; i < stalledPeople.Length; i++) {
            if(stalledPeople[i] == null) {
                stalledPeople[i] = p;
                int idx = GridManager.CoordsToIndex(p.X(), p.Y());
                Vector3[] value;
                if(stalledPositions.TryGetValue(idx, out value)) return value[i];
            }
        }
        return Vector3.zero;
    }

    public void RemoveStalledPerson(Person p) {
        numStalled--;
        p.ResetPosition();
        for(int i = 0; i < stalledPeople.Length; i++) {
            if(stalledPeople[i] == p) stalledPeople[i] = null;
        }
    }

    public void SprinklersOn(bool turnOn)
    {
        if (!hasSprinklers && turnOn)
        {
            totalDamage -= Mansion.sprinklerStrength;
            OnDamageOrHeal();
        } else if (hasSprinklers && !turnOn) {
            if (totalDamage < 0) totalDamage = 0;
            OnDamageOrHeal();
        }
        hasSprinklers = turnOn;
    }

    public int X() {
        return gridPos[0];
    }

    public int Y() {
        return gridPos[1];
    }

    protected virtual float ChewTimeOffset() {
        return 0f;
    }
}

