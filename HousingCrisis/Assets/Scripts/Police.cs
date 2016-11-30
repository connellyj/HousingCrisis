using UnityEngine;
using System.Collections;

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
        StartCoroutine(Shoot(h));
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
            ChangeState(PersonState.WANDER);
        } else if(state == PersonState.WANDER_SET) {
            ChangeState(PersonState.STALL);
        } else if(state == PersonState.STALL) {
            ChangeState(PersonState.WANDER);
        }
    }
}
