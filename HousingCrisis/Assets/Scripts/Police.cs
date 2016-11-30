using System.Collections;

public class Police : Person {

    private bool stalled;

    protected override void Start() {
        state = PersonState.WANDER;
        base.Start();
    }

    protected override void Update() {
        base.Update();
        if(stalled && !GridManager.houses.Contains(attackGoal)) {
            stalled = false;
            CompletePath();
        }
    }

    public override void OnSeeHouse(int houseIndex) {
        attackGoal = houseIndex;
        ChangeState(PersonState.ATTACK);
    }

    protected override IEnumerator Stall() {
        stalled = true;
        yield return null;
    }

    protected override void CompletePath() {
        base.CompletePath();
        if(state == PersonState.WANDER) {
            RemovePerson();
        }else if(state == PersonState.ATTACK) {
            ChangeState(PersonState.STALL);
        } else if(state == PersonState.STALL) {
            ChangeState(PersonState.WANDER);
        } 
    }
}
