using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Person : MonoBehaviour {

	// script component variables
	public Direction direction;
    public int value;
	public float speed;
	public float alertSpeed;
	public float animationFPS;
	private float motionFPS = 30;
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
	public enum PersonState { WANDER, PANIC, TARGET, NONE }
	public PersonState state = PersonState.WANDER;
	private List<Direction> path = new List<Direction>();
	private int pathIndex = 0;
	public Vector3 gridXY;
	public static Vector3 positionOffset = new Vector3(0,0.25f,0);

    private Population population;

    // HEY CONNOR!!
    // You have to account for the case where the person is on an exit when they panic
    // Because in that case the path will be an empty list
    // This causes problems with at least line 95
    // I didn't wanna try and fix it because I don't entirely know what's going on
	void Start () {
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
        population.AddPerson(GetComponent<Person>());
	}
	
	void Update () {
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

	private void ChangeState(PersonState newState)
	{
		StopAllCoroutines();
		StartCoroutine(PlayAnimation());
		state = newState;
		SetPath();
		if (path == null)
		{
			CompletePath();
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

	private void CompletePath()
	{
		StopAllCoroutines();
        population.RemovePerson(GetComponent<Person>());
        if(state == PersonState.PANIC) GameManager.UpdateWantedLevel(1);
        Destroy(gameObject);
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
				throw new System.InvalidOperationException("Direction cannot be converted to vector");
		}
	}

    private void SetPath()
    {
    	int personLoc = PersonLocFromPosition();
    	Pathfinder.GoalType goal = Pathfinder.GoalType.EXIT;
    	path = GameManager.FindPath(state, personLoc, goal);
    }

    private int PersonLocFromPosition()
    {
    	return (int)((GridManager.MAX_ROW - 1 - gridXY[1]) * GridManager.MAX_COL + gridXY[0]);
    }

    private void LogPath()
    {
        //Debug.Log("PATH =");
        Direction last = path[0];
    	int c = 0;
    	for (int i = 0; i < path.Count; i++)
    	{
    		Direction next = path[i];
    		if (next != last)
    		{
    			//Debug.LogFormat("   {0} {1}", last, c);
    			last = next;
    			c = 0;
    		}
    		c++;
    	}
    	//Debug.LogFormat("   {0} {1}", path[path.Count - 1], c);
    }

    private IEnumerator PlayAnimation(){
        int frameIndex = 0;
        while (true) {
        	spriteRenderer.sprite = spritesByDirection[(int)direction][frameIndex];
			frameIndex = (frameIndex + 1) % 2;
        	yield return new WaitForSeconds(1f/animationFPS);
        }
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

    public void OnSeeHouse() {
        Panic();
    }

    public int X() {
        return (int)gridXY[0];
    }

    public int Y() {
        return (int)gridXY[1];
    }
}
