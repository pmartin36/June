using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class BladeBundle {

	public AudioClip audio;
	public float InitTime { get; set; }
	public List<Blade> Blades;

	public int BladeCount {
		get {
			return Blades.Count;
		}
	}

	public BladeBundle(float initTime, List<Blade> blades) {
		InitTime =  initTime;
		Blades = blades;

		//pick audio clip

	}
}

