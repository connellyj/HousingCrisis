﻿using UnityEngine;
using UnityEngine.UI;

public class SpeechBubble : MonoBehaviour {

    public Text speechText;
    public Button okButton;
    public string[] words;
    public bool tutorial;

    private int wordIndex = 0;

    void Start() {
        if(!tutorial) {
            speechText.text = words[wordIndex];
            wordIndex++;
            okButton.onClick.AddListener(() => {
                if(!NextText()) Destroy(gameObject);
            });
        }
    }

    // Switches to the next section of text or destroys the bubble when there's no more text
    public bool NextText() {
        if(wordIndex < words.Length) {
            speechText.text = words[wordIndex];
            wordIndex++;
            return true;
        }
        return false;
    }
}