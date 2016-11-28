using UnityEngine;
using System.Collections.Generic;

public class Plot : MonoBehaviour {

    private static Plot currentlyOpen;

    private readonly int HEIGHT = 100;
    private readonly int WIDTH = 100;

    private bool open = false;

    private Vector2 screenPos;
    private Rect backgroundRect;
    private Rect buildRect;
    private Rect closeRect;
    private GUIStyle textStyle;
    private List<Direction> adjacentPaths; 

    void Start() {
        screenPos = Camera.main.WorldToScreenPoint(transform.position);
        screenPos[0] -= WIDTH / 2;
        screenPos[1] = Screen.height - (screenPos[1] + HEIGHT / 2);
        backgroundRect = new Rect(screenPos, new Vector2(WIDTH, HEIGHT));
        buildRect = new Rect(screenPos, new Vector2(WIDTH, HEIGHT / 2));
        closeRect = new Rect(screenPos[0], screenPos[1] + HEIGHT / 2, WIDTH, HEIGHT / 2);
        textStyle = new GUIStyle();
        textStyle.alignment = TextAnchor.MiddleCenter;

        adjacentPaths = GridManager.GetAdjacentPaths((int)Mathf.Round(transform.position.x), (int)Mathf.Round(transform.position.y), gameObject);
    }

    void OnMouseDown() {
        if(currentlyOpen != null) currentlyOpen.open = false;
        open = true;
        currentlyOpen = this;
    }

    void OnGUI() {
        if(open) {
            GUI.Box(backgroundRect, "");
            if(HouseManager.CanBuildHouse(HouseManager.HouseType.HOUSE1)) {
                if(GUI.Button(buildRect, "Build House")) {
                    HouseManager.BuildHouse(transform.position, HouseManager.HouseType.HOUSE1, adjacentPaths);
                    Destroy(gameObject);
                }
            }else GUI.Box(buildRect, "Not enough $$", textStyle);
            if(GUI.Button(closeRect, "Close")) open = false;
        }
    }
}