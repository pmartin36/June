using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class OptionResume : Option {
	public override void SelectionPressed() {
		base.SelectionPressed();

		//GameManager unpause
		GameManager.Instance.ExitMenu();
	}
}
