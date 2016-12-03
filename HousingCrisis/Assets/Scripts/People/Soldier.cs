using System.Collections;
using UnityEngine;

public class Soldier : Person {

    protected override void Start() {
        state = PersonState.TARGET_RANDOM;
        base.Start();
    }

    public override void OnSeeHouse(int houseIndex) {
        return;
    }

    protected override void Attack() {
        if(HouseManager.houses.ContainsKey(goalIndex)) {
            House h = HouseManager.houses[goalIndex];
            if(h.HasAvailableStallSpace()) {
                MoveToPosition(h.AddStalledPerson(this));
                StartCoroutine(Shoot(h));
            } else CompletePath();
        } else CompletePath();
    }

    private IEnumerator Shoot(House h) {
        while(HouseManager.houses.ContainsKey(goalIndex)) {
            ShootFireball();
            h.DamageHouse(attackValue);
            yield return new WaitForSeconds(attackStallTime);
        }
        h.RemoveStalledPerson(this);
        CompletePath();
    }

    protected override void CompletePath() {
        base.CompletePath();
        if(state == PersonState.TARGET_RANDOM) {
            ChangeState(PersonState.ATTACK);
        }else if(state == PersonState.ATTACK) {
            ChangeState(PersonState.TARGET_RANDOM);
        }else if(state == PersonState.WANDER) {
            RemovePerson();
        } else if(state == PersonState.WANDER_SET) {
            ChangeState(PersonState.STALL);
        } else if(state == PersonState.STALL) {
            ChangeState(PersonState.WANDER);
        }
    }
}