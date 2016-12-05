using UnityEngine;

public class TutorialManager : MonoBehaviour {

    public GameObject speechBubblePrefab;

    private SpeechBubble speechBubble;
    //private int moneyToPopAt = 

    void Start() {
        speechBubble = (Instantiate(speechBubblePrefab)).GetComponent<SpeechBubble>();
        speechBubble.okButton.onClick.AddListener(() => Destroy(speechBubble.gameObject));
        speechBubble.words = new string[1] {"Hello! My name is Hubert Houserton! *insert euphamism* Click on a plot " +
            "of land to build a house, then click the people as they walk by!"};
        speechBubble.NextText();
    }
}
