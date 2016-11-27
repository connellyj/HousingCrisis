using UnityEngine;

public class PlotController : MonoBehaviour {

    public GameObject house;

    private bool open;
    private Vector2 mousePos;

    void Awake() {
        open = false;
    }

    void OnMouseDown() {
        open = true;
        mousePos = Input.mousePosition;
    }

    void OnGUI() {
        if(open) {
            GUI.Box(new Rect(mousePos[0] - 50, mousePos[1] + 50, 100, 100), "");
            if(GUI.Button(new Rect(mousePos[0] - 50, mousePos[1] + 50, 100, 50), "Build House")) {
                Instantiate(house, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
            if(GUI.Button(new Rect(mousePos[0]- 50, mousePos[1] + 100, 100, 50), "Close")) {
                open = false;
            }
        }
    }
}
