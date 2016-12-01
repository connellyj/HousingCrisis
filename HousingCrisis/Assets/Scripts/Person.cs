using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
	public enum PersonState { WANDER, PANIC, TARGET_RANDOM, NONE, STALL, TARGET_SET, ATTACK, WANDER_SET, TARGET_RANDOM_NOTBURNING }
    public PersonState state;
	protected List<Direction> path = new List<Direction>();
	private int pathIndex = 0;
	public Vector3 gridXY;
	public static Vector3 positionOffset = new Vector3(0,0.25f,0);
    private Vector3 prevPos;
    public int attackValue;
    public int attackStallTime;
    public GameObject fireball;

    protected virtual void Start () {
        // set and start path
        gridXY[0] = (int)Math.Round(transform.position.x);
    	gridXY[1] = (int)Math.Round(transform.position.y);
    	gridXY[2] = 0;
        SetAction();
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
        if(SetAction()) {
            if(path != null && path.Count != 0) {
                FollowNewPath(path[0]);
                LogPath();
            }else {
                CompletePath();
            }
        }
	}

	private void FollowNewPath(Direction newDirection)
	{
        // attempt to fix diagonal problem
        /*if(Mathf.Abs(X() - transform.position.x) < 0.1 &&
            Mathf.Abs((Y() + positionOffset.magnitude) - transform.position.y) < 0.1) {
            SnapPositionToGrid();
            StartCoroutine(FollowPath(GridManager.DirectionToVector(path[0])));
        }else {*/
            Vector3 targetTile = gridXY;
            if(direction == newDirection) {
                targetTile += GridManager.DirectionToVector(direction);
                pathIndex = 0; // replaces first pathing direction
            } else {
                direction = Opposite(direction);
                pathIndex = -1; // does not replace first pathing direction
            }
            Vector3 tileAdjust = targetTile + positionOffset - transform.position;
            StartCoroutine(FollowPath(tileAdjust));
        //}
	}

    protected virtual IEnumerator Stall() {
        if(HouseManager.houses.ContainsKey(goalIndex)) {
            House h = HouseManager.houses[goalIndex];
            if(h.HasAvailableStallSpace()) {
                MoveToPosition(h.AddStalledPerson(this));
                yield return new WaitForSeconds(stallTime);
                h.RemoveStalledPerson(this);
                CompletePath();
            } else CompletePath();
        } else CompletePath();
        UnHighlight();
    }

    public void FaceGoal() {
        int curIdx = GridManager.CoordsToIndex((int)Mathf.Round(transform.position.x), (int)Mathf.Round(transform.position.y));
        if(curIdx + 1 == goalIndex) {
            direction = Direction.EAST;
        }else if(curIdx - 1 == goalIndex) {
            direction = Direction.WEST;
        } else if(curIdx + GridManager.MAX_COL == goalIndex) {
            direction = Direction.SOUTH;
        } else if(curIdx - GridManager.MAX_COL == goalIndex) {
            direction = Direction.NORTH;
        }
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

    private bool SetAction()
    {
    	int personLoc = PersonLocFromPosition();
        switch(state) {
            case PersonState.TARGET_SET:
                path = Pathfinder.FindPathToHouse(personLoc, goalIndex);
                return true;
            case PersonState.TARGET_RANDOM:
                if(HouseManager.houses.Count == 0) {
                    ChangeState(PersonState.WANDER);
                } else if(!HouseManager.AnyStallSpaceAnywhere()) {
                    ChangeState(PersonState.WANDER);
                } else {
                    goalIndex = HouseManager.houses.Keys.ElementAt(UnityEngine.Random.Range(0, HouseManager.houses.Count));
                    path = Pathfinder.FindPathToHouse(personLoc, goalIndex);
                }
                return true;
            case PersonState.TARGET_RANDOM_NOTBURNING:
                if(HouseManager.houses.Count == 0 || !HouseManager.AnyHousesNotBurning()) {
                    ChangeState(PersonState.WANDER);
                } else {
                    if(HouseManager.burningHouses.Count == 0) {
                        goalIndex = HouseManager.houses.Keys.ElementAt(UnityEngine.Random.Range(0, HouseManager.houses.Count));
                    }else {
                        goalIndex = HouseManager.burningHouses[0];
                        while(HouseManager.burningHouses.Contains(goalIndex)) {
                            goalIndex = HouseManager.houses.Keys.ElementAt(UnityEngine.Random.Range(0, HouseManager.houses.Count));
                        }
                    }
                    path = Pathfinder.FindPathToHouse(personLoc, goalIndex);
                }
                return true;
            case PersonState.STALL:
                StopAllCoroutines();
                StartCoroutine(Stall());
                return false;
            case PersonState.ATTACK:
                StopAllCoroutines();
                Attack();
                return false;
            case PersonState.WANDER_SET:
                path = Pathfinder.FindPathToHouse(personLoc, goalIndex);
                return true;
            default:
                path = Pathfinder.FindPath(state, personLoc);
                return true;
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
        FaceGoal();
        int frameIndex = 0;
        while (true) {
            if(spriteRenderer == null) yield return null;
        	spriteRenderer.sprite = spritesByDirection[(int)direction][frameIndex];
			frameIndex = (frameIndex + 1) % 2;
        	yield return new WaitForSeconds(1f/animationFPS);
        }
    }

    protected virtual void Attack() {
        return;
    }

    protected void ShootFireball() {
        Fireball fB = ((GameObject)Instantiate(fireball, transform.position, Quaternion.identity)).GetComponent<Fireball>();
        fB.Shoot(direction);
    }

    protected void MoveToPosition(Vector3 pos) {
        if(pos != prevPos) prevPos = transform.position;
        StartCoroutine(TranslateToPos(pos));
    }

    public void ResetPosition() {
        MoveToPosition(prevPos);
    }

    private IEnumerator TranslateToPos(Vector3 pos) {
        Vector3 dir = pos - transform.position;
        float dist = dir.magnitude;
        dir = dir.normalized;
        while(dist > 0) {
            transform.Translate(dir * 0.05f);
            dist -= 0.05f;
            yield return new WaitForSeconds(0.05f);
        }
    }

    protected void RemovePerson() {
        Population.RemovePerson(this);
        Destroy(gameObject);
    }

    public void HighlightEat() {
        spriteRenderer.color = Color.red;
    }

    public void HighlightStore() {
        spriteRenderer.color = Color.blue;
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

    public void OnStorePull(int index) {
        goalIndex = index;
        HighlightStore();
        ChangeState(PersonState.WANDER_SET);
    }

    public int X() {
        return (int)gridXY[0];
    }

    public int Y() {
        return (int)gridXY[1];
    }
}
