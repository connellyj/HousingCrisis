using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class House : Builder {

    // static info
    public static readonly int cost = 10;
    public static readonly int MAX_STALL = 3;
    public static readonly int alertRadius = 5;
    protected static readonly int healingPerTap = 20;
    protected static readonly float chewingTime = 5;
    protected static readonly float attritionDPS = 20f;
    public static readonly float eatRadius = 0.5f;
    private static readonly float personStallOffset = 0.3f;
    private static readonly float houseStallOffset = 0.7f;

    // sprites and renderer
    protected SpriteRenderer spriteRenderer;
    public GameObject spriteWrapper;
    public Sprite defaultSprite;
    public Sprite eatingSprite;
    public Sprite[] chewingSprites = new Sprite[4];

    // stalled people info
    protected List<Person> stalledPeople;
    protected Dictionary<int, Vector3[]> stalledPositions;
    [HideInInspector] public int numStalled = 0;

    // house info
    protected int[] gridPos;
    protected int gridIndex;
    protected int numNonEatAreaChildren = 2;
    protected bool isChewing = false;
    private List<Direction> adjacentPaths;
    protected bool hasSprinklers;

    // fire info
    protected int burnState = 0;
    private int totalDamage = 0;
    private List<GameObject> fires = new List<GameObject>();
    private Vector3 fireOffset = new Vector3(0,0.4f,0);
    private GameObject firePrefab;
    protected GameObject waterDrop;

    protected virtual void Awake() {
        gridPos = new int[2] { (int)Mathf.Round(transform.position.x), (int)Mathf.Round(transform.position.y) };
        gridIndex = GridManager.CoordsToIndex(X(), Y());
        spriteRenderer = spriteWrapper.GetComponent<SpriteRenderer>();
        stalledPeople = new List<Person>(MAX_STALL) {null, null, null};
        stalledPositions = new Dictionary<int, Vector3[]>();
        adjacentPaths = GridManager.GetAdjacentPathDirections(X(), Y());
    }
    
    protected virtual void Start() {
        firePrefab = HouseManager.GetFirePrefab();
        HouseManager.AddHouse(this);
        Buy();
        RemoveTriggers();
        CalculateStallPositions();
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

    private void CalculateStallPositions() {
        foreach(Direction d in adjacentPaths) {
            Vector3[] positions = new Vector3[MAX_STALL];
            switch(d) {
                case Direction.EAST:
                    for(int i = 0; i < MAX_STALL; i++) {
                        positions[i] = new Vector3(transform.position.x + houseStallOffset, transform.position.y + ((i - 1) * personStallOffset));
                    }
                    stalledPositions.Add(gridIndex + 1, positions);
                    break;
                case Direction.WEST:
                    for(int i = 0; i < MAX_STALL; i++) {
                        positions[i] = new Vector3(transform.position.x - houseStallOffset, transform.position.y + ((i - 1) * personStallOffset));
                    }
                    stalledPositions.Add(gridIndex - 1, positions);
                    break;
                case Direction.NORTH:
                    for(int i = 0; i < MAX_STALL; i++) {
                        positions[i] = new Vector3(transform.position.x + ((i - 1) * personStallOffset), transform.position.y + houseStallOffset);
                    }
                    stalledPositions.Add(gridIndex - GridManager.MAX_COL, positions);
                    break;
                case Direction.SOUTH:
                    for(int i = 0; i < MAX_STALL; i++) {
                        positions[i] = new Vector3(transform.position.x + ((i - 1) * personStallOffset), transform.position.y - houseStallOffset);
                    }
                    stalledPositions.Add(gridIndex + GridManager.MAX_COL, positions);
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
            Population.AlertPeopleAffectedByEat(d, X(), Y());
        }
    }

    public void RemoveTriggers() {
        foreach(Transform t in transform) {
            EatingArea e = t.GetComponent<EatingArea>();
            if(e != null) {
                if(!adjacentPaths.Contains(e.direction)) Destroy(t.gameObject);
            }
        }
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
            for(int i = 0; i < transform.childCount - numNonEatAreaChildren; i++)
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
            for(int i = 0; i < transform.childCount - numNonEatAreaChildren; i++)
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
            totalDamage = (hasSprinklers) ? (-1 * Mansion.sprinklerStrength) : 0;
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
            } else if (oldState <= 0 && burnState > 0) {
                StartBurning();
            } else if (oldState >= 0 && burnState <= 0) {
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
        if (totalDamage < 0)
        {
            return (hasSprinklers) ? -1 : 0;
        } else {
            return (int)Math.Floor(totalDamage / 100f);
        }
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
        foreach(Person p in stalledPeople) if(p != null) p.UnHighlight();
        Destroy(gameObject);
    }

    public bool HasAvailableStallSpace() {
        return numStalled < MAX_STALL;
    }

    public Vector3 AddStalledPerson(Person p) {
        numStalled++;
        for(int i = 0; i < stalledPeople.Count; i++) {
            if(stalledPeople[i] == null) {
                stalledPeople[i] = p;
                Vector3[] value;
                if(stalledPositions.TryGetValue(GridManager.CoordsToIndex(p.X(), p.Y()), out value)) return value[i];
            }
        }
        return Vector3.zero;
    }

    public void RemoveStalledPerson(Person p) {
        numStalled--;
        for(int i = 0; i < stalledPeople.Count; i++) {
            if(stalledPeople[i] == p) stalledPeople[i] = null;
        }
    }

    public void SprinklersOn(bool turnOn)
    {
        if (!hasSprinklers && turnOn)
        {
            totalDamage -= Mansion.sprinklerStrength;
            hasSprinklers = true;
            OnDamageOrHeal();
        } else if (hasSprinklers && !turnOn) {
            if (totalDamage < 0) totalDamage = 0;
            hasSprinklers = false;
            OnDamageOrHeal();
        }
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

