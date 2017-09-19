using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour {

	private SpriteRenderer spriteRenderer;
	public Material Blur;

	Coroutine trans;

	// Use this for initialization
	void Start () {
		spriteRenderer = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetColor(GameColors c) {
		if(trans != null)
			StopCoroutine(trans);

		trans = StartCoroutine(TransitionColor(GetColorFromGameColor(c)));
	}

	public void SetColor(Color c) {
		spriteRenderer.material.SetColor("_Color", c);
	}

	IEnumerator TransitionColor(Color c) {
		float journeyTime = 1f;
		float startTime = Time.time;
		Color startColor = spriteRenderer.material.GetColor("_Color");
		while ( Time.time - startTime < journeyTime + Time.deltaTime) {
			float jtime = (Time.time - startTime) / journeyTime;
			spriteRenderer.material.SetColor("_Color", Color.Lerp( startColor, c, jtime));

			yield return new WaitForEndOfFrame();
		}
	}

	public static Color GetColorFromGameColor(GameColors c) {
		switch (c) {
			default:
			case GameColors.white:
				return new Color(0.5f, 0.5f, 0.5f, 1);
			case GameColors.yellow:
				return new Color(0.5f, 0.5f, 0.1f, 1f);
			case GameColors.red:
				return new Color(0.7f, 0.4f, 0.7f, 1f);
			case GameColors.green:
				return new Color(0, 0.5f, 0.25f, 1f);
			case GameColors.blue:
				return new Color(0.1f, 0.5f, 0.75f, 1f);
			case GameColors.black:
				return new Color(0, 0, 0, 1f);
		}
	}
}
