using UnityEngine;
using System.Collections;

public class Police : Person { 

    protected override void Start() {
        state = PersonState.WANDER;
        base.Start();
    }

    // When they see a house eating, go attack that house
    public override void OnSeeHouse(int houseIndex) {
        goalIndex = houseIndex;
        ChangeState(PersonState.TARGET_SET);
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

    // Handles the state changes:
    // Police wander until they see a house eat, then they'll attack until it's destroyed or they get distracted by a store
    protected override void CompletePath() {
        StopAllCoroutines();
        if(state == PersonState.WANDER) {
            RemovePerson();
        }else if(state == PersonState.TARGET_SET) {
            ChangeState(PersonState.ATTACK);
        } else if(state == PersonState.ATTACK) {
            ChangeState(PersonState.WANDER);
        } else if(state == PersonState.WANDER_SET) {
            ChangeState(PersonState.STALL);
        } else if(state == PersonState.STALL) {
            ChangeState(PersonState.WANDER);
        }
    }
}
