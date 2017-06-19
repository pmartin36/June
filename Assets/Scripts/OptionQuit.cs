using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionQuit : Option {

	public override void SelectionPressed() {
		base.SelectionPressed();

		//Exit Game
		Application.Quit();
	}
}
