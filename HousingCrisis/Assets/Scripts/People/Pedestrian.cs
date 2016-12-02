public class Pedestrian : Person {

    protected override void Start() {
        state = PersonState.WANDER;
        base.Start();
    }

    protected override void Update() {
        base.Update();
    }

    protected override void CompletePath() {
        base.CompletePath();
        if(state == PersonState.PANIC) {
            GameManager.UpdateWantedLevel(1);
            RemovePerson();
        }else if(state == PersonState.WANDER) {
            RemovePerson();
        }else if(state == PersonState.WANDER_SET) {
            ChangeState(PersonState.STALL);
        }else if(state == PersonState.STALL) {
            ChangeState(PersonState.WANDER);
        }
    }
}
