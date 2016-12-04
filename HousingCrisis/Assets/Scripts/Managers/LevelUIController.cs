using UnityEngine;
using UnityEngine.UI;

public class LevelUIController : MonoBehaviour {

    // Onscreen info and buttons
    public Text moneyText;
    public Text wantedLevelText;
    public Text startWinButtonText;
    public Button startWinButton;
    public Button helpButton;
    // Game state info
    private bool paused = false;
    private bool wonLevel = false;
    private bool canWinLevel = false;
    private bool levelStarted = false;
    private bool helpMenuOpen = false;
    private int numHelpDisplayed = 0;
    // Pause/win/lose menu
    private string menuText = "Paused";
    private Rect pausedBackgroundRect;
    private Rect nextLevelRect;
    private Rect restartRect;
    private Rect exitRect;
    private Rect pausedRect;
    private GUIStyle textStyle;
    // Help menu
    public Texture[] houseImages;
    public string[] houseInfo;
    private Rect helpBackgroundRect;
    private Rect helpViewRect;
    private Rect closeHelpRect;
    private Rect[] houseImageRects;
    private Rect[] houseInfoRects;
    private Rect[] houseNameRects;
    private Vector2 scrollPosition = Vector2.zero;

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
        // Shows the pause/win/lose menu
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
        // Shows the help menu
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

    // Adds listeners to the start/win button and help button
    private void InitButtons() {
        startWinButton.onClick.AddListener(() => {
            if(canWinLevel) WinLevel();
            if(!levelStarted) {
                levelStarted = true;
                startWinButtonText.text = "";
                GameManager.BeginSpawning();
            }
        });
        helpButton.onClick.AddListener(() => {
            if(!paused) helpMenuOpen = true;
        });
    }

    // Initializes the rects for the pause/win/lose menu
    private void InitPauseRects() {
        int pauseWidth = 300;
        int pauseHeight = 200;
        Vector2 center = new Vector2(Screen.width / 2, Screen.height / 2);
        pausedBackgroundRect = new Rect(center[0] - pauseWidth / 2, center[1] - pauseHeight / 2, pauseWidth, pauseHeight);
        nextLevelRect = new Rect(center[0] - pauseWidth / 4, center[1] - pauseHeight / 16 - pauseHeight / 4, pauseWidth / 2, pauseHeight / 8);
        restartRect = new Rect(center[0] - pauseWidth / 4, center[1] - pauseHeight / 16, pauseWidth / 2, pauseHeight / 8);
        exitRect = new Rect(center[0] - pauseWidth / 4, center[1] - pauseHeight / 16 + pauseHeight / 4, pauseWidth / 2, pauseHeight / 8);
        pausedRect = new Rect(center[0] - pauseWidth * 3 / 8, center[1] - pauseHeight * 3 / 8, pauseWidth * 3 / 4, pauseHeight / 4);
        textStyle = new GUIStyle();
        textStyle.alignment = TextAnchor.MiddleCenter;
        textStyle.fontSize = 50;
    }

    // Initializes the rects for the help menu
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

    // Changes the onscreen money value to the provided string
    public void UpdateMoney(int money) {
        moneyText.text = "$" + money;
    }

    // Updates the onscreen wanted value
    public void UpdateWantedLevelAndProgress(string wantedLevel, string progress) {
        wantedLevelText.text = string.Format("{0} . {1}", wantedLevel, progress);
    }

    // Ends the level
    public void LoseLevel() {
        menuText = "Level Failed";
        Pause();
    }

    // Activates the win button
    public void SetCanWinLevel() {
        canWinLevel = true;
        startWinButtonText.text = "WIN!";
    }

    // Wins the level
    public void WinLevel() {
        wonLevel = true;
        Pause();
    }

    // Pauses the game
    private void Pause() {
        Time.timeScale = 0;
        paused = true;
    }

    // Unpauses the game
    private void UnPause() {
        Time.timeScale = 1;
        paused = false;
    }
}