using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
    
    public int houseCost;
    public int startingMoney;
    public GameObject pauseController;
    public GameObject houseManager;

    private static GameManager instance;

    private int currentScene;
    private int wantedLevel;
    private int moneyAmount;
    private LevelUIController levelUI;
    private Population population;

    void Awake() {
        if(instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        if(instance != this) Destroy(gameObject);
        population = new Population();
        currentScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.sceneLoaded += OnSceneLoaded;
        UpdateLevel();
    }

    // Automatically called when the scene is loaded
    private void OnSceneLoaded(Scene sceneIndex, LoadSceneMode loadSceneMode) {
        UpdateLevel();
    }

    // Updates values when a new level starts
    private void UpdateLevel() {
        UpdateUI();
        population.ClearPeople();
        moneyAmount = 0;
        wantedLevel = 0;
        UpdateMoney(startingMoney);
    }

    // Updates the UI when a new level starts
    private void UpdateUI() {
        GameObject ui = GameObject.FindGameObjectWithTag("LevelUI");
        if(ui != null) {
            levelUI = ui.GetComponent<LevelUIController>();
            Instantiate(instance.pauseController);
            Instantiate(houseManager);
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

    // Returns the population
    public static Population GetPopulation() {
        return instance.population;
    }
}