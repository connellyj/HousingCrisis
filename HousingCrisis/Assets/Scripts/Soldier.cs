using System.Collections;
using UnityEngine;

public class Soldier : Person {
    
    public int attackValue;
    public int attackStallTime;

    private bool stalled = false;

    protected override void Start() {
        state = PersonState.TARGET_RANDOM;
        base.Start();
    }

    public override void OnSeeHouse(int houseIndex) {
        return;
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
        if(state == PersonState.TARGET_RANDOM) {
            ChangeState(PersonState.ATTACK);
        }else if(state == PersonState.ATTACK) {
            ChangeState(PersonState.TARGET_RANDOM);
        }else if(state == PersonState.WANDER) {
            RemovePerson();
        }
    }
}