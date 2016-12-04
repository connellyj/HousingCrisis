public class Plot : Builder {

    private bool occupied = false;

    void Start() {
        type = HouseManager.HouseType.PLOT;
    }

    // If not occupies, opens a build menu
    void OnMouseDown() {
        if (!occupied) {
            BuildMenu.Open(this);
        }
    }

    // Once built on, becomes occupied
    public override void OnBuild() {
        occupied = true;
    }

}