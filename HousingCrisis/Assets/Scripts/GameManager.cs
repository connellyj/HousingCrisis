using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    private static GameManager instance;

    private int currentScene;
    private int wantedLevel;
    private int moneyAmount;
    private bool levelStarted = false;
    private LevelUIController levelUI;

    void Awake() {
        if(instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        if(instance != this) Destroy(gameObject);
    }

    void Start() {
        currentScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.sceneLoaded += OnSceneLoaded;
        UpdateUI();
        if(levelUI != null) UpdateLevel();
    }

    void Update() {
        if(levelUI != null) {
            if(levelStarted) {
                if(HouseManager.houses.Count == 0) LoseLevel();
                if(moneyAmount >= ContentManager.moneyToWinS) WinLevel();
            } else if(HouseManager.houses.Count > 0) levelStarted = true;
        }
    }

    // Automatically called when the scene is loaded
    private void OnSceneLoaded(Scene sceneIndex, LoadSceneMode loadSceneMode) {
        UpdateUI();
        if(levelUI != null) UpdateLevel();
    }

    // Updates values when a new level starts
    private void UpdateLevel() {
        levelStarted = false;
        Population.ClearPeople();
        moneyAmount = 0;
        wantedLevel = 0;
        UpdateMoney(ContentManager.startingMoneyS);
    }

    private void LoseLevel() {
        levelUI.LoseLevel();
    }

    private void WinLevel() {
        levelUI.SetCanWinLevel();
    }

    // Updates the UI when a new level starts
    private void UpdateUI() {
        GameObject ui = GameObject.FindGameObjectWithTag("LevelUI");
        if(ui != null) {
            levelUI = ui.GetComponent<LevelUIController>();
        } else levelUI = null;
    }

    // Loads the next scene
    public static void LoadNextLevel() {
        instance.currentScene++;
        SceneManager.LoadScene(instance.currentScene);
    }

    // Reloads the current scene
    public static void RestartLevel() {
        SceneManager.LoadScene(instance.currentScene);
    }

    // Returns to the first scene
    public static void Exit() {
        instance.currentScene = 0;
        SceneManager.LoadScene(instance.currentScene);
    }

    // Quits the game entirely
    public static void Quit() {
        Application.Quit();
    }

    // Updates the wanted level by the given amount
    public static void UpdateWantedLevel(int change) {
        if(instance.wantedLevel + change < 0) instance.wantedLevel = 0;
        else instance.wantedLevel += change;
        instance.levelUI.UpdateWantedLevel(instance.wantedLevel.ToString());
    }

    // Updates the money amount based on the given amount
    public static void UpdateMoney(int change) {
        if(instance.moneyAmount + change < 0) instance.moneyAmount = 0;
        else instance.moneyAmount += change;
        instance.levelUI.UpdateMoney("$" + instance.moneyAmount);
    }

    // Returns the amount of money
    public static int GetMoney() {
        return instance.moneyAmount;
    }
}