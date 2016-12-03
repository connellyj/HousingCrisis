using UnityEngine;
using UnityEngine.UI;

public class SpeechBubble : MonoBehaviour {

    public Text speechText;
    public Button okButton;
    public string[] words;

    private int wordIndex = 0;

    void Start() {
        speechText.text = words[wordIndex];
        wordIndex++;
        okButton.onClick.AddListener(() => {
            if(!NextText()) Destroy(gameObject);
        });
    }

    private bool NextText() {
        if(wordIndex < words.Length) {
            speechText.text = words[wordIndex];
            wordIndex++;
            return true;
        }
        return false;
    }
}
