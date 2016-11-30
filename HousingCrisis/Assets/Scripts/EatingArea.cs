using UnityEngine;

public class EatingArea : MonoBehaviour {

    public Direction direction;
    
    private House house;
    private int numPeopleInArea = 0;

    void Awake() {
        house = transform.parent.gameObject.GetComponent<House>();
    }

    void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Person") {
            numPeopleInArea++;
            Person p = other.gameObject.GetComponent<Person>();
            p.Highlight();
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if(other.tag == "Person") {
            numPeopleInArea--;
            Person p = other.gameObject.GetComponent<Person>();
            p.UnHighlight();
        }
    }

    void OnMouseDown() {
        Debug.Log("click");
        if(numPeopleInArea > 0) {
            house.Eat(direction);
        }
    }
}
