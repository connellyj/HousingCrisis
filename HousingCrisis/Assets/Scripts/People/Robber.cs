public class Robber : Person {

    public int minDamage;

    protected override void Start() {
        state = PersonState.TARGET_RANDOM_NOTBURNING;
        base.Start();
    }

    // If the targeted house exists, sets it on fire
    protected override void Attack() {
        if(HouseManager.houses.ContainsKey(goalIndex)) {
            House h = HouseManager.houses[goalIndex];
            ShootFireball();
            h.RobHouse(minDamage);
            CompletePath();
        } else CompletePath();
    }

    // Handles state changes:
    // Targets a not-burning house and sets it on fire, unless distracted by a store or freaked out by a house eating
    protected override void CompletePath() {
        StopAllCoroutines();
        if(state == PersonState.TARGET_RANDOM_NOTBURNING) {
            ChangeState(PersonState.ATTACK);
        }else if(state == PersonState.ATTACK) {
            ChangeState(PersonState.PANIC);
        }else if(state == PersonState.PANIC) {
            RemovePerson();
        } else if(state == PersonState.WANDER_SET) {
            ChangeState(PersonState.STALL);
        } else if(state == PersonState.STALL) {
            speed = alertSpeed / 2;
            ChangeState(PersonState.WANDER);
        }
    }
}