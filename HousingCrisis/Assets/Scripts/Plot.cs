﻿using UnityEngine;

public class Plot : MonoBehaviour {

    private readonly int HEIGHT = 100;
    private readonly int WIDTH = 100;

    private bool open = false;

    private Vector2 screenPos;
    private Rect backgroundRect;
    private Rect buildRect;
    private Rect closeRect;
    private GUIStyle textStyle;

    void Start() {
        screenPos = Camera.main.WorldToScreenPoint(transform.position);
        screenPos[0] -= WIDTH / 2;
        screenPos[1] += HEIGHT / 2;
        backgroundRect = new Rect(screenPos, new Vector2(WIDTH, HEIGHT));
        buildRect = new Rect(screenPos, new Vector2(WIDTH, HEIGHT / 2));
        closeRect = new Rect(screenPos[0], screenPos[1] + HEIGHT / 2, WIDTH, HEIGHT / 2);
        textStyle = new GUIStyle();
        textStyle.alignment = TextAnchor.MiddleCenter;
    }

    void OnMouseDown() {
        open = true;
    }

    void OnGUI() {
        if(open) {
            GUI.Box(backgroundRect, "");
            if(HouseManager.CanBuildHouse(HouseManager.HouseType.HOUSE1)) {
                if(GUI.Button(buildRect, "Build House")) {
                    HouseManager.BuildHouse(transform.position, HouseManager.HouseType.HOUSE1);
                    Destroy(gameObject);
                }
            }else GUI.Box(buildRect, "Not enough $$", textStyle);
            if(GUI.Button(closeRect, "Close")) open = false;
        }
    }
}