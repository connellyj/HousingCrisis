using System.Collections;
using UnityEngine;

public class Store : House {

    public float pulseInterval;
    public GameObject pulseSpriteObject;
    
    private bool pulsed = false;

    protected override void Awake() {
        numNonEatAreaChildren = 3;
        base.Awake();
    }

    protected override void Start() {
        base.Start();
        StartCoroutine(Pulse());
    }

    // Attracts people to the store every pulse interval if it's not burning
    protected IEnumerator Pulse() {
        while(true) {
            pulsed = false;
            if(burnState <= 0) {
                pulsed = true;
                StartCoroutine(AnimatePulse());
            }
            yield return new WaitForSeconds(pulseInterval);
            if(burnState <= 0 && pulsed) Population.AlertPeopleAffectedByStore(type, MAX_STALL - numStalled, transform.position, stalledPeople);
        }
    }

    // Animates a pulsing ring to represent the alert area
    private IEnumerator AnimatePulse() {
        float dist = alertRadius;
        float distPerFrame = 0.05f;
        Vector3 scaleChange = new Vector3(distPerFrame, distPerFrame, 0);
        pulseSpriteObject.transform.localScale = Vector3.zero;
        while(dist > 0) {
            pulseSpriteObject.transform.localScale += scaleChange;
            dist -= distPerFrame;
            yield return new WaitForSeconds((distPerFrame / alertRadius) * pulseInterval);
        }
    }
}