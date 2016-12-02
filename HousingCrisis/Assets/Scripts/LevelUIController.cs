using UnityEngine;
using UnityEngine.UI;

public class LevelUIController : MonoBehaviour {

    public Text moneyText;
    public Text wantedLevelText;

    private readonly int WIDTH = 400;
    private readonly int HEIGHT = 400;

    private bool paused = false;
    private bool wonLevel = false;
    private string menuText = "Paused";
    private Vector2 center;
    private Rect backgroundRect;
    private Rect nextLevelRect;
    private Rect restartRect;
    private Rect exitRect;
    private Rect pausedRect;
    private GUIStyle textStyle;

    void Start() {
        center = new Vector2(Screen.width / 2, Screen.height / 2);
        backgroundRect = new Rect(center[0] - WIDTH / 2, center[1] - HEIGHT / 2, WIDTH, HEIGHT);
        nextLevelRect = new Rect(center[0] - WIDTH / 4, center[1] - HEIGHT / 16 - HEIGHT * 3 / 8, WIDTH / 2, HEIGHT / 8);
        restartRect = new Rect(center[0] - WIDTH / 4, center[1] - HEIGHT / 16, WIDTH / 2, HEIGHT / 8);
        exitRect = new Rect(center[0] - WIDTH / 4, center[1] - HEIGHT / 16 + HEIGHT * 3 / 8, WIDTH / 2, HEIGHT / 8);
        pausedRect = new Rect(center[0] - WIDTH * 3 / 8, center[1] - HEIGHT * 3 / 8, WIDTH * 3 / 4, HEIGHT / 4);
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

    public void UpdateMoney(string money) {
        moneyText.text = money;
    }

    public void UpdateWantedLevel(string wantedLevel) {
        wantedLevelText.text = wantedLevel;
    }

    public void LoseLevel() {
        menuText = "Level Failed";
        Pause();
    }

    public void WinLevel() {
        wonLevel = true;
        Pause();
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
            if(wonLevel) {
                if(GUI.Button(nextLevelRect, "Next Level")) {
                    UnPause();
                    GameManager.LoadNextLevel();
                }
            } else GUI.Box(pausedRect, menuText, textStyle);
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
