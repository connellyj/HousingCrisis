public class Robber : Person {

    public int minDamage; // damage done to house if it is already burning

    protected override void Start() {
        state = PersonState.TARGET_RANDOM;
        base.Start();
    }

    protected override void Update() {
        base.Update();
    }

    protected override void Attack() {
        int hIndex = GridManager.houses.IndexOf(goalIndex);
        House h;
        if(hIndex < 0) {
            h = HouseManager.houses[GridManager.burningHouses.IndexOf(goalIndex)];
        } else {
            h = HouseManager.houses[hIndex];
        }
        MoveToPosition(h.transform.position);
        h.RobHouse(minDamage);
        CompletePath();
    }

    protected override void CompletePath() {
        base.CompletePath();
        if(state == PersonState.TARGET_RANDOM) {
            ChangeState(PersonState.ATTACK);
        }else if(state == PersonState.ATTACK) {
            ResetPosition();
            ChangeState(PersonState.PANIC);
        }else if(state == PersonState.PANIC) {
            RemovePerson();
        }
    }
}