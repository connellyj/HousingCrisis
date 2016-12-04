using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Person : MonoBehaviour {

    // static info
    protected static readonly float stallTime = 2;
    protected static readonly int value = 10;
    protected float speed = 2;
    protected static readonly float alertSpeed = 4;
    protected static readonly float animationFPS = 4;
    protected static readonly float motionFPS = 30;
    protected static readonly Color eatColor = Color.red;
    protected static readonly Color storeColor = Color.blue;
    protected static readonly Color normalColor = Color.white;

    // script component variables
    [HideInInspector] public Direction direction;
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
    [HideInInspector] public PersonState state;
	protected List<Direction> path = new List<Direction>();
	private int pathIndex = 0;
	protected Vector3 gridXY;
	public static Vector3 positionOffset = new Vector3(0,0.25f,0);
    public int attackValue;
    public int attackStallTime;
    public GameObject fireball;

    public enum PersonState { WANDER, PANIC, TARGET_RANDOM, NONE, STALL, TARGET_SET, ATTACK, WANDER_SET, TARGET_RANDOM_NOTBURNING }

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
        spriteRenderer.color = normalColor;

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
            } else {
                CompletePath();
            }
        }
	}

	private void FollowNewPath(Direction newDirection)
	{
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
	}

    protected virtual IEnumerator Stall() {
        if(HouseManager.houses.ContainsKey(goalIndex)) {
            House h = HouseManager.houses[goalIndex];
            if(h.HasAvailableStallSpace()) {
                MoveToPosition(h.AddStalledPerson(this));
                yield return new WaitForSeconds(stallTime);
                h.RemoveStalledPerson(this);
                if(spriteRenderer.color == storeColor) UnHighlight();
                CompletePath();
            } else CompletePath();
        } else CompletePath();
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
                if(HouseManager.houses.Count == 0 || !HouseManager.AnyStallSpaceAnywhere()) {
                    state = PersonState.WANDER;
                    goto default;
                } else {
                    goalIndex = HouseManager.houses.Keys.ElementAt(UnityEngine.Random.Range(0, HouseManager.houses.Count));
                    path = Pathfinder.FindPathToHouse(personLoc, goalIndex);
                }
                return true;
            case PersonState.TARGET_RANDOM_NOTBURNING:
                if(HouseManager.houses.Count == 0 || !HouseManager.AnyHousesNotBurning()) {
                    state = PersonState.WANDER;
                    goto default;
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
        StartCoroutine(TranslateToPos(pos));
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
        spriteRenderer.color = eatColor;
    }

    public void HighlightStore() {
        spriteRenderer.color = storeColor;
    }

    public void UnHighlight() {
        spriteRenderer.color = normalColor;
    }

    public void OnEaten() {
        if(tag == "PersonBanker") GameManager.UpdateMoney(value * 2);
        else GameManager.UpdateMoney(value);
        RemovePerson();
    }

    public virtual void OnSeeHouse(int houseIndex) {
        Panic();
    }

    public void OnStorePull(int index) {
        goalIndex = index;
        if(spriteRenderer.color == normalColor) HighlightStore();
        ChangeState(PersonState.WANDER_SET);
    }

    public int X() {
        return (int)gridXY[0];
    }

    public int Y() {
        return (int)gridXY[1];
    }
}
