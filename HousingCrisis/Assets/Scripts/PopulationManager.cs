using UnityEngine;
using System.Collections.Generic;

public class PopulationManager : MonoBehaviour {

    public List<Person> people;

    private static PopulationManager instance;

    void Awake() {
        instance = this;
        people = new List<Person>();
    }

    public static void AddPerson(Person p) {
        instance.people.Add(p);
    }

    public static void RemovePerson(Person p) {
        instance.people.Remove(p);
    }

    public static List<Person> GetAllPeople() {
        return instance.people;
    }
}
