using UnityEngine;
using System.Collections.Generic;

public class EatingArea : MonoBehaviour {

    public Direction direction;

    private List<Person> peopleInArea;
    private Population population;
    private House house;

    void Awake() {
        peopleInArea = new List<Person>();
        house = transform.parent.gameObject.GetComponent<House>();
        population = GameManager.GetPopulation();
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
        List<Person> toRemove = new List<Person>();
        foreach(Person p in peopleInArea) {
            population.RemovePerson(p);
            toRemove.Add(p);
            p.OnEaten();
        }
        foreach(Person p in toRemove) peopleInArea.Remove(p);
    }
}
