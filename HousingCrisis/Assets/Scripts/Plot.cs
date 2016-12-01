using UnityEngine;
using System.Collections.Generic;

public class Plot : Builder {

    private bool occupied = false;

    void Start()
    {
        type = HouseManager.HouseType.PLOT;
    }

    void OnMouseDown() {
        if (!occupied)
        {
            BuildMenu.Open(this);
        }
    }

    public override void OnBuild() {
        occupied = true;
        Debug.Log("plot is occupied");
    }

}