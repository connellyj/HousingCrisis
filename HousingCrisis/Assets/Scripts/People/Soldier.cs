using System.Collections;
using UnityEngine;

public class Soldier : Person {

    protected override void Start() {
        state = PersonState.TARGET_RANDOM;
        base.Start();
    }

    // Doesn't care about seeing houses because they're always attacking anyways
    public override void OnSeeHouse(int houseIndex) {
        return;
    }

    // If the targeted house exists and has open space, go attack it
    protected override void Attack() {
        if(HouseManager.houses.ContainsKey(goalIndex)) {
            House h = HouseManager.houses[goalIndex];
            if(h.HasAvailableStallSpace()) {
                MoveToPosition(h.AddStalledPerson(this));
                StartCoroutine(Shoot(h));
            } else CompletePath();
        } else CompletePath();
    }

    // Shoots and damages the given house until it's destroyed
    private IEnumerator Shoot(House h) {
        while(HouseManager.houses.ContainsKey(goalIndex)) {
            ShootFireball();
            h.DamageHouse(attackValue);
            yield return new WaitForSeconds(attackStallTime);
        }
        h.RemoveStalledPerson(this);
        CompletePath();
    }

    // Handles state changes:
    // Soldiers will immediately attack a house until it's destroyed then attack another one unless distracted by a store
    protected override void CompletePath() {
        StopAllCoroutines();
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