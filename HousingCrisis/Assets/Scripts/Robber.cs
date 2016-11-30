public class Robber : Person {

    protected override void Start() {
        state = PersonState.TARGET_RANDOM;
        base.Start();
    }

    protected override void Update() {
        base.Update();
    }

    protected override void Attack() {
        House h = HouseManager.houses[GridManager.houses.IndexOf(goalIndex)];
        h.RobHouse();
        CompletePath();
    }

    protected override void CompletePath() {
        base.CompletePath();
        if(state == PersonState.TARGET_RANDOM) {
            ChangeState(PersonState.ATTACK);
        }else if(state == PersonState.ATTACK) {
            ChangeState(PersonState.PANIC);
        }else if(state == PersonState.PANIC) {
            RemovePerson();
        }
    }
}