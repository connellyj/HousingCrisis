public class Pedestrian : Person {

    public int progressPerEscape;

    protected override void Start() {
        state = PersonState.WANDER;
        base.Start();
    }

    // Pedestrians don't attack houses
    protected override void Attack() {
        return;
    }

    // Handles the state changes:
    // Pedestrians will wander around until they are attracted by stores or scared by houses
    protected override void CompletePath() {
        StopAllCoroutines();
        if(state == PersonState.PANIC) {
            GameManager.UpdateWantedLevel(progressPerEscape);
            RemovePerson();
        }else if(state == PersonState.WANDER) {
            RemovePerson();
        }else if(state == PersonState.WANDER_SET) {
            ChangeState(PersonState.STALL);
        }else if(state == PersonState.STALL) {
            speed = alertSpeed / 2;
            ChangeState(PersonState.WANDER);
        }
    }
}
