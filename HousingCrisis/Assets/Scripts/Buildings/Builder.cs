using UnityEngine;

public abstract class Builder : MonoBehaviour {

	public HouseManager.HouseType type;

	public abstract void OnBuild();
}