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
        if(state == PersonState.PANIC) GameManager.UpdateWantedLevel(1);
        RemovePerson();
    }
}
