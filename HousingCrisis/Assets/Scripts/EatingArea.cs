using UnityEngine;

public class EatingArea : MonoBehaviour {

    public Direction direction;
    
    private Population population;
    private House house;

    void Awake() {
        house = transform.parent.gameObject.GetComponent<House>();
        population = GameManager.GetPopulation();
    }

    void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Person") {
            Person p = other.gameObject.GetComponent<Person>();
            p.Highlight();
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if(other.tag == "Person") {
            Person p = other.gameObject.GetComponent<Person>();
            p.UnHighlight();
        }
    }

    void OnMouseDown() {
        house.Eat(direction);
    }
}
