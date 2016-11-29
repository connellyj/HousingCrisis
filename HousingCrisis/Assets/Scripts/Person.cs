using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Person : MonoBehaviour {

	// script component variables
	public Direction direction;
    public float stallTime;
    public int value;
	public float speed;
	public float alertSpeed;
	public float animationFPS;
	private float motionFPS = 30;
    protected int attackGoal;
	// component references
	SpriteRenderer spriteRenderer;
	// directional sprites
	public Sprite spriteSouthA;
	public Sprite spriteSouthB;
	public Sprite spriteWestA;
	public Sprite spriteWestB;
	public Sprite spriteEastA;
	public Sprite spriteEastB;
	public Sprite spriteNorthA;
	public Sprite spriteNorthB;
	// sprite arrays
	private Sprite[] northSprites;
	private Sprite[] southSprites;
	private Sprite[] westSprites;
	private Sprite[] eastSprites;
	private Sprite[][] spritesByDirection;
	// AI and pathing
	public enum PersonState { WANDER, PANIC, TARGET, NONE, STALL, ATTACK }
    public PersonState state;
	protected List<Direction> path = new List<Direction>();
	private int pathIndex = 0;
	public Vector3 gridXY;
	public static Vector3 positionOffset = new Vector3(0,0.25f,0);

    protected Population population;

    // HEY CONNOR!!
    // You have to account for the case where the person is on an exit when they panic
    // Because in that case the path will be an empty list
    // This causes problems with at least line 95
    // I didn't wanna try and fix it because I don't entirely know what's going on
	protected virtual void Start () {
        // set and start path
        gridXY[0] = (int)Math.Round(transform.position.x);
    	gridXY[1] = (int)Math.Round(transform.position.y);
    	gridXY[2] = 0;
        SetPath();
        LogPath();
        direction = path[0];
        Vector3 v = DirectionToVector(direction);
        StartCoroutine(FollowPath(v));
        // create sprite arrays and start animation
		northSprites = new Sprite[] {spriteNorthA, spriteNorthB};
		southSprites = new Sprite[] {spriteSouthA, spriteSouthB};
		westSprites = new Sprite[] {spriteWestA, spriteWestB};
		eastSprites = new Sprite[] {spriteEastA, spriteEastB};
		spritesByDirection = new Sprite[][] {northSprites, southSprites, westSprites, eastSprites};
		spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(PlayAnimation());

        population = GameManager.GetPopulation();
        population.AddPerson(this);
	}
	
	protected virtual void Update () {
		if(Input.GetKeyDown(KeyCode.P)) {
			Panic();
		}
	}

	public void Panic()
	{
		if (state != PersonState.PANIC)
		{
			speed = alertSpeed;
			ChangeState(PersonState.PANIC);
		}
	}

	protected void ChangeState(PersonState newState)
	{
		StopAllCoroutines();
		StartCoroutine(PlayAnimation());
		state = newState;
		SetPath();
		if (path == null) {
			CompletePath();
		} else if(path[0] == Direction.NONE) {
            StopAllCoroutines();
            StartCoroutine(Stall());
        } else {
			FollowNewPath(path[0]);
			LogPath();
		}
	}

	private void FollowNewPath(Direction newDirection)
	{
		Vector3 targetTile = gridXY;
		if (direction == newDirection)
		{
			targetTile += DirectionToVector(direction);
			pathIndex = 0; // replaces first pathing direction
		} else {
			direction = Opposite(direction);
			pathIndex = -1; // does not replace first pathing direction
		}
		Vector3 tileAdjust = targetTile + positionOffset - transform.position;
		StartCoroutine(FollowPath(tileAdjust));
	}

    protected virtual IEnumerator Stall() {
        yield return new WaitForSeconds(stallTime);
        CompletePath();
    }

	private IEnumerator FollowPath(Vector3 v)
	{
		float walkTime = 1f / speed;
		int totalFrames = Mathf.CeilToInt(walkTime * motionFPS);
		for (int i = 0; i < totalFrames; i++)
		{
			transform.position += (v / totalFrames);
			yield return new WaitForSeconds(walkTime / totalFrames);
		}
		SnapPositionToGrid();
		pathIndex++;
		if (pathIndex < path.Count)
		{
			direction = path[pathIndex];
			Vector3 newV = DirectionToVector(direction);
			StartCoroutine(FollowPath(newV));
		} else {
			CompletePath();
		}
	}

	protected virtual void CompletePath()
	{
		StopAllCoroutines();
	}

	private void SnapPositionToGrid()
	{
		gridXY[0] = (int)Math.Round(transform.position.x);
    	gridXY[1] = (int)Math.Round(transform.position.y);
    	gridXY[2] = 0;
		transform.position = gridXY + positionOffset;
	}

	private Direction Opposite(Direction d)
	{
		switch (d) {
			case Direction.NORTH:
				return Direction.SOUTH;
			case Direction.SOUTH:
				return Direction.NORTH;
			case Direction.WEST:
				return Direction.EAST;
			case Direction.EAST:
				return Direction.WEST;
			default:
				throw new System.InvalidOperationException("Direction cannot be converted to opposite");
		}
	}

	private Vector3 DirectionToVector(Direction d)
	{
		switch (d) {
			case Direction.NORTH:
				return Vector3.up;
			case Direction.SOUTH:
				return Vector3.down;
			case Direction.WEST:
				return Vector3.left;
			case Direction.EAST:
				return Vector3.right;
			default:
				throw new InvalidOperationException("Direction cannot be converted to vector");
		}
	}

    private void SetPath()
    {
    	int personLoc = PersonLocFromPosition();
    	if(state == PersonState.ATTACK) path = Pathfinder.FindPath(personLoc, attackGoal);
        else if(state == PersonState.TARGET) {
            if(GridManager.houses.Count == 0) {
                ChangeState(PersonState.WANDER);
            } else {
                attackGoal = GridManager.houses[UnityEngine.Random.Range(0, GridManager.houses.Count)];
                path = Pathfinder.FindPath(personLoc, attackGoal);
            }
        } else path = Pathfinder.FindPath(state, personLoc);
    }

    protected int PersonLocFromPosition()
    {
        return GridManager.coordsToIndex((int)gridXY[0], (int)gridXY[1]);
    }

    private void LogPath()
    {
        Direction last = path[0];
    	int c = 0;
    	for (int i = 0; i < path.Count; i++)
    	{
    		Direction next = path[i];
    		if (next != last)
    		{
    			last = next;
    			c = 0;
    		}
    		c++;
    	}
    }

    private IEnumerator PlayAnimation(){
        int frameIndex = 0;
        while (true) {
        	spriteRenderer.sprite = spritesByDirection[(int)direction][frameIndex];
			frameIndex = (frameIndex + 1) % 2;
        	yield return new WaitForSeconds(1f/animationFPS);
        }
    }

    protected void RemovePerson() {
        population.RemovePerson(this);
        Destroy(gameObject);
    }

    public void Highlight() {
        spriteRenderer.color = Color.red;
    }

    public void UnHighlight() {
        spriteRenderer.color = Color.white;
    }

    public void OnEaten() {
        GameManager.UpdateMoney(value);
        Destroy(gameObject);
    }

    public virtual void OnSeeHouse(int houseIndex) {
        Panic();
    }

    public int X() {
        return (int)gridXY[0];
    }

    public int Y() {
        return (int)gridXY[1];
    }
}
