<<<<<<< HEAD
﻿using UnityEngine;
using System.Collections;
=======
﻿using System.Collections;
using UnityEngine;
>>>>>>> 52d114b4b5cac5bfd094086cca6f7283aeb548e1

public class Police : Person {

    private bool stalled;
    public int attackValue;
    public int attackStallTime;

    protected override void Start() {
        state = PersonState.WANDER;
        base.Start();
    }

    public override void OnSeeHouse(int houseIndex) {
        goalIndex = houseIndex;
        ChangeState(PersonState.TARGET_SET);
    }

    protected override void Attack() {
        House h = HouseManager.houses[GridManager.houses.IndexOf(goalIndex)];
        if(h.HasAvailableStallSpace()) {
            MoveToPosition(h.AddStalledPerson(X(), Y()));
            StartCoroutine(Shoot(h));
        } else CompletePath();
    }

    private IEnumerator Shoot(House h) {
        while(HouseManager.houses.Contains(h)) {
            h.DamageHouse(attackValue);
            yield return new WaitForSeconds(attackStallTime);
        }
        CompletePath();
    }

    protected override void CompletePath() {
        base.CompletePath();
        if(state == PersonState.WANDER) {
            RemovePerson();
        }else if(state == PersonState.TARGET_SET) {
            ChangeState(PersonState.ATTACK);
        } else if(state == PersonState.ATTACK) {
            ResetPosition();
            ChangeState(PersonState.WANDER);
        } else if(state == PersonState.WANDER_SET) {
            ChangeState(PersonState.STALL);
        } else if(state == PersonState.STALL) {
            ChangeState(PersonState.WANDER);
        }
    }
}
