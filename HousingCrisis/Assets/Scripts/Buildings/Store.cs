using System.Collections;
using UnityEngine;

public class Store : House {

    public float pulseInterval;
    public GameObject pulseSpriteObject;

    protected override void Awake() {
        numNonEatAreaChildren = 3;
        pulseSpriteObject.transform.localScale = Vector3.zero;
        base.Awake();
    }

    protected override void Start() {
        base.Start();
        SpriteRenderer s = pulseSpriteObject.GetComponent<SpriteRenderer>();
        if(type == HouseManager.HouseType.BANK) s.color = Color.black;
        if(type == HouseManager.HouseType.STORE) s.color = Color.blue;
        StartCoroutine(Pulse());
    }

    // Attracts people to the store every pulse interval if it's not burning
    protected IEnumerator Pulse() {
        while(true) {
            yield return new WaitForSeconds(pulseInterval);
            if(burnState <= 0) {
                StartCoroutine(AnimatePulse());
                Population.AlertPeopleAffectedByStore(type, MAX_STALL - numStalled, transform.position, stalledPeople);
            }
        }
    }

    // Animates a pulsing ring to represent the alert area
    private IEnumerator AnimatePulse() {
        float dist = alertRadius;
        float distPerFrame = 0.2f;
        Vector3 scaleChange = new Vector3(distPerFrame, distPerFrame, 0);
        pulseSpriteObject.transform.localScale = Vector3.zero;
        while(dist > 0) {
            pulseSpriteObject.transform.localScale += scaleChange;
            dist -= distPerFrame;
            yield return new WaitForSeconds((distPerFrame / alertRadius) * 0.005f);
        }
        pulseSpriteObject.transform.localScale = Vector3.zero;
    }
}