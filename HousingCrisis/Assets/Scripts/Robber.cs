public class Robber : Person {

    protected override void Start() {
        state = PersonState.TARGET;
        base.Start();
    }

    protected override void Update() {
        base.Update();
    }

    protected override void CompletePath() {
        base.CompletePath();
        if(state == PersonState.TARGET) {
            ChangeState(PersonState.STALL);
        }else if(state == PersonState.STALL) {
            ChangeState(PersonState.PANIC);
        }else if(state == PersonState.PANIC) {
            RemovePerson();
        }
    }
}