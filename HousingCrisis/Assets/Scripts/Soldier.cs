using System.Collections;

public class Soldier : Person {

    private bool stalled = false;

    protected override void Start() {
        state = PersonState.TARGET;
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
        if(state == PersonState.TARGET) {
            ChangeState(PersonState.STALL);
        }else if(state == PersonState.STALL) {
            ChangeState(PersonState.TARGET);
        }else if(state == PersonState.WANDER) {
            RemovePerson();
        }
    }
}