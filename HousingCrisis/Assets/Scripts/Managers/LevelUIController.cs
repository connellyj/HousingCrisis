using UnityEngine;
using UnityEngine.UI;

public class LevelUIController : MonoBehaviour {

    public Text moneyText;
    public Text wantedLevelText;
    public Text startWinButtonText;
    public Button startWinButton;
    public Button helpButton;
    public GameObject[] peopleSpawners;
    public Texture[] houseImages;
    public string[] houseInfo;

    private readonly int PAUSE_WIDTH = 200;
    private readonly int PAUSE_HEIGHT = 200;

    private bool paused = false;
    private bool wonLevel = false;
    private bool canWinLevel = false;
    private bool levelStarted = false;
    private bool helpMenuOpen = false;
    private int numHelpDisplayed = 0;
    private string menuText = "Paused";
    private Vector2 center;
    private Vector2 scrollPosition = Vector2.zero;
    private Rect pausedBackgroundRect;
    private Rect nextLevelRect;
    private Rect restartRect;
    private Rect exitRect;
    private Rect pausedRect;
    private Rect helpBackgroundRect;
    private Rect helpViewRect;
    private Rect closeHelpRect;
    private Rect[] houseImageRects;
    private Rect[] houseInfoRects;
    private Rect[] houseNameRects;
    private GUIStyle textStyle;

    void Start() {
        UnPause();
        InitButtons();
        InitPauseRects();
        InitHelpRects();
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            if(paused) UnPause();
            else Pause();
        }
    }

    void OnGUI() {
        if(paused) {
            GUI.Box(pausedBackgroundRect, "");
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

        if(helpMenuOpen) {
            if(GUI.Button(closeHelpRect, "Close")) helpMenuOpen = false;
            GUI.skin.box.wordWrap = true;
            scrollPosition = GUI.BeginScrollView(helpBackgroundRect, scrollPosition, helpViewRect, false, false);
            for(int i = 0; i < numHelpDisplayed; i++) {
                GUI.Box(houseImageRects[i], houseImages[i]);
                GUI.Box(houseInfoRects[i], houseInfo[i]);
                GUI.Box(houseNameRects[i], ((HouseManager.HouseType)i).ToString());
            }
            GUI.EndScrollView();
        }
    }

    private void InitButtons() {
        startWinButton.onClick.AddListener(() => {
            if(canWinLevel) WinLevel();
            if(!levelStarted) {
                levelStarted = true;
                startWinButtonText.text = "";
                StartLevel();
            }
        });
        helpButton.onClick.AddListener(() => {
            if(!paused) helpMenuOpen = true;
        });
    }

    private void InitPauseRects() {
        center = new Vector2(Screen.width / 2, Screen.height / 2);
        pausedBackgroundRect = new Rect(center[0] - PAUSE_WIDTH / 2, center[1] - PAUSE_HEIGHT / 2, PAUSE_WIDTH, PAUSE_HEIGHT);
        nextLevelRect = new Rect(center[0] - PAUSE_WIDTH / 4, center[1] - PAUSE_HEIGHT / 16 - PAUSE_HEIGHT * 3 / 8, PAUSE_WIDTH / 2, PAUSE_HEIGHT / 8);
        restartRect = new Rect(center[0] - PAUSE_WIDTH / 4, center[1] - PAUSE_HEIGHT / 16, PAUSE_WIDTH / 2, PAUSE_HEIGHT / 8);
        exitRect = new Rect(center[0] - PAUSE_WIDTH / 4, center[1] - PAUSE_HEIGHT / 16 + PAUSE_HEIGHT * 3 / 8, PAUSE_WIDTH / 2, PAUSE_HEIGHT / 8);
        pausedRect = new Rect(center[0] - PAUSE_WIDTH * 3 / 8, center[1] - PAUSE_HEIGHT * 3 / 8, PAUSE_WIDTH * 3 / 4, PAUSE_HEIGHT / 4);
        textStyle = new GUIStyle();
        textStyle.alignment = TextAnchor.MiddleCenter;
        textStyle.fontSize = 50;
    }

    private void InitHelpRects() {
        for(int i = 0; i < 6; i++) {
            if(ContentManager.IsBuildingUnlocked((HouseManager.HouseType) i)) numHelpDisplayed++;
        }
        int viewHeight = Screen.height / 4 * numHelpDisplayed;
        int infoWidth = Screen.height / 4;
        int viewWidth = infoWidth * 2;
        helpBackgroundRect = new Rect(0, 0, viewWidth + 18, Screen.height - 40);
        helpViewRect = new Rect(0, 0, viewWidth, viewHeight);
        closeHelpRect = new Rect(0, Screen.height - 40, viewWidth, 40);
        houseImageRects = new Rect[numHelpDisplayed];
        houseInfoRects = new Rect[numHelpDisplayed];
        houseNameRects = new Rect[numHelpDisplayed];
        for(int i = 0; i < numHelpDisplayed; i++) {
            houseImageRects[i] = new Rect(0, viewHeight / numHelpDisplayed * i, infoWidth, infoWidth);
            houseInfoRects[i] = new Rect(infoWidth, viewHeight / numHelpDisplayed * i + 20, infoWidth, infoWidth - 20);
            houseNameRects[i] = new Rect(infoWidth, viewHeight / numHelpDisplayed * i, infoWidth, 20);
        }
    }

    public void UpdateMoney(string money) {
        moneyText.text = money;
    }

    public void UpdateWantedLevel(string wantedLevel) {
        wantedLevelText.text = wantedLevel;
    }

    private void StartLevel() {
        foreach(GameObject spawn in peopleSpawners) spawn.SetActive(true);
    }

    public void LoseLevel() {
        menuText = "Level Failed";
        Pause();
    }

    public void SetCanWinLevel() {
        canWinLevel = true;
        startWinButtonText.text = "WIN!";
    }

    public void WinLevel() {
        wonLevel = true;
        Pause();
    }

    private void Pause() {
        Time.timeScale = 0;
        paused = true;
    }

    private void UnPause() {
        Time.timeScale = 1;
        paused = false;
    }
}
