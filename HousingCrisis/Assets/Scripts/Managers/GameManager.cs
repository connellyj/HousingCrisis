using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour {

    private static GameManager instance;

    public int peopleEaten;
    private int currentScene;
    private int moneyAmount;
    private bool levelStarted = false;
    private LevelUIController levelUI;
    private DifficultyManager difficultyManager;

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
        if(InLevel()) UpdateLevel();
    }

    void Update() {
        if(InLevel()) {
            if(levelStarted) {
                if(HouseManager.houses.Count == 0) LoseLevel();
                if(peopleEaten >= ContentManager.PeopleEatenToWin()) WinLevel();
            } else if(HouseManager.houses.Count > 0) levelStarted = true;
        }
    }

    public static void StartLevel() {
        instance.StartWantedLevelCounter();
        instance.BeginSpawning();
    }

    private void StartWantedLevelCounter() {
        StartCoroutine(IncreaseWantedLevel());
    }

    private IEnumerator IncreaseWantedLevel() {
        while(true) {
            yield return new WaitForSeconds(1);
            UpdateWantedLevel(1);
        }
    }

    // Automatically called when the scene is loaded
    private void OnSceneLoaded(Scene sceneIndex, LoadSceneMode loadSceneMode) {
        UpdateUI();
        if(InLevel()) UpdateLevel();
    }

    // Returns whether or not the GameManager is currently in a level
    private static bool InLevel() {
        return instance.levelUI != null;
    }

    // Updates values when a new level starts
    private void UpdateLevel() {
        levelStarted = false;
        moneyAmount = 0;
        peopleEaten = 0;
        UpdateMoney(ContentManager.StartingMoney());
        UpdateDiffManager();
    }

    // Loses the level
    private void LoseLevel() {
        levelUI.LoseLevel();
    }

    // Wins the level
    private void WinLevel() {
        levelUI.WinLevel();
    }

    // Updates the UI when a new level starts
    private void UpdateUI() {
        GameObject ui = GameObject.FindGameObjectWithTag("LevelUI");
        if(ui != null) {
            levelUI = ui.GetComponent<LevelUIController>();
        } else levelUI = null;
    }

    // Updates difficulty manager when a new level starts
    private void UpdateDiffManager() {
        GameObject dM = GameObject.FindGameObjectWithTag("DifficultyManager"); // putting DiffManager on LevelUI for the moment
        if(dM != null) {
            difficultyManager = dM.GetComponent<DifficultyManager>();
        } else difficultyManager = null;
    }

    // tells DifficultyManager to tell spawnpoints to start spawning
    private void BeginSpawning() {
        difficultyManager.ActivateSpawners();
    }

    // Loads the next scene
    public static void LoadNextLevel() {
        instance.currentScene++;
        SceneManager.LoadScene(instance.currentScene % SceneManager.sceneCount);
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

    // Updates the escapeCount by the given amount which updates the wanted level accordingly
    public static void UpdateWantedLevel(int change) {
        instance.difficultyManager.ChangeProgressCount(change);
        instance.levelUI.UpdateWantedLevelAndProgress(instance.difficultyManager.wantedLevel);
    }

    public static int GetWantedLevel() {
        return instance.difficultyManager.wantedLevel;
    }

    // Updates the money amount based on the given amount
    public static void UpdateMoney(int change) {
        if(instance.moneyAmount + change < 0) instance.moneyAmount = 0;
        else instance.moneyAmount += change;
        instance.levelUI.UpdateMoney(instance.moneyAmount);
    }

    public static void UpdatePeopleEaten(int change)
    {
        instance.peopleEaten += change;
        instance.levelUI.UpdatePeopleEaten(instance.peopleEaten);
    }

    public static int GetPeopleEaten() {
        return instance.peopleEaten;
    }

    public static void UpdateBankerChance(int change)
    {
        instance.difficultyManager.UpdateBankerChance(change);
    }

    // Returns the amount of money
    public static int GetMoney() {
        return instance.moneyAmount;
    }
}