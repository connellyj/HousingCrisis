﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class House : Builder {

    // static info
    public static readonly int MAX_STALL = 3;
    public static readonly int alertRadius = 5;
    protected static readonly int healingPerTap = 80;
    protected static readonly float chewingTime = 5;
    protected static readonly float attritionDPS = 20f;
    public static readonly float eatRadius = 0.5f;
    private static readonly float personStallOffset = 0.3f;
    private static readonly float houseStallOffset = 0.9f;
    // sprites and renderer
    protected SpriteRenderer spriteRenderer;
    public GameObject spriteWrapper;
    public Sprite defaultSprite;
    public Sprite eatingSprite;
    public Sprite[] chewingSprites = new Sprite[4];
    // stalled people info
    [HideInInspector] public List<Person> stalledPeople;
    protected Dictionary<int, Vector3[]> stalledPositions;
    [HideInInspector] public int numStalled = 0;
    // house info
    protected int[] gridPos;
    [HideInInspector] public int gridIndex;
    protected bool isChewing = false;
    private List<Direction> adjacentPaths;
    protected bool hasSprinklers;
    protected int numNonEatAreaChildren = 3;
    // fire info
    public int burnState = 0;
    public int totalDamage = 0;
    private List<GameObject> fires = new List<GameObject>();
    private Vector3 fireOffset = new Vector3(0,0.4f,0);
    private GameObject firePrefab;
    protected GameObject waterDrop;
    private Sprite[] smokeSprites;
    private bool isSmoking = false;
    private float smokeTime;
    public GameObject smokeWrapper;
    private SpriteRenderer smokeRenderer;

    protected virtual void Awake() {
        gridPos = new int[2] { (int)Mathf.Round(transform.position.x), (int)Mathf.Round(transform.position.y) };
        gridIndex = GridManager.CoordsToIndex(X(), Y());
        spriteRenderer = spriteWrapper.GetComponent<SpriteRenderer>();
        smokeRenderer = smokeWrapper.GetComponent<SpriteRenderer>();
        smokeWrapper.SetActive(false);
        stalledPeople = new List<Person>(MAX_STALL) {null, null, null};
        stalledPositions = new Dictionary<int, Vector3[]>();
        adjacentPaths = GridManager.GetAdjacentPathDirections(X(), Y());
    }
    
    protected virtual void Start() {
        firePrefab = HouseManager.GetFirePrefab();
        smokeTime = HouseManager.GetSmokeTime();
        smokeSprites = HouseManager.GetSmokeSprites();
        HouseManager.AddHouse(this);
        Buy();
        RemoveTriggers();
        CalculateStallPositions();
        SetUpSprinklers();
    }

    // When clicked, opens a menu or heals the house
    void OnMouseDown() {
        if(burnState <= 0 && !isSmoking) {
            // display build options
            if (type == HouseManager.HouseType.HOUSE || 
                type == HouseManager.HouseType.STORE) BuildMenu.Open(this);
        } else {
            // put out fire
            HealHouse(healingPerTap);
        }
    }

    // Updates the money based on the house cost
    public void Buy() {
        GameManager.UpdateMoney(-1 * HouseManager.GetCost(type));
    }

    // Removes all the eatingArea triggers that aren't over a path
    public void RemoveTriggers() {
        foreach(Transform t in transform) {
            EatingArea e = t.GetComponent<EatingArea>();
            if(e != null) {
                if(!adjacentPaths.Contains(e.direction)) Destroy(t.gameObject);
            }
        }
    }

    // Calculates all the vector3 positions where people can stall outside this house
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
    
    // Initializes the sprinklers
    private void SetUpSprinklers() {
        bool adjacentToMansion = HouseManager.isAdjacentToMansion(this);
        waterDrop = transform.GetChild(4).gameObject;
        waterDrop.SetActive(adjacentToMansion);
        SprinklersOn(adjacentToMansion);
    }

    // Returns whether or not the house can eat
    public bool CanEat() {
        return (!isChewing) && (burnState <= 0);
    }

    // Makes the house eat
    public virtual void Eat(Direction d) {
        if (CanEat()) {
            isChewing = true;
            DisableEatingAreas();
            StartCoroutine(EatAnimation(d));
            Population.AlertPeopleAffectedByEat(d, X(), Y());
        }
    }

    // Animates the house while it eats
    protected virtual IEnumerator EatAnimation(Direction d) {
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

    // Animates the house while it chews
    protected IEnumerator ChewAnimation() {
        int frameIndex = 0;
        float chewingFPS = 6f;
        while (isChewing) {
            spriteRenderer.sprite = chewingSprites[frameIndex];
            frameIndex = (frameIndex + 1) % chewingSprites.Length;
            yield return new WaitForSeconds(1f/chewingFPS);
        }
    }

    // Enables the eating areas
    protected void EnableEatingAreas() {
        if (CanEat()) {   
            for(int i = 0; i < transform.childCount - numNonEatAreaChildren; i++) {
                GameObject eatingArea = transform.GetChild(i).gameObject;
                eatingArea.SetActive(true);
            }
        }
    }

    // Disables the eating areas
    protected void DisableEatingAreas() {
        if (!CanEat()) {
            for(int i = 0; i < transform.childCount - numNonEatAreaChildren; i++) {
                GameObject eatingArea = transform.GetChild(i).gameObject;
                eatingArea.SetActive(false);
            }
        }
    }

    // If not burning, sets the house on fire, otherwise just damages it
    public void RobHouse(int minRobberDamage) {
        if (totalDamage < 100) {
            int damageToBurn = 100 - totalDamage;
            if (damageToBurn > 100) damageToBurn = 100;
            DamageHouse(damageToBurn);
        } else {
            DamageHouse(minRobberDamage);
        }
    }

    // Damages the house the given amount
    public void DamageHouse(int damage) {
        totalDamage += damage;
        OnDamageOrHeal();
    }

    // Heals the house the given amount
    public void HealHouse(int damageHealed) {
        totalDamage -= damageHealed;
        if (totalDamage < 100) {
            totalDamage = (hasSprinklers) ? (-1 * Mansion.sprinklerStrength) : 0;
        }
        OnDamageOrHeal();
    }

    // Updates the burning state if needed
    private void OnDamageOrHeal() {
        if (DidBurnStateChange()) {
            int oldState = burnState;
            burnState = CorrectBurnState();
            if (burnState > 3) {
                RemoveHouse();
            } else if (oldState <= 0 && burnState > 0) {
                StartBurning();
            } else if (oldState >= 0 && burnState <= 0) {
                StopBurning();
            }
            UpdateFires();
        }
    }

    // Returns whether or not the burn state has changed
    private bool DidBurnStateChange() {
        int correctBurnState = CorrectBurnState();
        return correctBurnState != burnState;
    }

    // Returns the currect burn state based on damage and sprinklers
    private int CorrectBurnState() {
        if (totalDamage < 0) {
            return (hasSprinklers) ? -1 : 0;
        } else {
            return (int)Math.Floor(totalDamage / 100f);
        }
    }

    // Makes the house start burning
    private void StartBurning() {
        DisableEatingAreas();
        HouseManager.AddBurningHouse(gridIndex);
        StartCoroutine(BurnDown());
    }
    
    // Makes the house stop burning
    private void StopBurning() {
        isSmoking = true;
        StartCoroutine(StartSmoking());
        HouseManager.RemoveBurningHouse(gridIndex);
    }

    private IEnumerator StartSmoking() {
        smokeWrapper.SetActive(true);
        float time = smokeTime;
        int spriteIndex = 0;
        while(time > 0) {
            smokeRenderer.sprite = smokeSprites[spriteIndex % smokeSprites.Length];
            spriteIndex++;
            time -= 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
        isSmoking = false;
        smokeWrapper.SetActive(false);
        EnableEatingAreas();
    }

    // Slowly damages the house while it burns
    private IEnumerator BurnDown() {
        while (burnState > 0) {
            DamageHouse(1);
            yield return new WaitForSeconds(1f / attritionDPS);
        }
    }

    // Updates the fire and water drop sprites based on burn state
    private void UpdateFires() {
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

    // Creates a fire object at the given distance away from the house's center
    private void AddFireWithOffset(Vector3 individualOffset) {
        Vector3 firePosition = transform.position + individualOffset + fireOffset;
        GameObject fire = (GameObject)Instantiate(firePrefab, firePosition, Quaternion.Euler(0,0,270));
        fires.Add(fire);
    }

    // Removes all the fire objetcs
    private void RemoveAllFires() {
        for (int i = 0; i < fires.Count; i++) {
            GameObject fire = fires[i];
            Destroy(fire);
        }
        fires.Clear();
    }

    // Called when something is built on the area
    public override void OnBuild() {
        Destroy(gameObject);
    }

    // Removes and destroys the house
    protected virtual void RemoveHouse() {
        HouseManager.RemoveHouse(this);
        if (type == HouseManager.HouseType.BANK) GameManager.UpdateBankerChance(-1);
        foreach(Person p in stalledPeople) if(p != null) p.UnHighlight();
        Destroy(gameObject);
    }

    // Returns whether or not there's an open place in front of the house
    public bool HasAvailableStallSpace() {
        return numStalled < MAX_STALL;
    }

    // Returns the location the new stalled person should move to
    public Vector3 AddStalledPerson(Person p) {
        stalledPeople.Add(p);
        numStalled++;
        return stalledPositions[GridManager.CoordsToIndex(p.X(), p.Y())][numStalled - 1];
    }

    // Removes the given person when they're no longer stalled
    public void RemoveStalledPerson(Person p) {
        if(stalledPeople.Remove(p)) numStalled--;
    }

    // Turns the sprinklers on or off based on whether the house has sprinklers
    public void SprinklersOn(bool turnOn) {
        if (!hasSprinklers && turnOn) {
            totalDamage -= Mansion.sprinklerStrength;
            hasSprinklers = true;
            OnDamageOrHeal();
        } else if (hasSprinklers && !turnOn) {
            if (totalDamage < 0) totalDamage = 0;
            hasSprinklers = false;
            OnDamageOrHeal();
        }
    }

    // Returns the grid x position of the house
    public int X() {
        return gridPos[0];
    }

    // Returns the grid y position of the house
    public int Y() {
        return gridPos[1];
    }

    // Used to offset the chewing time in inherited classes
    protected virtual float ChewTimeOffset() {
        return 0f;
    }
}