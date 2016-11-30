using UnityEngine;
using System.Collections;

public class Fire : MonoBehaviour {

	SpriteRenderer spriteRenderer;
	public Sprite[] sprites = new Sprite[2];
	public float framesPerSecond;

	void Start () {
		spriteRenderer = GetComponent<SpriteRenderer>();
		StartCoroutine(Animate());
	}
	
	void Update () {
		
	}

	private IEnumerator Animate()
	{
		int frameIndex = 0;
		while (true)
		{
			spriteRenderer.sprite = sprites[frameIndex];
			frameIndex = (frameIndex + 1) % sprites.Length;
			yield return new WaitForSeconds(1f / framesPerSecond);
		}
	}
}
