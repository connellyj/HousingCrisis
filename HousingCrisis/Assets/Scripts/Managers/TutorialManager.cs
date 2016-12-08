using UnityEngine;
using System.Collections;

public class TutorialManager : MonoBehaviour {

    public GameObject speechBubblePrefab;

    private SpeechBubble speechBubble;

    private bool hasShownFirstMessage = false;
    private bool hasShownSecondMessage = false;
    private bool hasShownThirdMessage = false;

    void Start() {
        MessagePlayer("This is it. We have tried negotiating with the humans for better treatment, " +
            "but they continue to neglect us, their beloved homes. We must retaliate! " +
            "Click on a plot of land to build a house, then click the start button and click on the people as they walk by.");
    }

    private void MessagePlayer(string message) {
        Time.timeScale = 0;
        speechBubble = (Instantiate(speechBubblePrefab)).GetComponent<SpeechBubble>();
        speechBubble.okButton.onClick.AddListener(() => {
            Time.timeScale = 1; 
            Destroy(speechBubble.gameObject);
        });
        speechBubble.words = new string[1] {message};
        speechBubble.NextText();
    }

    void Update() {
        if (!hasShownThirdMessage) {
            if(GameManager.GetPeopleEaten() >= 1 && !hasShownFirstMessage) {
                MessagePlayer("Perfect, we can make the world a better place by removing all these humans! " +
                    "We're able to get money from the people you get rid of, that way you can build more houses. " +
                    "You should deal with all the humans in this area, then we'll move on.");
                hasShownFirstMessage = true;
            } else if(GameManager.GetWantedLevel() >= 1 && hasShownFirstMessage && !hasShownSecondMessage) {
                MessagePlayer("Careful, when pedestrians run away they alert the local authorities." +
                    "Since they have made it clear they won't negotiate peacefully with us, they will " +
                    "likely try to damage the houses. Tap on them to put out any fires.");
                hasShownSecondMessage = true;
            } else if (GameManager.GetPeopleEaten() >= 10 && hasShownSecondMessage) {
                MessagePlayer("There's no chance for peace now, the only way we can keep the homes safe is to " +
                    "get rid of the careless humans!");
                hasShownThirdMessage = true;
            }
        }
    }
}
