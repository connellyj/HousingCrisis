using UnityEngine;

public class PauseController : MonoBehaviour {

    private readonly int WIDTH = 400;
    private readonly int HEIGHT = 400;

    private bool paused = false;
    private Vector2 center;
    private Rect backgroundRect;
    private Rect restartRect;
    private Rect exitRect;
    private Rect pausedRect;
    private GUIStyle textStyle;

    void Start() {
        center = new Vector2(Screen.width / 2, Screen.height / 2);
        backgroundRect = new Rect(center[0] - WIDTH / 2, center[1] - HEIGHT / 2, WIDTH, HEIGHT);
        restartRect = new Rect(center[0] - WIDTH / 4, center[1] - HEIGHT / 8, WIDTH / 2, HEIGHT / 8);
        exitRect = new Rect(center[0] - WIDTH / 4, center[1] + HEIGHT / 8, WIDTH / 2, HEIGHT / 8);
        pausedRect = new Rect(center[0] - WIDTH * 3 / 8, center[1] - HEIGHT / 2, WIDTH * 3 / 4, HEIGHT / 4);
        textStyle = new GUIStyle();
        textStyle.alignment = TextAnchor.MiddleCenter;
        textStyle.fontSize = 50;
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            if(paused) UnPause();
            else Pause();
        }
    }

    void Pause() {
        Time.timeScale = 0;
        paused = true;
    }

     void UnPause() {
        Time.timeScale = 1;
        paused = false;
    }

    void OnGUI() {
        if(paused) {
            GUI.Box(backgroundRect, "");
            GUI.Box(pausedRect, "Paused", textStyle);
            if(GUI.Button(restartRect, "Restart")) {
                UnPause();
                GameManager.RestartLevel();
            }
            if(GUI.Button(exitRect, "Exit")) {
                UnPause();
                GameManager.Exit();
            }
        }
    }
}
