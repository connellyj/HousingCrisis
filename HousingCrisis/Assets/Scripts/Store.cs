using System.Collections;
using UnityEngine;

public class Store : House {

    public float pulseInterval;

    protected override void Awake() {
        base.Awake();
    }

    protected virtual void Start() {
        StartCoroutine(Pulse());
    }

    protected IEnumerator Pulse() {
        while(true) {
            yield return new WaitForSeconds(pulseInterval);
            if(burnState == 0) Population.AlertPeopleAffectedByStore(type, alertRadius, MAX_STALL - numStalled, transform.position);
        }
    }
}