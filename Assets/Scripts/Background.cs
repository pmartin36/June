using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour {

	private SpriteRenderer spriteRenderer;
	public Material Blur;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetColor(Color c) {
		spriteRenderer.material.SetColor("_Color", c);
	}

	void OnWillRenderObject() {
		RenderTexture temp = null;
		for(int i = 0; i < 4; i++) {
			
		}
	}
}
