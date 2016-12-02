using UnityEngine;
using UnityEngine.UI;

public class StartScreenController : MonoBehaviour {

    public Button startButton;
    public Button quitButton;

    void Start() {
        startButton.onClick.AddListener(() => GameManager.LoadNextLevel());
        quitButton.onClick.AddListener(() => GameManager.Quit());
    }
}
