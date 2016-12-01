using UnityEngine;
using System.Collections;

public class BuildMenu : MonoBehaviour {

	private static readonly int HEIGHT = 100;
    private static readonly int WIDTH = 100;

    private static bool open = false;

    private static Builder builder;
    private static Vector2 worldPos;
    private static Vector2 screenPos;
    private static Rect backgroundRect;
    private static Rect firstBuildRect;
    private static Rect secondBuildRect;
    private static Rect closeRect;
    private static GUIStyle textStyle;
    private static HouseManager.HouseType[] buildOptions = new HouseManager.HouseType[2];

	void Start () {
        backgroundRect = new Rect(Vector2.zero, new Vector2(WIDTH, HEIGHT));
        firstBuildRect = new Rect(Vector2.zero, new Vector2(WIDTH, HEIGHT / 2));
        secondBuildRect = new Rect(Vector2.zero, new Vector2(WIDTH, HEIGHT / 2));
        //closeRect = new Rect(Vector2.zero, new Vector2(WIDTH / 6, HEIGHT / 6));
        textStyle = new GUIStyle();
        textStyle.alignment = TextAnchor.MiddleCenter;
	}

	public static void Open(Builder caller) {
    	open = true;
    	builder = caller;
    	MoveMenu(caller.transform.position);
    	SetBuildOptionsForCaller(caller.type);
    }

    public static void Close() {
    	open = false;
    }
	
    private static void MoveMenu(Vector3 position)
    {
    	worldPos = position;
    	screenPos = worldToScreenPosition(position);
    	backgroundRect.position = screenPos;
        firstBuildRect.position = screenPos;
        secondBuildRect.position = new Vector2(screenPos[0], screenPos[1] + HEIGHT / 2);
        //closeRect.position = new Vector2(screenPos[0] + WIDTH / , screenPos[1] + HEIGHT / 2);
    }

	private static Vector2 worldToScreenPosition(Vector3 position)
	{
		Vector2 v = Camera.main.WorldToScreenPoint(position);
        v[0] -= WIDTH / 2;
        v[1] = Screen.height - v[1] - HEIGHT / 2;
        return v;
	}

	void OnGUI() {
        if (open) {
        	// close if clicking elsewhere
        	if (Input.GetMouseButtonDown(0)) {
            	Vector2 mouse = Input.mousePosition;
            	if(!backgroundRect.Contains(new Vector2(mouse[0], Screen.height - mouse[1]))) {
                	Close();
                }
            }
            // respond to button clicks
            GUI.Box(backgroundRect, "");
            HouseManager.HouseType firstType = buildOptions[0];
            HouseManager.HouseType secondType = buildOptions[1];
            if (firstType == HouseManager.HouseType.LOCKED)
            {
            	GUI.Box(firstBuildRect, "LOCKED", textStyle);
            } else {
            	if (HouseManager.CanBuild(firstType)) {
                    if (GUI.Button(firstBuildRect, firstType.ToString())) {
                    	Close();
                    	Build(firstType);
                    }
                } else {
                	GUI.Box(firstBuildRect, "NEED $$$", textStyle);
                }
            }
            if (secondType == HouseManager.HouseType.LOCKED)
            {
            	GUI.Box(secondBuildRect, "LOCKED", textStyle);
            } else {
            	if (HouseManager.CanBuild(secondType)) {
                    if (GUI.Button(secondBuildRect, secondType.ToString())) {
                    	Close();
                    	Build(secondType);
                    }
                } else {
                	GUI.Box(firstBuildRect, "NEED $$$", textStyle);
                }
            // } else if (GUI.Button(closeRect, "Close")) {
            // 	Close();
        	}
        }
    }

    private void Build(HouseManager.HouseType type)
    {
    	builder.OnBuild();
    	HouseManager.Build(worldPos, HouseManager.HouseType.HOUSE);
    }


    private static void SetBuildOptionsForCaller(HouseManager.HouseType type)
    {
    	if (type == HouseManager.HouseType.PLOT)
    	{
    		buildOptions[0] = CheckLocked(HouseManager.HouseType.HOUSE);
    		buildOptions[1] = CheckLocked(HouseManager.HouseType.STORE);
    	} else if (type == HouseManager.HouseType.HOUSE) {
			buildOptions[0] = CheckLocked(HouseManager.HouseType.APARTMENT);
    		buildOptions[1] = CheckLocked(HouseManager.HouseType.MANSION);
    	} else if (type == HouseManager.HouseType.STORE) {
    		buildOptions[0] = CheckLocked(HouseManager.HouseType.DONUT);
    		buildOptions[1] = CheckLocked(HouseManager.HouseType.BANK);
    	} else {
    		Debug.Log("Error: Invalid callType passed to BuildMenu");
    	}
    }

    private static HouseManager.HouseType CheckLocked(HouseManager.HouseType type)
    {
    	if (ContentManager.isBuildingUnlocked(type))
    	{
    		return type;
    	} else {
    		return HouseManager.HouseType.LOCKED;
    	}
    }

}
