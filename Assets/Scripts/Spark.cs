using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spark : MonoBehaviour {

	float InitTime { get; set; }

	// Use this for initialization
	void Start () {
		InitTime = Time.time;
	}

	public void Init(float size, GameColors c1, GameColors c2) {
		ParticleSystem ps = GetComponent<ParticleSystem>();

		//scale parameters based on the size of blades that collided
		var shape = ps.shape;
		shape.radius *= size;

		var emission = ps.emission;
		ParticleSystem.Burst[] bursts = new ParticleSystem.Burst[emission.burstCount];
		emission.GetBursts(bursts);

		short count = (short)Mathf.Pow(size, 2);
		bursts[0].minCount *= count;
		bursts[0].maxCount *= count;

		emission.SetBursts(bursts);

		var main = ps.main;
		main.startColor = new ParticleSystem.MinMaxGradient( GetColorFromGameColor(c1), GetColorFromGameColor(c2) );
	}

	void Update() {
		if( Time.time - InitTime > 2) {
			Destroy(this.gameObject);
		}
	}

	public static Color GetColorFromGameColor(GameColors c) {
		switch (c) {
			default:
			case GameColors.white:
				return new Color(1, 1, 1, 1);
			case GameColors.yellow:
				return new Color(1f, 1f, 0, 1f);
			case GameColors.red:
				return new Color(1f, 0, 0, 1f);
			case GameColors.green:
				return new Color(0, 1f, 0, 1f);
			case GameColors.blue:
				return new Color(0, 0, 1f, 1f);
			case GameColors.black:
				return new Color(0.25f, 0.25f, 0.25f, 1f);
		}
	}
}
