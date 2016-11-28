using System.Collections.Generic;

public class Population {

    public List<Person> people;

    public Population() {
        people = new List<Person>();
    }

    public void AddPerson(Person p) {
        people.Add(p);
    }

    public void RemovePerson(Person p) {
        people.Remove(p);
    }

    public List<Person> GetAllPeople() {
        return people;
    }

    public void ClearPeople() {
        people.Clear();
    }
}