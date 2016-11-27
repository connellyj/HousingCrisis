using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Person : MonoBehaviour {

	// script component variables
	public Direction direction;
	public float speed;
	public float framesPerSecond;
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
	private Vector3 gridXY;
	private Vector3 positionOffset = new Vector3(0,0.25f,0);


	void Start () {
        // set and start path
        gridXY[0] = (int)Math.Round(transform.position.x);
    	gridXY[1] = (int)Math.Round(transform.position.y);
    	gridXY[2] = 0;
        SetPath();
        LogPath();
        direction = path[0];
		Vector3 dVector = DirectionToVector(direction);
        StartCoroutine(FollowPath(dVector));
        // create sprite arrays and start animation
		northSprites = new Sprite[] {spriteNorthA, spriteNorthB};
		southSprites = new Sprite[] {spriteSouthA, spriteSouthB};
		westSprites = new Sprite[] {spriteWestA, spriteWestB};
		eastSprites = new Sprite[] {spriteEastA, spriteEastB};
		spritesByDirection = new Sprite[][] {northSprites, southSprites, westSprites, eastSprites};
		spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(PlayAnimation());
	}
	
	void Update () {
		if(Input.GetKeyDown(KeyCode.P)) {
			Panic();
		}
	}

	public void Panic()
	{
		speed *= 2;
		ChangeState(PersonState.PANIC);
	}

	private void ChangeState(PersonState newState)
	{
		StopCoroutine("FollowPath");
		state = newState;
		SetPath();
		// transition to new path
		Direction firstDirection = path[0];
		Vector3 targetTile = gridXY;
		if (direction == firstDirection)
		{
			targetTile += DirectionToVector(direction);
			pathIndex = 0; // replaces first pathing direction
		} else {
			pathIndex = -1; // does not replace first pathing direction
		}
		Vector3 tileAdjust = targetTile + positionOffset - transform.position;
		StartCoroutine(FollowPath(tileAdjust));
	}

	private IEnumerator FollowPath(Vector3 v)
	{
		float moveFrames = 30;
		float walkTime = 1f / speed;
		for (int i = 0; i < moveFrames; i++)
		{
			transform.position += (v / moveFrames);
			yield return new WaitForSeconds(walkTime / moveFrames);
		}
		gridXY += v;
		transform.position = gridXY + positionOffset;
		pathIndex++;
		if (pathIndex < path.Count)
		{
			direction = path[pathIndex];
			Vector3 dVector = DirectionToVector(direction);
			StartCoroutine(FollowPath(dVector));
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
    	Strategy strategy = StrategyFromState();
    	int personLoc = PersonLocFromPosition();
    	Heuristic.HeuristicType heuristic = Heuristic.HeuristicType.EXIT;
    	Node start = new Node(personLoc, heuristic);
    	path = Search.DoSearch(strategy, start);
    }

    private Strategy StrategyFromState()
    {
    	switch(state) {
    		case PersonState.WANDER:
    			return new Strategy.DFS();
    		case PersonState.PANIC:
    			return new Strategy.Greedy();
    		case PersonState.TARGET:
    			return new Strategy.DFS();
    		case PersonState.NONE:
    			return new Strategy.DFS();
    		default:
    			return new Strategy.DFS();
    	}
    }

    private int PersonLocFromPosition()
    {
    	return (int)(gridXY[1] * GridManager.MAX_COL + gridXY[0]);
    }

    private void LogPath()
    {
    	Debug.Log("PATH =");
    	Direction last = path[0];
    	int c = 0;
    	for (int i = 0; i < path.Count; i++)
    	{
    		Direction next = path[i];
    		if (next != last)
    		{
    			Debug.LogFormat("   {0} {1}", last, c);
    			last = next;
    			c = 0;
    		}
    		c++;
    	}
    	Debug.LogFormat("   {0} {1}", path[path.Count - 1], c);
    }

    private IEnumerator PlayAnimation(){
        int frameIndex = 0;
        while (true) {
        	spriteRenderer.sprite = spritesByDirection[(int)direction][frameIndex];
			frameIndex = (frameIndex + 1) % 2;
        	yield return new WaitForSeconds(1f/framesPerSecond);
        }
    }
}
