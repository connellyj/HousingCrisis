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
    protected int goalIndex;
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
	public enum PersonState { WANDER, PANIC, TARGET_RANDOM, NONE, STALL, TARGET_SET, ATTACK, WANDER_SET }
    public PersonState state;
	protected List<Direction> path = new List<Direction>();
	private int pathIndex = 0;
	public Vector3 gridXY;
	public static Vector3 positionOffset = new Vector3(0,0.25f,0);
    private Vector3 prevPos;

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
        Vector3 v = GridManager.DirectionToVector(direction);
        StartCoroutine(FollowPath(v));
        // create sprite arrays and start animation
		northSprites = new Sprite[] {spriteNorthA, spriteNorthB};
		southSprites = new Sprite[] {spriteSouthA, spriteSouthB};
		westSprites = new Sprite[] {spriteWestA, spriteWestB};
		eastSprites = new Sprite[] {spriteEastA, spriteEastB};
		spritesByDirection = new Sprite[][] {northSprites, southSprites, westSprites, eastSprites};
		spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(PlayAnimation());

        Population.AddPerson(this);
        gameObject.layer = 2;
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
			targetTile += GridManager.DirectionToVector(direction);
			pathIndex = 0; // replaces first pathing direction
		} else {
			direction = Opposite(direction);
			pathIndex = -1; // does not replace first pathing direction
		}
		Vector3 tileAdjust = targetTile + positionOffset - transform.position;
		StartCoroutine(FollowPath(tileAdjust));
	}

    protected virtual IEnumerator Stall() {
        House h = HouseManager.houses[GridManager.houses.IndexOf(goalIndex)];
        if(h.HasAvailableStallSpace()) {
            MoveToPosition(h.AddStalledPerson(X(), Y()));
            yield return new WaitForSeconds(stallTime);
            ResetPosition();
            CompletePath();
        } else CompletePath();
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
			Vector3 newV = GridManager.DirectionToVector(direction);
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
				throw new InvalidOperationException("Direction cannot be converted to opposite");
		}
	}

    private void SetPath()
    {
    	int personLoc = PersonLocFromPosition();
        switch(state) {
            case PersonState.TARGET_SET:
                path = Pathfinder.FindPathToHouse(personLoc, goalIndex);
                break;
            case PersonState.TARGET_RANDOM:
                if(GridManager.houses.Count == 0) {
                    ChangeState(PersonState.WANDER);
                } else {
                    goalIndex = GridManager.houses[UnityEngine.Random.Range(0, GridManager.houses.Count)];
                    path = Pathfinder.FindPathToHouse(personLoc, goalIndex);
                }
                break;
            case PersonState.STALL:
                StopAllCoroutines();
                StartCoroutine(Stall());
                break;
            case PersonState.ATTACK:
                Attack();
                break;
            case PersonState.WANDER_SET:
                path = Pathfinder.FindPathToHouse(personLoc, goalIndex);
                break;
            default:
                path = Pathfinder.FindPath(state, personLoc);
                break;
        }
    }

    protected int PersonLocFromPosition()
    {
        return GridManager.CoordsToIndex((int)gridXY[0], (int)gridXY[1]);
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

    protected virtual void Attack() {
        return;
    }

    protected void MoveToPosition(Vector3 pos) {
        prevPos = transform.position;
        StartCoroutine(TranslateToPos(pos));
    }

    protected void ResetPosition() {
        MoveToPosition(prevPos);
    }

    private IEnumerator TranslateToPos(Vector3 pos) {
        Vector3 dir = pos - transform.position;
        float dist = dir.magnitude;
        dir = dir.normalized;
        while(dist > 0) {
            transform.Translate(dir * 0.1f);
            dist -= 0.1f;
            yield return new WaitForSeconds(0.5f);
        }
    }

    protected void RemovePerson() {
        Population.RemovePerson(this);
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
