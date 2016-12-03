public class Robber : Person {

    public int minDamage; // damage done to house if it is already burning

    protected override void Start() {
        state = PersonState.TARGET_RANDOM_NOTBURNING;
        base.Start();
    }

    protected override void Update() {
        base.Update();
    }

    protected override void Attack() {
        if(HouseManager.houses.ContainsKey(goalIndex)) {
            House h = HouseManager.houses[goalIndex];
            ShootFireball();
            h.RobHouse(minDamage);
            CompletePath();
        } else CompletePath();
    }

    protected override void CompletePath() {
        base.CompletePath();
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