using UnityEngine;
using System.Collections.Generic;

public class EatingArea : MonoBehaviour {

    public Direction direction;

    private List<Person> peopleInArea;
    private House house;

    void Awake() {
        peopleInArea = new List<Person>();
        house = transform.parent.gameObject.GetComponent<House>();
    }

    void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Person") {
            Person p = other.gameObject.GetComponent<Person>();
            peopleInArea.Add(p);
            p.Highlight();
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if(other.tag == "Person") {
            Person p = other.gameObject.GetComponent<Person>();
            peopleInArea.Remove(p);
            p.UnHighlight();
        }
    }

    void OnMouseDown() {
        house.Eat(direction);
        foreach(Person p in peopleInArea) {
            PopulationManager.RemovePerson(p);
            peopleInArea.Remove(p);
            p.OnEaten();
        }
    }
}
