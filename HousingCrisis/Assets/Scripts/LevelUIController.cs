using UnityEngine;
using UnityEngine.UI;

public class LevelUIController : MonoBehaviour {

    public Text moneyText;
    public Text wantedLevelText;

    public void UpdateMoney(string money) {
        moneyText.text = money;
    }

    public void UpdateWantedLevel(string wantedLevel) {
        wantedLevelText.text = wantedLevel;
    }
}
