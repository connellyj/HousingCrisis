using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Apartment : House {

    protected override void Awake() {
        base.Awake();
        type = HouseManager.HouseType.APARTMENT;
        eatRadius = 1.5f;
        spriteWrapper.transform.position += new Vector3(0,.125f,0);
    }

    public override void Eat(Direction d) {
        if (CanEat())
        {
            isChewing = true;
            DisableEatingAreas();
            StartCoroutine(EatAnimation(d));
            Population.AlertAffectedPeopleApartment(d, gridPos, eatRadius, noticeThreshold);
        }
    }

    protected override IEnumerator EatAnimation(Direction d)
    {
        spriteRenderer.sprite = eatingSprite;
        // animate red laser
        yield return new WaitForSeconds(.5f);
        StartCoroutine(ChewAnimation());
        yield return new WaitForSeconds(chewingTime);
        isChewing = false;
        StopCoroutine("ChewingAnimation");
        spriteRenderer.sprite = defaultSprite;
        EnableEatingAreas();
    }
}
