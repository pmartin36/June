using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class BladeInfo {
	public Direction BladeDirection { get; set; }
	public GameColors BladeColor { get; set; }
	public float Size { get; set; }
	public float Position { get; set; }
	public float InitTime { get; set; }
	public int OrderInLayer { get; set; }

	public AudioClip AudioClip { get; set; }
}

