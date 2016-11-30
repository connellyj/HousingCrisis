using UnityEngine;
using System.Collections;

public class Police : Person {
    
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
