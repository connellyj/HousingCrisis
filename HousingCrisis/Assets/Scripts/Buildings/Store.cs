using System.Collections;
using UnityEngine;

public class Store : House {

    public float pulseInterval;

    protected override void Awake() {
        base.Awake();
    }

    protected override void Start() {
        base.Start();
        StartCoroutine(Pulse());
    }

    // Attracts people to the store every pulse interval if it's not burning
    protected IEnumerator Pulse() {
        while(true) {
            yield return new WaitForSeconds(pulseInterval);
            if(burnState <= 0) Population.AlertPeopleAffectedByStore(type, MAX_STALL - numStalled, transform.position, stalledPeople);
        }
    }
}