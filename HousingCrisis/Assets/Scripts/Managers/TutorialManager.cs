using UnityEngine;

public class TutorialManager : MonoBehaviour {

    public GameObject speechBubblePrefab;

    private SpeechBubble speechBubble;
    //private int moneyToPopAt = 

    private bool hasShownFirstMessage = false;
    private bool hasShownSecondMessage = false;
    private bool hasShownThirdMessage = false;

    void Start() {
        MessagePlayer("Hello there! My name is Hubert Houserton! *insert euphamism* Click on a plot " +
            "of land to build a house, then click on those silly people as they walk by!");
    }

    private void MessagePlayer(string message) 
    {
        Time.timeScale = 0;
        speechBubble = (Instantiate(speechBubblePrefab)).GetComponent<SpeechBubble>();
        speechBubble.okButton.onClick.AddListener(() => {
            Time.timeScale = 1; 
            Destroy(speechBubble.gameObject);
        });
        speechBubble.words = new string[1] {message};
        speechBubble.NextText();
    }

    void Update()
    {
        if (!hasShownThirdMessage) {
            if (GameManager.GetPeopleEaten() >= 1 && !hasShownFirstMessage)
            {
                MessagePlayer("Woah there! It looks like you've devoured your first human! From the clothes" +
                    " he had on we were able to make a quick $10, that means we almost have the $50 we need to build" +
                    " another house. Keep on crunching those tasty pedestrians!");
                hasShownFirstMessage = true;
            } else if (GameManager.GetWantedLevel() >= 1 && hasShownFirstMessage && !hasShownSecondMessage) {
                MessagePlayer("Uh oh, when pedestrians escape they raise our wanted level shown by the stars in the" +
                    " upper right hand corner. It looks like we've attracted the attention of some local criminals" +
                    " looking for an easy score. If they set your buildings on fire, click in the flames to snuff them" +
                    " out!");
                hasShownSecondMessage = true;
            } else if (GameManager.GetPeopleEaten() >= 10 && hasShownSecondMessage) {
                MessagePlayer("I'm gonna let you in on a little secret, our top laboratories have been working on something new," +
                    " something big, but we're running out of specimens. We need you to snatch up 20 more.");
                hasShownThirdMessage = true;
            }
        }
    }
}
