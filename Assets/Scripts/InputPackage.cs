public class InputPackage {
	public float Horizontal { get; set; }
	public float Vertical { get; set; }

	public bool Down { get; set; }
	public bool Up { get; set; }
	public bool Left { get; internal set; }
	public bool Right { get; internal set; }

	public bool Pause { get; set; }
	public bool Select { get; set; }
}
