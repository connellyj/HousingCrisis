using UnityEngine;

public class EatingArea : MonoBehaviour {

    public Direction direction;
    
    private House house;
    private int numPeopleInArea = 0;

    void Awake() {
        house = transform.parent.gameObject.GetComponent<House>();
    }

    // Highlights any people in eating range
    void OnTriggerEnter2D(Collider2D other) {
        if(other.tag.Contains("Person")) {
            numPeopleInArea++;other.gameObject.GetComponent<Person>().HighlightEat();
        }
    }

    // Unhighlights people when they leave the eating area
    void OnTriggerExit2D(Collider2D other) {
        if(other.tag.Contains("Person")) {
            numPeopleInArea--;
            Person p = other.gameObject.GetComponent<Person>();
            p.UnHighlight();
        }
    }

    // When clicked, makes the house eat if there are people present
    void OnMouseDown() {
        if(numPeopleInArea > 0) house.Eat(direction);
    }
}