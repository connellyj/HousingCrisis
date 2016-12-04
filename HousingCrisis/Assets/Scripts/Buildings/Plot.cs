public class Plot : Builder {

    private int gridIndex;

    void Start() {
        type = HouseManager.HouseType.PLOT;
        gridIndex = GridManager.CoordsToIndex((int)transform.position.x, (int)transform.position.y);
    }

    // If not occupies, opens a build menu
    void OnMouseDown() {
        if (!HouseManager.houses.ContainsKey(gridIndex)) {
            BuildMenu.Open(this);
        }
    }

    // Once built on, becomes occupied
    public override void OnBuild() {
        return;
    }
}