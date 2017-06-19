using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionStart : Option {

	public override void SelectionPressed() {
		base.SelectionPressed();

		//Exit Game
		GameManager.Instance.StartGame();
	}
}
