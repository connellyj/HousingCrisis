using UnityEngine;
using System.Collections;

public class Fireball : MonoBehaviour {

    private Vector3 direction;

    private static readonly float shootDistance = 0.5f;

	public void Shoot(Direction d) {
        direction = GridManager.DirectionToVector(d);
        StartCoroutine(PlayAnimation());
    }

    private IEnumerator PlayAnimation() {
        float dist = shootDistance;
        while(dist > 0) {
            transform.Translate(direction * 0.05f);
            dist -= 0.05f;
            yield return new WaitForSeconds(0.02f);
        }
        Destroy(gameObject);
    }
}
