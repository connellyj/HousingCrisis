using System.Collections;
using UnityEngine;

public class Soldier : Person {
    
    public int attackValue;
    public int attackStallTime;

    protected override void Start() {
        state = PersonState.TARGET_RANDOM;
        base.Start();
    }

    public override void OnSeeHouse(int houseIndex) {
        return;
    }

    protected override void Attack() {
        int hIndex = GridManager.houses.IndexOf(goalIndex);
        House h;
        if(hIndex < 0) {
            h = HouseManager.houses[GridManager.burningHouses.IndexOf(goalIndex)];
        } else {
            h = HouseManager.houses[hIndex];
        }
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
        if(state == PersonState.TARGET_RANDOM) {
            ChangeState(PersonState.ATTACK);
        }else if(state == PersonState.ATTACK) {
            ResetPosition();
            ChangeState(PersonState.TARGET_RANDOM);
        }else if(state == PersonState.WANDER) {
            RemovePerson();
        }
    }
}